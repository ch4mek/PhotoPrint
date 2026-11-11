using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Services.PortalSettings;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Home.Requests.RequestsDto;

namespace Modsen.Portal.TeamBoard.Requests.Home.Requests;

internal sealed class GetProbationPeriodEndRequest : IQueryHandler<GetProbationPeriodEndRequest.Query, List<ProbationPeriodEndDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IPortalSettingsAccessor _portalSettingsAccessor;

    public GetProbationPeriodEndRequest(
        ITeamBoardDbContext teamBoardDbContext,
        IClockService clockService,
        IAuthenticatedUserContext authenticatedUserContext,
        IPortalSettingsAccessor portalSettingsAccessor)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
        _authenticatedUserContext = authenticatedUserContext;
        _portalSettingsAccessor = portalSettingsAccessor;
    }

    public async Task<List<ProbationPeriodEndDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var today = _clockService.TodayUtc();
        var lastDay = today.AddDays(await GetDays(cancellationToken));

        var query = ApplyAuthenticationFilter(_teamBoardDbContext.TeamBoardUsers);

        if (request.DepartmentId != null)
        {
            query = query.Where(x => x.DepartmentId == request.DepartmentId);
        }

        var probationPeriodEndDates = await query
            .Where(user => user.Position != PortalRoleNames.Intern)
            .Where(x => x.IsActive)
            .Where(x => x.HireDate.HasValue)
            .Select(x => new ProbationPeriodEndDto
            {
                UserId = x.UserId,
                UserFullName = x.LastNameRu == null && x.FirstNameRu == null
                    ? x.LastName + " " + x.FirstName
                    : x.LastNameRu != null && x.FirstNameRu != null
                        ? x.LastNameRu + " " + x.FirstNameRu + " (" + x.LastName + " " + x.FirstName + ")"
                        : (x.LastNameRu ?? x.FirstNameRu) + " (" + x.LastName + " " + x.FirstName + ")",
                Date = x.HireDate!.Value.AddMonths(PortalSharedConstants.ProbationPeriodInMonth).AddDays(-1),
                UserPosition = x.Position,
            })
            .Where(x => x.Date >= today && x.Date <= lastDay)
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return probationPeriodEndDates;
    }

    private async Task<int> GetDays(CancellationToken cancellationToken)
    {
        var daysString = await _portalSettingsAccessor.GetStringValueByName(
            PortalSettingNames.EmployeesProbationPeriodEndDaysSpan, cancellationToken);

        if (int.TryParse(daysString, out var days))
        {
            return days;
        }

        throw new ArgumentException(message: "Invalid employees probation period end days span value");
    }

    public IQueryable<TeamBoardUser> ApplyAuthenticationFilter(IQueryable<TeamBoardUser> users)
    {
        var role = _authenticatedUserContext.GetRoles();

        if (role.IsCeo() || role.IsCto())
        {
            return users;
        }

        if (role.IsHeadOfDepartment())
        {
            return users.Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (role.IsGroupManager())
        {
            return users.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        return users.Where(x => false);
    }

    internal sealed record Query(string? DepartmentId) : IQuery<List<ProbationPeriodEndDto>>;
}
