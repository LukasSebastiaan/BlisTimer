using System.Text.Json.Serialization;

namespace SimplicateAPI.Enitities;

public record ProjectEmployee(
    [property: JsonPropertyName("id")] string ProjectEmployeeId,
    [property: JsonPropertyName("employee_id")] string EmployeeId,
    [property: JsonPropertyName("name")] string Name
);

public record Organization(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name
);

public record ProjectStatus(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("label")] string Label
);

public record Project(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("project_status")] ProjectStatus ProjectStatus,
    [property: JsonPropertyName("hours_rate_type")] string HoursRateType,
    [property: JsonPropertyName("organization")] Organization Organization,
    [property: JsonPropertyName("separate_invoice_recipient")] SeparateInvoiceRecipient SeparateInvoiceRecipient,
    [property: JsonPropertyName("employees")] IReadOnlyList<ProjectEmployee> Employees,
    [property: JsonPropertyName("timeline_email_address")] string TimelineEmailAddress,
    [property: JsonPropertyName("created")] string Created,
    [property: JsonPropertyName("modified")] string Modified,
    [property: JsonPropertyName("created_at")] string CreatedAt,
    [property: JsonPropertyName("updated_at")] string UpdatedAt,
    [property: JsonPropertyName("simplicate_url")] string SimplicateUrl,
    [property: JsonPropertyName("is_reverse_billing")] bool? IsReverseBilling,
    [property: JsonPropertyName("is_invoice_approval")] bool? IsInvoiceApproval,
    [property: JsonPropertyName("my_organization_profile_id")] string MyOrganizationProfileId,
    [property: JsonPropertyName("organization_id")] string OrganizationId,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("billable")] bool? Billable,
    [property: JsonPropertyName("can_register_mileage")] bool? CanRegisterMileage,
    [property: JsonPropertyName("project_number")] string ProjectNumber,
    [property: JsonPropertyName("start_date")] string StartDate
);

public record SeparateInvoiceRecipient(
    [property: JsonPropertyName("is_separate_invoice_recipient")]
    bool? IsSeparateInvoiceRecipient
);

public record Total(
    [property: JsonPropertyName("value_budget")]
    int? ValueBudget,
    [property: JsonPropertyName("value_spent")]
    double? ValueSpent,
    [property: JsonPropertyName("value_invoiced")]
    int? ValueInvoiced
);