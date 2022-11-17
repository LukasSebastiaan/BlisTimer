using SimplicateAPI;
using SimplicateAPI.Client;

namespace BlisTimer.Data;

public class ApiDatabaseHandler
{
    public static readonly SimplicateApi SimplicateApiClient = new SimplicateApi(
        hostUrl: "hr2022.simplicate.nl",
        apiKey: "HInAJkEpNHKXNZfDFkRs96blsgCSYF4g",
        apiSecret: "bvyi1UPanMisCNaeM4YtHFOpkk0UVd5C"
    );
}