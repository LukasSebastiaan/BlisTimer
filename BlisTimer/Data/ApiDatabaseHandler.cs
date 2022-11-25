using BlisTimer.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimplicateAPI;
using SimplicateAPI.Client;

namespace BlisTimer.Data;

public class ApiDatabaseHandler
{
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
        
        _logger.LogInformation("Deleting all projects and linked activities/hourtypes from database that" +
                               "aren't in the Simplicate database anymore");
        // Delete all projects that arent in Simplicate anymore from the database 
        var notExistingInSimplicateProjects = _dbContext.Projects.Select(x => x.Id).ToList()
                                                .Except(projects.Select(x => x.Id)).ToList();
        
        // TODO: remove HourTypes of Activities from DB
        _dbContext.WorkActivities.RemoveRange(_dbContext.WorkActivities.Where(x => notExistingInSimplicateProjects.Contains(x.ProjectId)));
        _dbContext.Projects.RemoveRange(_dbContext.Projects.Where(x => notExistingInSimplicateProjects.Contains(x.Id)));
        
        
        // TODO: Remove HourTypes of activities that are not in Simplicate anymore
        // TODO: before deleting, check if any other activity has that HourType
        _logger.LogInformation("Deleting all services that aren't in simplicate anymore in general");
        var notExistingInSimplicateServices = _dbContext.WorkActivities.Select(x => x.Id).ToList()
                                                .Except(services.Select(x => x.ServiceId)).ToList();
        _dbContext.WorkActivities.RemoveRange(
                _dbContext.WorkActivities.Where(x => notExistingInSimplicateServices.Contains(x.Id)));
        
        _logger.LogInformation("Going through all the Simplicate projects/services and adding them to our database...");
        foreach (var project in projects)
        {
            #region AddProjectToDbWithEmployees
            // Getting the employees from the project that are already in our database
            // so that we can link the employees to the project
            List<string> employeesIdsInDb = new();

            if (project.Employees is not null)
            {
                employeesIdsInDb = project.Employees.Select(x => x.EmployeeId).ToList()
                    .Intersect(_dbContext.Employees.Select(x => x.Id)).ToList() ?? new();
            }

            var projectEmployees = _dbContext.Employees.Where(_ => employeesIdsInDb.Contains(_.Id)).ToList();
            
            _logger.LogInformation("Checking if database already contains the project of simplicate");
            if (!_dbContext.Projects.Select(x => x.Id).Contains(project.Id))
            {
                _logger.LogInformation("Adding project from Simplicate to own database");
                await _dbContext.Projects.AddAsync(new Project()
                {
                    Employees = projectEmployees,
                    Id = project.Id,
                    Name = project.Name,
                });
            }
            else
            {
                _logger.LogInformation("Found project, updating the information");
                var projectDbRecord = _dbContext.Projects.FirstOrDefault(x => x.Id == project.Id);
                
                projectDbRecord!.Employees = projectEmployees;

                if (!projectDbRecord.Name.Equals(project.Name))
                    projectDbRecord.Name = project.Name;

            }
            #endregion

            #region AddProjectActivitiesToDb
            // Getting the services that are attached to this project and adding them to the database with
            // the project's id
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
            }
            #endregion
        }
        
        await _dbContext.SaveChangesAsync();
    }
}