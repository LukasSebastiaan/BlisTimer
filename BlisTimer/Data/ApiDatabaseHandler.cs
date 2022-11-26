using BlisTimer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimplicateAPI;
using SimplicateAPI.Client;
using SimplicateAPI.Enitities;
using Project = BlisTimer.Models.Project;

namespace BlisTimer.Data;

public class ApiDatabaseHandler
{
    /// <summary>
    /// Simplicate Api client that you can use to access the Simplicate Api
    /// </summary>
    public readonly SimplicateApi SimplicateApiClient = new SimplicateApi(
        hostUrl: "hr2022.simplicate.nl",
        apiKey: "HInAJkEpNHKXNZfDFkRs96blsgCSYF4g",
        apiSecret: "bvyi1UPanMisCNaeM4YtHFOpkk0UVd5C"
    );
    
    private readonly TimerDbContext _dbContext;
    private readonly ILogger<ApiDatabaseHandler> _logger;
    
    public ApiDatabaseHandler(TimerDbContext dbContext, ILogger<ApiDatabaseHandler> logger) {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task SyncDbWithSimplicate()
    {
        _logger.LogInformation("Getting projects and services (activity) information from Simplicate Api");
        // get projects from simplicate and also all services
        var projectsTask = SimplicateApiClient.Projects.GetProjects();
        var servicesTask = SimplicateApiClient.Projects.GetServices();

        var projects = await projectsTask;
        var services = await servicesTask;
        
        ClearRemovedSimplicateData(projects, services);

        _logger.LogInformation("Going through all the Simplicate projects/services and adding them to our database...");
        foreach (var project in projects)
        {
            // Getting the employees from the project that are already in our database
            // so that we can link the employees to the project
            await AddOrUpdateProjectWithEmployees(project);
            
            // Getting the services that are attached to this project and adding them to the database with
            // the project's id
            await AddOrUpdateProjectActivities(services, project);
        }
        
        // Saving the changes made to the database
        await _dbContext.SaveChangesAsync();
    }

    private async Task AddOrUpdateProjectActivities(Service[] services, SimplicateAPI.Enitities.Project project)
    {
        var existingWorkActivities = _dbContext.WorkActivities.Select(x => x.Id);

        foreach (var activity in services)
        {
            if (activity.Name is null || activity.ProjectId != project.Id)
                continue;

            if (existingWorkActivities.Contains(activity.ServiceId))
            {
                var activityDbRecord = _dbContext.WorkActivities.FirstOrDefault(x => x.Id == activity.ServiceId);
                activityDbRecord!.Name = activity.Name;
                activityDbRecord!.ProjectId = activity.ProjectId;
                continue;
            }

            await _dbContext.WorkActivities.AddAsync(new WorkActivity()
            {
                Id = activity.ServiceId,
                Name = activity.Name ?? "No Name",
                ProjectId = activity.ProjectId,
            });
            
            // Unlink all activities that have been unlinked from a project at Simplicate's side
            var removedHourTypeIdsFromService = _dbContext.WorkActivityHourTypes
                .Where(x => x.WorkActivityId == activity.ServiceId).AsEnumerable()
                .Select(x => x.HourTypeId).AsEnumerable()
                .Except(activity.HourTypes.Select(x => x.HourstypeInfo.Id).AsEnumerable());
            
            _dbContext.WorkActivityHourTypes
                .RemoveRange(_dbContext.WorkActivityHourTypes.Where(_ => _.WorkActivityId == activity.ServiceId 
                                                                         && removedHourTypeIdsFromService.Contains(_.HourTypeId)));

            foreach (var activityHourType in activity.HourTypes)    
            {
                // Add HourType to db if it doesn't exist yet
                if (!_dbContext.HourTypes.Any(x => x.HourTypeId == activityHourType.HourstypeInfo.Id))
                {
                    if (!_dbContext.HourTypes.Local.Any(x => x.HourTypeId == activityHourType.HourstypeInfo.Id))
                    {
                        await _dbContext.HourTypes.AddAsync(new()
                        {
                            HourTypeId = activityHourType.HourstypeInfo.Id,
                            Label = activityHourType.HourstypeInfo.Label,
                            Type = activityHourType.HourstypeInfo.Type,
                        });
                    }
                }
                else
                {
                    var dbHourTypeRecord =
                        _dbContext.HourTypes.FirstOrDefault(x => x.HourTypeId == activityHourType.HourstypeInfo.Id);

                    dbHourTypeRecord!.Label = activityHourType.HourstypeInfo.Label;
                    dbHourTypeRecord!.Type = activityHourType.HourstypeInfo.Type;
                }
                
                // Add record to WorkActivityHourTypes to link the HoursType to the work activity if it doesn't
                // exist yet in the database
                if (!_dbContext.WorkActivityHourTypes.Any(x => x.HourTypeId == activityHourType.HourstypeInfo.Id 
                                                               && x.WorkActivityId == activity.ServiceId)) 
                    await _dbContext.WorkActivityHourTypes.AddAsync(new() {
                        HourTypeId = activityHourType.HourstypeInfo.Id,
                        WorkActivityId = activity.ServiceId,
                    });
            }
        }
    }

    private async Task AddOrUpdateProjectWithEmployees(SimplicateAPI.Enitities.Project project)
    {
        _logger.LogInformation("Checking if database already contains the project of simplicate");
        if (!_dbContext.Projects.Select(x => x.Id).Contains(project.Id))
        {
            _logger.LogInformation("Adding project from Simplicate to own database");
            await _dbContext.Projects.AddAsync(new Project()
            {
                Id = project.Id,
                Name = project.Name,
            });
        }
        else
        {
            _logger.LogInformation("Found project, updating the information");
            var projectDbRecord =
                _dbContext.Projects.Include(_ => _.EmployeeProjects).FirstOrDefault(x => x.Id == project.Id);

            _dbContext.EmployeeProjects.RemoveRange(projectDbRecord.EmployeeProjects);

            if (!projectDbRecord!.Name.Equals(project.Name))
                projectDbRecord.Name = project.Name;
        }

        if (project.Employees is not null)
        {
            var employeesIdsInDb = project.Employees.Select(x => x.EmployeeId)
                .Intersect(_dbContext.Employees.Select(x => x.Id)).ToList() ?? new();

            foreach (var employeeId in employeesIdsInDb)
            {
                await _dbContext.EmployeeProjects.AddAsync(new()
                {
                    EmployeeId = employeeId,
                    ProjectId = project.Id,
                });
            }
        }
    }

    private void ClearRemovedSimplicateData(SimplicateAPI.Enitities.Project[] projects, SimplicateAPI.Enitities.Service[] services)
    {
        _logger.LogInformation("Deleting all projects and linked activities/hourtypes from database that" +
                               "aren't in the Simplicate database anymore");
        // Delete all projects that arent in Simplicate anymore from the database 
        var notExistingInSimplicateProjects = _dbContext.Projects.Select(x => x.Id).AsEnumerable()
            .Except(projects.Select(x => x.Id)).ToList();

        var toBeDeletedActivities = _dbContext.WorkActivities.Where(x => notExistingInSimplicateProjects.Contains(x.ProjectId));
        
        // Deleting hour types of project activities do be deleted
        _dbContext.WorkActivityHourTypes.RemoveRange(
            _dbContext.WorkActivityHourTypes.Where(x => toBeDeletedActivities.Select(_ => _.Id).Contains(x.WorkActivityId)));
        
        // Deleting WorkActivities of project that are to be deleted
        _dbContext.WorkActivities.RemoveRange(
            _dbContext.WorkActivities.Where(x => notExistingInSimplicateProjects.Contains(x.ProjectId)));
        _dbContext.Projects.RemoveRange(_dbContext.Projects.Where(x => notExistingInSimplicateProjects.Contains(x.Id)));


        _logger.LogInformation("Deleting all services that aren't in Simplicate anymore in general");
        // TODO: Remove HourTypes of activities that are not in Simplicate anymore
        // TODO: before deleting, check if any other activity has that HourType
        // This requires adding another endpoint to the SimplicateAPI  project
        
        var notExistingInSimplicateServices = _dbContext.WorkActivities.Select(x => x.Id).AsEnumerable()
            .Except(services.Select(x => x.ServiceId)).ToList();
        
        _dbContext.WorkActivities.RemoveRange(
            _dbContext.WorkActivities.Where(x => notExistingInSimplicateServices.Contains(x.Id)));
    }
}