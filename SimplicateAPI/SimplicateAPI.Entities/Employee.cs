using System.Text.Json.Serialization;

namespace SimplicateAPI.Enitities;

public sealed class Employee
{
    [JsonPropertyName("id")] public string Id { get; set; }
    [JsonPropertyName("is_user")] public bool IdUser { get; set; }
    [JsonPropertyName("person_id")] public string PersonId { get; set; }
    [JsonPropertyName("name")] public string Name { get; set; }
    [JsonPropertyName("bank_account")] public string BankAccount { get; set; }
    [JsonPropertyName("function")] public string Function { get; set; }
    [JsonPropertyName("employment_status")] public string EmploymentStatus { get; set; }
    [JsonPropertyName("work_email")] public string WorkEmail { get; set; }
    [JsonPropertyName("hourly_sales_tariff")] public string HourlySalesTariff { get; set; }
    [JsonPropertyName("hourly_cost_tariff")] public string HourlyCostTariff { get; set; }
    [JsonPropertyName("created")] public string Created { get; set; }
    [JsonPropertyName("modified")] public string Modified { get; set; }
    [JsonPropertyName("simplicate_url")] public string SimplicateUrl { get; set; }
    [JsonPropertyName("timeline_email_address")] public string TimelineEmailAddress { get; set; }
}