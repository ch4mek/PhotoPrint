using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;

internal interface IHardSkillUpdateValidator
{
    Task ValidateAsync(int? id, string name, string groupId, CancellationToken cancellationToken);
}

internal sealed class HardSkillUpdateValidator : IHardSkillUpdateValidator
{
    private readonly IIsHardSkillNameUniqueBusinessRule _isHardSkillNameUniqueBusinessRule;
    private readonly IHardSkillGroupExistsBusinessRule _hardSkillGroupExistsBusinessRule;

    public HardSkillUpdateValidator(IIsHardSkillNameUniqueBusinessRule isHardSkillNameUniqueBusinessRule,
        IHardSkillGroupExistsBusinessRule hardSkillGroupExistsBusinessRule)
    {
        _isHardSkillNameUniqueBusinessRule = isHardSkillNameUniqueBusinessRule;
        _hardSkillGroupExistsBusinessRule = hardSkillGroupExistsBusinessRule;
    }

    public async Task ValidateAsync(int? id, string name, string groupId, CancellationToken cancellationToken)
    {
        var isNewNameUnique = await _isHardSkillNameUniqueBusinessRule.IsCorrespondsToBusiness(id, name,
            cancellationToken);

        if (!isNewNameUnique)
        {
            throw new InvalidDataAppException("Hard skill with the same name already exists");
        }

        var isHardSkillGroupExisting = await _hardSkillGroupExistsBusinessRule.IsCorrespondsToBusiness(groupId,
            cancellationToken);

        if (!isHardSkillGroupExisting)
        {
            throw new InvalidDataAppException("There is no hard skill group with such id");
        }
    }
}
