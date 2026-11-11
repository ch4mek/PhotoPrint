using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;

public interface IHardSkillsExistBusinessRule
{
    Task IsCorrespondsToBusiness(
        IReadOnlyCollection<HardSkillSeniorityDto> hardSkills,
        CancellationToken cancellationToken);
}

internal class HardSkillsExistBusinessRule(ITeamBoardDbContext teamBoardDbContext) : IHardSkillsExistBusinessRule
{
    public async Task IsCorrespondsToBusiness(
        IReadOnlyCollection<HardSkillSeniorityDto> hardSkills,
        CancellationToken cancellationToken)
    {
        var seniorityLevelsIds = hardSkills
            .Where(x => !x.SeniorityLevelId.IsNullOrWhiteSpace())
            .Select(x => x.SeniorityLevelId!)
            .ToArray();

        var existingSeniorityLevelsIds = await teamBoardDbContext.SeniorityLevels
            .Where(x => seniorityLevelsIds.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSetAsync(cancellationToken);

        foreach (var seniorityLevelId in seniorityLevelsIds)
        {
            var isSeniorityLevelExists = existingSeniorityLevelsIds.Contains(seniorityLevelId);

            if (!isSeniorityLevelExists)
            {
                throw new EntityNotFoundException($"Seniority with id {seniorityLevelId} does not exist");
            }

            if (SeniorityLevel.IsSeniorityLevelSigned(seniorityLevelId, checkForNoneSeniorityLevel: false))
            {
                throw new InvalidDataAppException("Hard skill seniority level with sign can't be used for hard skill");
            }
        }
    }
}
