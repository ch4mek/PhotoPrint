using Modsen.Portal.TeamBoard.DataAccess.Npgsql.Cache;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.Resolvers;

internal interface ISpecializationSeniorityResolver
{
    Task<string> ResolveAsync(HardSkillSeniorityDto[] hardSkillSeniorities, CancellationToken cancellationToken);
}

internal class SpecializationSeniorityResolver : ISpecializationSeniorityResolver
{
    private readonly ISeniorityLevelCache _seniorityLevelCache;

    public SpecializationSeniorityResolver(ISeniorityLevelCache seniorityLevelCache)
    {
        _seniorityLevelCache = seniorityLevelCache;
    }

    public async Task<string> ResolveAsync(HardSkillSeniorityDto[] hardSkillSeniorities, CancellationToken cancellationToken)
    {
        if (hardSkillSeniorities == null || hardSkillSeniorities.Length == 0)
        {
            throw new InvalidDataException("Specialization without hard skills cannot be assigned.");
        }

        var seniorityLevels = (await _seniorityLevelCache.GetAllSeniorityLevels(cancellationToken)).ToList();

        var average = Math.Round(hardSkillSeniorities
            .SelectMany(hardSkill => seniorityLevels
                .Where(level => level.Id == hardSkill.SeniorityLevelId)
                .Select(level => level.Coefficient ?? 0))
            .Average(), 2);

        seniorityLevels.RemoveAll(x => Math.Abs(x.SpecializationRangeTo ?? 0) < double.Epsilon);
        seniorityLevels = seniorityLevels.OrderBy(x => x.SpecializationRangeTo).ToList();
        var specializationSeniority = seniorityLevels
            .FirstOrDefault(x => average <= x.SpecializationRangeTo);

        return specializationSeniority?.Id ?? "None";
    }
}
