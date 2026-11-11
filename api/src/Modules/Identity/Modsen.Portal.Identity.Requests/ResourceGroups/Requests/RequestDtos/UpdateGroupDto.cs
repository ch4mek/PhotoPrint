namespace Modsen.Portal.Identity.Requests.ResourceGroups.Requests.RequestDtos;

internal sealed record UpdateGroupDto(int GroupId, int? GroupManagerId, string GroupName);
