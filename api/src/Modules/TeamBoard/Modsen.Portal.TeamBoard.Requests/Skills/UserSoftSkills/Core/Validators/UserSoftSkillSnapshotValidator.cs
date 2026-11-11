using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;

internal interface IUserSoftSkillSnapshotValidator
{
    Task ValidateAsync(IReadOnlyCollection<UserSoftSkillSeniorityLevelDto> softSkillsSeniorityLevels, CancellationToken cancellationToken);
}

internal sealed class UserSoftSkillSnapshotValidator : IUserSoftSkillSnapshotValidator
{
    private readonly ITeamBoardDbContext _dbContext;

    public UserSoftSkillSnapshotValidator(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ValidateAsync(IReadOnlyCollection<UserSoftSkillSeniorityLevelDto> softSkillsSeniorityLevels,
        CancellationToken cancellationToken)
    {
        var skillsIdsToCheck = softSkillsSeniorityLevels.Select(level => level.SoftSkillId).ToArray();

        var existingSoftSkillsIds = await _dbContext.SoftSkills
            .Where(skill => skillsIdsToCheck.Contains(skill.Id))
            .Select(skill => skill.Id)
            .ToArrayAsync(cancellationToken);

        var areAllSoftSkillsListed = skillsIdsToCheck.All(id => existingSoftSkillsIds.Contains(id));
        if (!areAllSoftSkillsListed)
        {
            throw new InvalidDataAppException("Some soft skills not found");
        }

        var seniorityLevelsToCheck = softSkillsSeniorityLevels.Select(level => level.SeniorityLevelId).ToArray();

        var existingSeniorityLevelsIds = await _dbContext.SeniorityLevels
            .Where(level => level.InternalName == level.Id && seniorityLevelsToCheck.Contains(level.Id))
            .Select(level => level.Id)
            .ToArrayAsync(cancellationToken);

        var areAllSeniorityLevelsValid = seniorityLevelsToCheck.All(id => existingSeniorityLevelsIds.Contains(id));
        if (!areAllSeniorityLevelsValid)
        {
            throw new InvalidDataAppException("Some soft skill seniorities not found");
        }
    }
}
