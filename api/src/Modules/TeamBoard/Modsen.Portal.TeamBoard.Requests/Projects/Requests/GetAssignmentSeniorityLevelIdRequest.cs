using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Projects.Requests;

internal sealed class GetAssignmentSeniorityLevelIdRequest
    : IQueryHandler<GetAssignmentSeniorityLevelIdRequest.Query, SeniorityLevelOutput>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetAssignmentSeniorityLevelIdRequest(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<SeniorityLevelOutput> Handle(Query request, CancellationToken cancellationToken)
    {
        var result = await _teamBoardDbContext.ProjectUsers
            .Where(projectUser => projectUser.Id == request.Id)
            .Include(projectUser => projectUser.UserOnProjectSeniorityLevel)
            .Select(projectUser => new SeniorityLevelOutput
            {
                Id = projectUser.UserOnProjectSeniorityLevel!.Id,
                Name = projectUser.UserOnProjectSeniorityLevel.InternalName
            })
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectAssignNotFound);

        return result;
    }

    internal sealed record Query(int Id) : IQuery<SeniorityLevelOutput>;
}
