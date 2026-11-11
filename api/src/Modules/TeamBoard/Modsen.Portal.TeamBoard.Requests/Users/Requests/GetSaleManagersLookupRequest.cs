using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.TeamBoardPortalSettings;
using Modsen.Portal.TeamBoard.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Users.Requests;

internal sealed class
    GetSaleManagersLookupRequest : IQueryHandler<GetSaleManagersLookupRequest.Query, List<TeamBoardUserRequestDto>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly ITeamBoardPortalSettingsAccessor _teamBoardPortalSettingsAccessor;

    public GetSaleManagersLookupRequest(ITeamBoardDbContext teamBoardDbContext,
        ITeamBoardPortalSettingsAccessor teamBoardPortalSettingsAccessor)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _teamBoardPortalSettingsAccessor = teamBoardPortalSettingsAccessor;
    }

    public async Task<List<TeamBoardUserRequestDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var settings = await _teamBoardPortalSettingsAccessor.GetSettings(cancellationToken);

        var salesDepartmentId = settings.SalesDepartmentId;

        var query = _teamBoardDbContext.TeamBoardUsers
            .AsNoTracking();

        if (request.IncludeTopManagement)
        {
            query = query.Where(x => x.DepartmentId == salesDepartmentId ||
                x.Position == PortalRoleNames.Ceo || x.Position == PortalRoleNames.Cto);
        }
        else
        {
            query = query.Where(x => x.DepartmentId == salesDepartmentId);
        }

        if (request.HasInterviews == true)
        {
            query = query.Where(x => x.ManagedUserInterviews.Any());
        }

        if (request.OnlyActive == true)
        {
            query = query.Where(x => x.IsActive);
        }

        return await query.Select(x => new TeamBoardUserRequestDto
            {
                Id = x.UserId,
                FirstName = x.FirstName,
                LastName = x.LastName,
                FirstNameRu = x.FirstNameRu,
                LastNameRu = x.LastNameRu
            }).OrderBy(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    internal sealed class Query : IQuery<List<TeamBoardUserRequestDto>>
    {
        public bool? HasInterviews { get; set; }
        public bool? OnlyActive { get; set; }
        public bool IncludeTopManagement { get; set; }
    }
}
