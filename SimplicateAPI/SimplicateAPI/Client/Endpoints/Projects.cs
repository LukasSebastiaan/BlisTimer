using SimplicateAPI.Client;
using SimplicateAPI.Enitities;

namespace SimplicateAPI.Endpoints;

public sealed class Projects
{
    private SimplicateRequestClient RequestClient { get; }
    
    public Projects(SimplicateRequestClient simplicateRequestClient) =>
        RequestClient = simplicateRequestClient;

    /// <summary>
    /// Gets all the projects from the Simplicate API, returns a list of <see cref="Project"/> objects.
    /// </summary>
    /// <param name="limit"></param>
    /// <returns></returns>
    public async Task<Project[]> GetProjects(int limit = 5)
    {
        return (await RequestClient.GetRequestAsync<Project[]>(
            $"projects/project?limit={limit}")) ?? Array.Empty<Project>();
    }

    /// <summary>
    /// Gets a project from the Simplicate API, returns a <see cref="Project"/> object.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Project?> GetProjectById(string id)
    {
        return await RequestClient.GetRequestAsync<Project>(
            $"hours/hours/{id.Replace(":", "%3A")}");
    }

    public async Task<Service[]> GetServices(int limit = 5)
    {
        return (await RequestClient.GetRequestAsync<Service[]>(
            $"projects/service?limit={limit}")) ?? Array.Empty<Service>();
    }
}