using System.Text.Json.Serialization;

namespace SimplicateAPI.Enitities;

    public record Hourstype(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("type")] string Type,
        [property: JsonPropertyName("label")] string Label,
        [property: JsonPropertyName("blocked")] bool? Blocked,
        [property: JsonPropertyName("color")] string Color
    );

    public record HourType(
        [property: JsonPropertyName("id")] string Id,
        [property: JsonPropertyName("hourstype")] Hourstype HourstypeInfo,
        [property: JsonPropertyName("budgeted_amount")] int? BudgetedAmount,
        [property: JsonPropertyName("tariff")] int? Tariff,
        [property: JsonPropertyName("billable")] bool? Billable
    );

    public record Service(
        [property: JsonPropertyName("project_id")] string ProjectId,
        [property: JsonPropertyName("status")] string Status,
        [property: JsonPropertyName("hour_types")] IReadOnlyList<HourType> HourTypes,
        [property: JsonPropertyName("created_at")] string CreatedAt,
        [property: JsonPropertyName("updated_at")] string UpdatedAt,
        [property: JsonPropertyName("id")] string ServiceId,
        [property: JsonPropertyName("budget")] string Budget,
        [property: JsonPropertyName("default_service_id")] string DefaultServiceId,
        [property: JsonPropertyName("name")] string Name,
        [property: JsonPropertyName("invoice_method")] string InvoiceMethod,
        [property: JsonPropertyName("amount")] string Amount,
        [property: JsonPropertyName("price")] string Price,
        [property: JsonPropertyName("track_hours")] bool? TrackHours,
        [property: JsonPropertyName("track_cost")] bool? TrackCost
    );