namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

internal sealed record GroupLookupDto(
    int Id,
    string Name,
    string DepartmentId);
