using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimplicateAPI.Client;
using SimplicateAPI;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SimplicateAPI.Client;

public sealed class SimplicateApi
{
    private SimplicateRequestClient RequestClient;
    
    /// <summary>
    /// Contains methods for getting and setting information in the Hrm endpoint
    /// </summary>
    public Endpoints.Hrm Hrm { get; }
    public Endpoints.Hours Hours { get; }
    public Endpoints.Login Login { get; }
    public Endpoints.Projects Projects { get; }


    public SimplicateApi(string hostUrl, string apiKey, string apiSecret)
    {
        // Setting up de httpClient headers and accepted headers
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        httpClient.BaseAddress = new($"https://{hostUrl}/api/v2/");
        httpClient.DefaultRequestHeaders.Add("Authentication-Key", apiKey);
        httpClient.DefaultRequestHeaders.Add("Authentication-Secret", apiSecret);

        RequestClient = new(httpClient);

        Hrm = new(RequestClient);
        Hours = new(RequestClient);
        Projects = new(RequestClient);
        Login = new(new($"https://{hostUrl}/"));
    }
}