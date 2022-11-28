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
            $"hours/hours?limit={limit}");
    }

    public async Task AddHours()
    {
        // {
        //     "employee_id": "employee:be93045f0f01e63a",
        //     "project_id": "project:6cc70702c221b840",
        //     "projectservice_id": "service:8c29d20520ed8ffe",
        //     "type_id": "hourstype:f902fc5514b044a2",
        //     "approvalstatus_id": "approvalstatus:9a4660a21af7234e",
        //     "address_id": "address:6036ffae58d1b855e57e4617bc5cd25c",
        //     "hours": 10,
        //     "start_date": "15-11-2022 8:00:00",
        //     "end_date": "15-11-2022 18:00:00",
        //     "is_time_defined": false,
        //     "is_external": false,
        //     "billable": false,
        //     "should_sync_to_cronofy": true,
        //     "source": "Blis Timer"
        // }
        
    }

    public async Task<WorkedHours> GetHoursById(string id)
    {
        if (!id.Contains(':'))
            throw new Exception("invalid input id: " + id);

        return await RequestClient.GetRequestAsync<WorkedHours>(
            $"hours/hours/{id.Replace(":", "%3A")}");
    }
}