using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimplicateAPI.Client;

public class SimplicateRequestClient
{
    private HttpClient HttpClient { get; }

    public SimplicateRequestClient(HttpClient httpClient) =>
        HttpClient = httpClient;

    /// <summary>
    /// Makes a request to the Simplicate API and does all the necessary error handling.
    /// </summary>
    /// <param name="relativeUrl">The relative url to the API endpoint</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>A generic type containing the response of the request</returns>
    public async Task<T?> GetRequestAsync<T>(string relativeUrl) where T : class
    {
        var httpResponse = await HttpClient.GetAsync(relativeUrl);

        if (!httpResponse.IsSuccessStatusCode)
            return null;

        var responseJson = JsonNode.Parse(await httpResponse.Content.ReadAsStringAsync());

        // Checking if the response actually contains valid content
        if (ReferenceEquals(responseJson, null) || responseJson["data"] == null
                                                || responseJson["data"]!.ToString() == ""
                                                || responseJson["data"]!.ToString() == "[]") {
            return null;
        }

        return JsonSerializer.Deserialize<T>(responseJson["data"]!.ToJsonString())!;
    }

    /// <summary>
    /// Posts a request to the Simplicate API and does all the necessary error handling.
    /// </summary>
    /// <param name="relativeUrl">The relative url to the API endpoint</param>
    /// <param name="content">The HttpContent to post the the endpoint</param>
    /// <returns>A Http Status code that is returned from the request</returns>
    public async Task<HttpStatusCode> PostRequestAsync(string relativeUrl, HttpContent content)
    {
        var httpResponse = await HttpClient.PostAsync(relativeUrl, content);
        
        return httpResponse.StatusCode;
    }
}