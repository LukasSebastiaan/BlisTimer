using SimplicateAPI.Enitities;
using SimplicateAPI.Client;

namespace SimplicateAPI.Endpoints;

public sealed class Hrm
{
    private SimplicateRequestClient RequestClient { get; }

    public Hrm(SimplicateRequestClient simplicateRequestClient)
    {
        RequestClient = simplicateRequestClient;
    }
    
    /// <summary>
    /// Gets all the employees that are listed in the HRM section in Simplicate. Optional parameter can be passed
    /// to specify a maximum number of employees returned (defaults to 5). In simplicate's database, employees are
    /// not required to have all their information filled out, if an employee does not have certain data attached
    /// to them an empty string will be stored in the property. If something went wrong an HttpRequestException
    /// is thrown.
    /// </summary>
    /// <returns>Array of type Employee</returns>
    public async Task<Employee[]> GetEmployees(int limit = 5)
    {
        return await RequestClient.GetRequestAsync<Employee[]>(
            $"hrm/employee?limit={limit}");
    }

    /// <summary>
    /// Gets a specified employee's data by Id. If the employee does not have some information filled out that's
    /// required for a property of the Employee class, then the property will be set to an empty string
    /// </summary>
    /// <param name="id">The employee id of the one you want to get</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Employee> GetEmployeeById(string id)
    {
        if (!id.Contains(':'))
            throw new Exception("invalid input id: " + id);

        return await RequestClient.GetRequestAsync<Employee>(
            $"hrm/employee/{id.Replace(":", "%3A")}");
    }
}