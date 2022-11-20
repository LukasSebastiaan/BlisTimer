using System.Text.Json;
using System.Text.Json.Nodes;

namespace SimplicateAPI.Client;

public class SimplicateRequestClient
{
    private HttpClient HttpClient { get; }

    public SimplicateRequestClient(HttpClient httpClient) =>
        HttpClient = httpClient;


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

    public async Task PostRequestAsync(string relativeUrl, HttpContent content)
    {
        var httpResponse = await HttpClient.PostAsync(relativeUrl, content);
        httpResponse.EnsureSuccessStatusCode();
    }
}