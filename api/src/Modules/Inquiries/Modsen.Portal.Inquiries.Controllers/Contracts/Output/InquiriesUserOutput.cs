using Modsen.Portal.Inquiries.Requests.Dto;

namespace Modsen.Portal.Inquiries.Controllers.Contracts.Output;

internal record InquiriesUserOutput(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string? DepartmentId,
    int? RegisteredLocationId,
    string? AvatarFieldId,
    string? Color,
    string? Position,
    bool? CanUserInfoBeRetrievedByCurrentUser)
{
    public static InquiriesUserOutput? Of(InquiriesUserDto? dto)
    {
        return dto == null
            ? null
            : new InquiriesUserOutput(
                dto.Id,
                dto.FirstName,
                dto.LastName,
                dto.Email,
                dto.DepartmentId,
                dto.RegisteredLocationId,
                dto.AvatarFileId,
                dto.Color,
                dto.Position,
                dto.CanUserInfoBeRetrievedByCurrentUser);
    }
}
