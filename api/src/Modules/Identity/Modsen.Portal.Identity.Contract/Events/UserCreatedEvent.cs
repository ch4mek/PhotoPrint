using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.TeamBoard;

namespace Modsen.Portal.Identity.Contract.Events;

public record UserCreatedEvent : IIntegrationEvent
{
    public required string EventId { get; init; }
    public required int UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string? FirstNameRu { get; init; }
    public string? LastNameRu { get; init; }
    public string? PatronymicRu { get; init; }
    public required string Email { get; init; }
    public string? DepartmentId { get; init; }
    public int? ResourceGroupId { get; init; }
    public string? Color { get; init; }
    public string? AvatarFileId { get; init; }
    public string? Position { get; init; }
    public LocationType? Location { get; set; }
    public int PositionOrderWeight { get; init; }
    public int? CountryId { get; init; }
    public int RegisteredLocationId { get; init; }
    public string? CurrentLocation { get; init; }
    public DateOnly? HireDate { get; init; }
    public MilitaryServiceStatus? MilitaryServiceStatus { get; init; }
    public bool? IsGraduateJobPlacement { get; init; }
    public DateOnly? ContractDateFrom { get; init; }
    public DateOnly? ContractDateTo { get; init; }

    public string? PersonalEmail { get; set; }
    public string? PersonalEmailSecond { get; set; }
    public string? PersonalPhone { get; set; }
    public string? Telegram { get; set; }
    public string? LinkedIn { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public int? CreatedById { get; set; }
}

internal sealed class UserCreatedExchangeDefinition : ExchangeDefinitionBase<UserCreatedEvent>
{
    public override string ExchangeName => "user-created-exchange";
}
