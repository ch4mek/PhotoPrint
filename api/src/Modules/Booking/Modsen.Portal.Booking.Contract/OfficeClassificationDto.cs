using Modsen.Portal.Core;

namespace Modsen.Portal.Booking.Contract;

public record OfficeClassificationDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? TimeZone { get; init; }
    public string? TimeZoneDisplayName { get; init; }
    public LocationType Location { get; init; }
    public IReadOnlyCollection<OfficeWorkspaceClassificationDto> Workspaces { get; init; } = [];
    public IReadOnlyCollection<MeetingRoomClassificationDto> MeetingRooms { get; init; } = [];
}

public record OfficeWorkspaceClassificationDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string InternalCode { get; init; }
}

public record MeetingRoomClassificationDto
{
    public int Id { get; init; }
    public required string Name { get; init; }
}
