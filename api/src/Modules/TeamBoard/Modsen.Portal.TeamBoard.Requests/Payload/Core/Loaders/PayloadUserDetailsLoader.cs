using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Core.Loaders;

internal interface IPayloadUserDetailsLoader
{
    Task<UserDetailsCoreDto[]> Load(int[] userIds, int payloadYear, string? projectName, CancellationToken token);
}

internal class PayloadUserDetailsLoader : IPayloadUserDetailsLoader
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public PayloadUserDetailsLoader(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<UserDetailsCoreDto[]> Load(int[] userIds, int payloadYear, string? projectName, CancellationToken token)
    {
        if (userIds.Length == 0)
        {
            return await Task.FromResult(Array.Empty<UserDetailsCoreDto>());
        }

        var users = await _teamBoardDbContext.TeamBoardUsers
            .AsSplitQuery()
            .Where(x => userIds.Contains(x.UserId))
            .Where(x => x.CareerStartedAt != null)
            .GroupJoin(_teamBoardDbContext.TeamBoardResourceGroups,
                user => user.ResourceGroupId,
                group => group.ResourceGroupId,
                (user, groups) => new { User = user, Groups = groups })
            .SelectMany(x => x.Groups.DefaultIfEmpty(), (x, group) => new UserDetailsCoreDto
            {
                UserId = x.User.UserId,
                IsActive = x.User.IsActive,
                Comment = x.User.Comments,
                DepartmentId = x.User.DepartmentId,
                ResourceGroupId = group != null && group.IsActive ? x.User.ResourceGroupId : null,
                ResourceGroupName = group != null && group.IsActive ? group.ResourceGroupName : null,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                FirstNameRu = x.User.FirstNameRu,
                LastNameRu = x.User.LastNameRu,
                CareerStartedAt = x.User.CareerStartedAt!.Value,
                Color = x.User.AvatarColor,
                Plan = x.User.UserPayloadDetails
                    .Select(details => details.Plan)
                    .FirstOrDefault(),
                NeedProject = x.User.UserPayloadDetails
                    .Select(details => details.NeedProject)
                    .FirstOrDefault(),
                RegisteredLocations = x.User.RegisteredLocations
                    .Select(history => new RegisteredLocationHistoryCoreDto
                    {
                        RegisteredLocationId = history.RegisteredLocationId,
                        DateFrom = history.DateFrom,
                        DateTo = history.DateTo
                    }).ToArray(),
                PrimarySpecializationName = x.User.UserSpecializations
                    .Where(sp => sp.IsPrimary)
                    .Select(sp => sp.SpecializationTemplate!.Name)
                    .FirstOrDefault(),
                SpecializationSeniority = x.User.UserSpecializations
                    .Where(sp => sp.IsPrimary)
                    .Select(sp => sp.SeniorityLevel!.InternalName)
                    .FirstOrDefault(),
                ProjectDetails = x.User.UserProjects
                    .Where(projectUser => !projectUser.Project!.IsPublic &&
                        (projectUser.UserOnProjectFrom == DateOnly.FromDateTime(DateTime.MinValue)
                        || projectUser.UserOnProjectFrom.Year <= payloadYear)
                        &&
                        (projectUser.UserOnProjectTo == null
                        || projectUser.UserOnProjectTo == DateOnly.FromDateTime(DateTime.MinValue)
                        || projectUser.UserOnProjectTo.Value.Year >= payloadYear)
                        &&
                        (projectName == null || EF.Functions.ILike(projectUser.Project!.ProjectName, $"%{projectName.Trim()}%")))
                    .Select(projectUser => new UserProjectDetailsCoreDto
                    {
                        ProjectId = projectUser.ProjectId,
                        Fte = projectUser.Fte,
                        UserOnProjectFrom = projectUser.UserOnProjectFrom,
                        UserOnProjectTo = projectUser.UserOnProjectTo,
                        IsTimeOnProjectBillable = projectUser.IsTimeOnProjectBillable
                    })
                    .ToArray(),
                UserMonthPayloadDetails = x.User.EstimatedUserMonthPayloads
                    .Where(payload => payload.PayloadYear == payloadYear)
                    .Select(payload => new UserMonthPayloadDetailsCoreDto
                    {
                        PayloadMonthOfYear = payload.PayloadMonthOfYear,
                        EstimatedFte = payload.EstimatedFte
                    })
                    .ToArray(),
                ContractDates = x.User.Contracts
                    .Select(contract => new ContractDatesDto
                    {
                        ContractFrom = contract.ContractFrom,
                        ContractTo = contract.ContractTo,
                        FireDate = contract.FireDate,
                    }).ToList(),
                IsUserHasContracts = x.User.Contracts.Count > 0,
                HireDate = x.User.HireDate,
                IsHeadOfDepartment = x.User.Position == PortalRoleNames.HeadOfDepartment
            })
            .ToArrayAsync(token);

        return users;
    }
}
