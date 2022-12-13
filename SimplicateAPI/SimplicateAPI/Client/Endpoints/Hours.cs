using System.Net;
using Domain.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using SimplicateAPI.Client;
using SimplicateAPI.Enitities;

namespace SimplicateAPI.Endpoints;

public sealed class Hours
{
    public Hours(SimplicateRequestClient simplicateRequestClient)
    {
        RequestClient = simplicateRequestClient;
    }

    private SimplicateRequestClient RequestClient { get; }

    /// <summary>
    ///     Get all the worked hours from all employees in one array. Default return
    ///     limit is set to 5
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<WorkedHours[]> GetHours(int limit = 5)
    {
        return await RequestClient.GetRequestAsync<WorkedHours[]>(
            $"hours/hours?limit={limit}") ?? Array.Empty<WorkedHours>();
    }

    public async Task<HttpStatusCode> AddHour(TimeLog hourToSubmit, string projectId)
    {
        // {
        //     "employee_id": "employee:be93045f0f01e63a",
        //     "project_id": "project:6cc70702c221b840",
        //     "projectservice_id": "service:cb06f818e9b413da",
        //     "type_id": "hourstype:a8df4761fe85a646",
        //     "hours": 10,
        //     "start_date": "2022-12-04 8:00:00",
        //     "end_date": "2022-12-04 18:00:00",
        //     "is_time_defined": false,
        //     "is_external": false,
        //     "billable": false,
        //     "source": "Blis Timer"
        // }
        
        
        var hourJson = new JsonObject();
        hourJson.Add("employee_id", hourToSubmit.EmployeeId);
        hourJson.Add("project_id", hourToSubmit.ProjectId);
        hourJson.Add("projectservice_id", hourToSubmit.ActivityId);
        hourJson.Add("type_id", hourToSubmit.HourTypeId);
        hourJson.Add("hours", (hourToSubmit.EndTime - hourToSubmit.StartTime!).TotalHours);
        hourJson.Add("start_date", hourToSubmit.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));
        hourJson.Add("end_date", hourToSubmit.EndTime.ToString("yyyy-MM-dd HH:mm:ss"));
        hourJson.Add("is_time_defined", true);
        hourJson.Add("is_external", false);
        hourJson.Add("billable", false);
        hourJson.Add("source", "Blis Timer");

        var hoursJsonString = JsonSerializer.Serialize(hourJson);
        
        var httpContent = new StringContent(hoursJsonString, Encoding.UTF8, "application/json");

        return await RequestClient.PostRequestAsync("hours/hours" , httpContent);
    }

    public async Task<WorkedHours> GetHoursById(string id)
    {
        if (!id.Contains(':'))
            throw new Exception("invalid input id: " + id);

        return await RequestClient.GetRequestAsync<WorkedHours>(
            $"hours/hours/{id.Replace(":", "%3A")}");
    }
}