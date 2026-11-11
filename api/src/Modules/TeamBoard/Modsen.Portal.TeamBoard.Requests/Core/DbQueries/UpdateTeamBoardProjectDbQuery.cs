using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;

namespace Modsen.Portal.TeamBoard.Requests.Core.DbQueries;

internal interface IUpdateTeamBoardProjectDbQuery
{
    Task ExecuteAsync(UpdateTeamBoardProjectDbRequest request, CancellationToken cancellationToken);
}

internal record UpdateTeamBoardProjectDbRequest(
    int Id, bool IsActive, string ProjectName, string ClientName, int? UpdatedById, bool IsPublic, string? Color);

internal class UpdateTeamBoardProjectDbQuery : IUpdateTeamBoardProjectDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IClockService _clockService;

    public UpdateTeamBoardProjectDbQuery(ITeamBoardDbContext teamBoardDbContext, IClockService clockService)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _clockService = clockService;
    }

    public async Task ExecuteAsync(UpdateTeamBoardProjectDbRequest request, CancellationToken cancellationToken)
    {
        var project = await _teamBoardDbContext.TeamBoardProjects
                .FirstOrDefaultAsync(teamBoardProject => teamBoardProject.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.ProjectNotFound);

        project.ProjectName = request.ProjectName;
        project.ClientName = request.ClientName;
        project.UpdatedAtUtc = _clockService.UtcNow;
        project.UpdatedById = request.UpdatedById;
        project.IsActive = request.IsActive;
        project.IsPublic = request.IsPublic;

        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
    }
}
