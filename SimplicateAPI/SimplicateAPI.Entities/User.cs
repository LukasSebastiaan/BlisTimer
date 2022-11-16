using System.Text.Json.Serialization;

namespace SimplicateAPI.Enitities;

public class User
{
    [JsonPropertyName("username")] public string Username { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("gender")] public object Gender { get; set; }
    [JsonPropertyName("first_name")] public string FirstName { get; set; }
    [JsonPropertyName("family_name")] public string FamilyName { get; set; }
    [JsonPropertyName("birthdate")] public string Birthdate { get; set; }
    [JsonPropertyName("is_authy_enabled")] public bool IsAuthyEnabled { get; set; }
    [JsonPropertyName("is_employee")] public bool IsEmployee { get; set; }
    [JsonPropertyName("is_light_user")] public bool IsLightUser { get; set; }
    [JsonPropertyName("employee_id")] public string EmployeeId { get; set; }
    [JsonPropertyName("person_id")] public string PersonId { get; set; }
    [JsonPropertyName("is_blocked")] public bool IsBlocked { get; set; }
    [JsonPropertyName("is_lock_nav")] public bool IsLockNav { get; set; }
    [JsonPropertyName("key_identifier")] public string KeyIdentifier { get; set; }
    [JsonPropertyName("timezone")] public Timezone timezone { get; set; }
    [JsonPropertyName("is_account_owner")] public bool IsAccountOwner { get; set; }
    [JsonPropertyName("has_external_agenda_integration")]
    public bool HasExternalAgendaIntegration { get; set; }

    public class Timezone
    {
        [JsonPropertyName("id")] public string Id { get; set; }
        [JsonPropertyName("timezone")] public string timezone { get; set; }
        [JsonPropertyName("location")] public string Location { get; set; }
        [JsonPropertyName("description")] public string Description { get; set; }
    }
}