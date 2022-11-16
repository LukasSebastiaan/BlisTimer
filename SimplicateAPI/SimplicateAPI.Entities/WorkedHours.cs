using System.Text.Json.Serialization;

namespace SimplicateAPI.Enitities;

public record Corrections(
        [property: JsonPropertyName("amount")] int Amount,
        [property: JsonPropertyName("value")] int Value,
        [property: JsonPropertyName("last_correction_date")] string LastCorrectionDate
    );

    public record EmployeeData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name
    );

    public record IsDeletable(
        [property: JsonPropertyName("value")] bool Value
    );

    public record IsEditable(
        [property: JsonPropertyName("value")] bool Value
    );

    public record OrganizationData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name
    );

    public record ProjectData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("project_number")] string ProjectNumber,
        [property: JsonPropertyName("organization")] OrganizationData Organization
    );

    public record ProjectserviceData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("default_service_id")] string DefaultServiceId,
        [property: JsonPropertyName("revenue_group_id")] string RevenueGroupId
    );

    public record WorkedHours(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("employee")] EmployeeData EmployeeData,
        [property: JsonPropertyName("project")] ProjectData ProjectData,
        [property: JsonPropertyName("projectservice")] ProjectserviceData ProjectserviceData,
        [property: JsonPropertyName("type")] TypeData Type,
        [property: JsonPropertyName("tariff")] int Tariff,
        [property: JsonPropertyName("created_at")] string CreatedAt,
        [property: JsonPropertyName("updated_at")] string UpdatedAt,
        [property: JsonPropertyName("locked")] bool Locked,
        [property: JsonPropertyName("is_editable")] IsEditable IsEditable,
        [property: JsonPropertyName("is_deletable")] IsDeletable IsDeletable,
        [property: JsonPropertyName("corrections")] Corrections Corrections,
        [property: JsonPropertyName("is_productive")] bool IsProductive,
        [property: JsonPropertyName("hours")] double Hours,
        [property: JsonPropertyName("start_date")] string StartDate,
        [property: JsonPropertyName("end_date")] string EndDate,
        [property: JsonPropertyName("is_time_defined")] bool IsTimeDefined,
        [property: JsonPropertyName("is_recurring")] bool IsRecurring,
        [property: JsonPropertyName("is_external")] bool IsExternal,
        [property: JsonPropertyName("billable")] bool Billable,
        [property: JsonPropertyName("should_sync_to_cronofy")] bool ShouldSyncToCronofy,
        [property: JsonPropertyName("source")] string Source
    );

    public record TypeData(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("vatclass")] Vatclass Vatclass,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("tariff")] string Tariff,
        [property: JsonPropertyName("color")] string Color
    );

    public record Vatclass(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("percentage")] int Percentage
    );