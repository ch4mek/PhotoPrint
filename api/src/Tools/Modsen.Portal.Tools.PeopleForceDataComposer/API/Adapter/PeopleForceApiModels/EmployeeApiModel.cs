using System.Text.Json.Serialization;

namespace Modsen.Portal.Tools.PeopleForceDataComposer.API.Adapter.PeopleForceApiModels;

internal sealed class EmployeeApiModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("middle_name")]
    public string? MiddleName { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("personal_email")]
    public string? PersonalEmail { get; set; }

    [JsonPropertyName("mobile_number")]
    public string? MobileNumber { get; set; }

    [JsonPropertyName("work_phone_number")]
    public string? WorkPhoneNumber { get; set; }

    [JsonPropertyName("date_of_birth")]
    public DateOnly? DateOfBirth { get; set; }

    [JsonPropertyName("hired_on")]
    public DateOnly? HiredOn { get; set; }

    [JsonPropertyName("department")]
    public DepartmentApiModel? Department { get; set; }

    [JsonPropertyName("position")]
    public PositionApiModel? Position { get; set; }

    [JsonPropertyName("active")]
    public bool IsActive { get; set; }

    [JsonPropertyName("reporting_to")]
    public ReportingToApiModel? ReportingTo { get; set; }
}

public class ReportingToApiModel
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }
}
