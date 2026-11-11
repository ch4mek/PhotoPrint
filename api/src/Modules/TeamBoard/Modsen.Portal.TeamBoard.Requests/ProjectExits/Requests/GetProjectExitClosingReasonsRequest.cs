using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;

internal sealed class GetProjectExitClosingReasonsRequest
    : IQueryHandler<GetProjectExitClosingReasonsRequest.Query, IReadOnlyCollection<ProjectExitClosingReasonDto>>
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetProjectExitClosingReasonsRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyCollection<ProjectExitClosingReasonDto>> Handle(
        Query request, CancellationToken cancellationToken)
    {
        return await _dbContext.ProjectExitClosingReasons
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new ProjectExitClosingReasonDto(x.Id, x.Name))
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query : IQuery<IReadOnlyCollection<ProjectExitClosingReasonDto>>;
}

public record ProjectExitClosingReasonDto(int Id, string Name);
