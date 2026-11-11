using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication;
using Modsen.Portal.Core.Communication.ExchangeDefinition;

namespace Modsen.Portal.Identity.Contract.Events;

public record UserUpdatedEvent : IIntegrationEvent
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
    public string? AvatarColor { get; init; }
    public string? Position { get; init; }
    public LocationType? Location { get; set; }
    public int PositionOrderWeight { get; init; }
    public int? InitiatorId { get; init; }
    public int? CountryId { get; init; }
    public int? RegisteredLocationId { get; init; }
    public string? CurrentLocation { get; init; }
    public DateOnly? HireDate { get; init; }
}

internal sealed class UserUpdatedExchangeDefinition : ExchangeDefinitionBase<UserUpdatedEvent>
{
    public override string ExchangeName => "user-updated-exchange";
}
