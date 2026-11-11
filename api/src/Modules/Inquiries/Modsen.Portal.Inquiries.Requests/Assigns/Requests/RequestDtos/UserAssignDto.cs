namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

internal sealed record UserAssignDto(
    int UserId,
    string? UserFullName,
    string? Department,
    string? Position,
    string? AvatarFileId,
    string? UserFirstName,
    string? UserLastName,
    DateOnly DateFrom,
    DateOnly? DateTo);


