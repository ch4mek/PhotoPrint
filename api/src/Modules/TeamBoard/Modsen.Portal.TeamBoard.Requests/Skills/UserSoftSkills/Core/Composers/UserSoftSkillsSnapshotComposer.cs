using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Composers;

internal interface IUserSoftSkillsSnapshotComposer
{
    Task<int> ComposeAsync(UserSoftSkillSnapshotCoreDto snapshotCoreDto, CancellationToken cancellationToken);
}

internal sealed class UserSoftSkillsSnapshotComposer : IUserSoftSkillsSnapshotComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IClockService _clockService;

    public UserSoftSkillsSnapshotComposer(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext dbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
        _clockService = clockService;
    }

    public async Task<int> ComposeAsync(UserSoftSkillSnapshotCoreDto snapshotCoreDto, CancellationToken cancellationToken)
    {
        var dbEntity = new UserSoftSkillsSnapshot
        {
            UserId = snapshotCoreDto.UserId,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId,
            SnapshotDate = snapshotCoreDto.SnapshotDate,
        };

        foreach (var softSkillLevel in snapshotCoreDto.UserSoftSkillCoreDtos)
        {
            dbEntity.SoftSkills.Add(new UserSoftSkill
            {
                UserId = snapshotCoreDto.UserId,
                SoftSkillId = softSkillLevel.SoftSkillId,
                SeniorityLevelId = softSkillLevel.SeniorityLevelId,
                IsVisibleSeniorityLevel = true,
                IsVisibleSoftSkill = true
            });
        }

        var createdEntity = await _dbContext.UserSoftSkillsSnapshots.AddAsync(dbEntity, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return createdEntity.Entity.Id;
    }
}
