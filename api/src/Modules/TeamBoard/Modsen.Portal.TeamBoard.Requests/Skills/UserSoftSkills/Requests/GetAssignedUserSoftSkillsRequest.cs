using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;

internal sealed class GetAssignedUserSoftSkillsRequest : IQueryHandler<GetAssignedUserSoftSkillsRequest.Query, List<AssignedUserSoftSkillDto>>
{
    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isUserExistByUserIdDbQuery;
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetAssignedUserSoftSkillsRequest(IIsTeamBoardUserExistWithAccessControlDbQuery isUserExistByUserIdDbQuery,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _isUserExistByUserIdDbQuery = isUserExistByUserIdDbQuery;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<AssignedUserSoftSkillDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var isTeamBoardUserExisting = await _isUserExistByUserIdDbQuery
        .ExecuteAsync(request.UserId, cancellationToken: cancellationToken);

        if (!isTeamBoardUserExisting)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var result = await GetUserSoftSkillsAsync(request.UserId, request.SnapshotId, cancellationToken);

        return result;
    }

    private Task<List<AssignedUserSoftSkillDto>> GetUserSoftSkillsAsync(int userId, int? snapshotId,
        CancellationToken cancellationToken)
    {
        return _teamBoardDbContext.SoftSkills
            .AsNoTracking()
            .Where(skill => skill.IsActive)
            .Select(softSkill => new
                {
                    SoftSkill = softSkill,
                    LatestSnapshot = _teamBoardDbContext.UserSoftSkillsSnapshots
                        .Where(snapshot => snapshot.UserId == userId)
                        .Where(snapshot => snapshot.SoftSkills.Any(us => us.SoftSkillId == softSkill.Id))
                        .Where(snapshot => snapshotId == null || snapshot.Id == snapshotId)
                        .OrderByDescending(snapshot => snapshot.SnapshotDate)
                        .Select(snapshot => new
                            {
                                Snapshot = snapshot,
                                SoftSkillEntry = snapshot.SoftSkills
                                    .Where(us => us.SoftSkillId == softSkill.Id)
                                    .OrderByDescending(us => snapshot.SnapshotDate)
                                    .FirstOrDefault()
                            })
                        .FirstOrDefault()
                })
            .OrderByDescending(x => x.SoftSkill.Id)
            .Select(x => new AssignedUserSoftSkillDto
                {
                    SoftSkillId = x.SoftSkill.Id,
                    SoftSkillName = x.SoftSkill.Name,
                    SeniorityLevelId = x.LatestSnapshot == null
                        ? null
                        : x.LatestSnapshot!.SoftSkillEntry!.SeniorityLevelId,
                    SeniorityLevelName = x.LatestSnapshot == null
                        ? null
                        : x.LatestSnapshot!.SoftSkillEntry!.SeniorityLevel!.ExternalName,
                    SoftSkillSeniorityOrderWeight = x.LatestSnapshot == null
                        ? null
                        : x.LatestSnapshot!.SoftSkillEntry!.SeniorityLevel!.OrderWeight,
                    SnapshotDate = x.LatestSnapshot == null
                        ? null
                        : x.LatestSnapshot.Snapshot.SnapshotDate
                })
            .ToListAsync(cancellationToken);
    }

    internal sealed record Query(int UserId, int? SnapshotId) : IQuery<List<AssignedUserSoftSkillDto>>;
}
