using System.Net;
using System.Net.Http.Headers;
using SimplicateAPI.Enitities;
using SimplicateAPI.Client;
using System.Text.Json;
using System.Text.Json.Nodes;
using SimplicateAPI.ReturnTypes;

namespace SimplicateAPI.Endpoints;

public sealed class Login
{
    private Uri SimplicateBaseAddress { get; init; }
    
    public Login(Uri baseAddress) =>
        SimplicateBaseAddress = baseAddress;
    
    /// <summary>
    /// Tries to login to Simplicate with the given credentials. This method will return a <see cref="User"/> if the login was successful, but
    /// this method's reliability has not been tested yet. There is a chance it might fail, even if the credentials are correct.
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    /// <exception cref="HttpRequestException"></exception>
    public async Task<LoginResult> TryUnsafeLoginAsync(string username, string password)
    {
        // Setting the client up with the base address
        var client = new HttpClient();
        client.BaseAddress = SimplicateBaseAddress;
        
        // Logging in the HttpClient to simplicate with the given credentials so we can
        // try to get the user's data later.
        await client.PostAsync("/site/login", new FormUrlEncodedContent(new[] {
            new KeyValuePair<string, string>("LoginForm[username]", $"{username}"),
            new KeyValuePair<string, string>("LoginForm[password]", $"{password}"),
            new KeyValuePair<string, string>("LoginForm[rememberMe]", "0"),
        }));
        
        // Trying to get the user's data by going to a url that is only authorized for logged in users.
        var response = await client.GetAsync("/api/v2/users/user");
       
        // If the response is not successful, we return the proper LoginResult.
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    return LoginResult.BadCredentials;
                default:
                    return LoginResult.Failed;
            }
        }
        
        // If the response is successful, we deserialize the response content to a User object.
        // but first we need to check if the response content is empty or not.
        var responseJson = JsonNode.Parse(await response.Content.ReadAsStringAsync());
        if (ReferenceEquals(responseJson, null) 
            || responseJson["data"] == null
            || responseJson["data"]!.ToString() == ""
            || responseJson["data"]!.ToString() == "[]")
            return LoginResult.Failed;


        // Check if we can successfully deserialize the user object. If not, we return the proper LoginResult.
        User? user = JsonSerializer.Deserialize<User>(responseJson["data"]!.ToJsonString());
        if (user is null)
            return LoginResult.Failed;
    
        return new() {
            Status = LoginResult.LoginStatus.Success,
            User = user,
        };
    }
}