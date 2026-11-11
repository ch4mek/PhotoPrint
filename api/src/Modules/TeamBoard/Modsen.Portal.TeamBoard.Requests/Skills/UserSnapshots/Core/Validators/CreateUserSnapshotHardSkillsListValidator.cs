using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.Validators;

internal interface ICreateUserSnapshotHardSkillsListValidator
{
    Task ValidateAsync(CreateUserSpecializationDto specialization, CancellationToken cancellationToken);
}

internal class CreateUserSnapshotHardSkillsListValidator : ICreateUserSnapshotHardSkillsListValidator
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAreAllHardSkillsListedBusinessRule _areAllHardSkillsListedBusinessRule;
    private readonly IHardSkillsExistBusinessRule _hardSkillsExistBusinessRule;

    public CreateUserSnapshotHardSkillsListValidator(ITeamBoardDbContext teamBoardDbContext,
        IAreAllHardSkillsListedBusinessRule areAllHardSkillsListedBusinessRule,
        IHardSkillsExistBusinessRule hardSkillsExistBusinessRule)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _areAllHardSkillsListedBusinessRule = areAllHardSkillsListedBusinessRule;
        _hardSkillsExistBusinessRule = hardSkillsExistBusinessRule;
    }

    public async Task ValidateAsync(CreateUserSpecializationDto specialization, CancellationToken cancellationToken)
    {
        if (!SeniorityLevel.IsSeniorityLevelSigned(specialization.SpecializationSeniorityLevelId, checkForNoneSeniorityLevel: true))
        {
            throw new InvalidDataAppException("Specialization seniority level without sign can't be used");
        }

        var areAllHardSkillsListed = await _areAllHardSkillsListedBusinessRule.IsCorrespondsToBusiness(
                specialization.SpecializationTemplateId, specialization.HardSkills, cancellationToken);

        if (!areAllHardSkillsListed)
        {
            throw new EntityConflictException("Not all hard skills are listed in request");
        }

        var isSeniorityLevelExists = await _teamBoardDbContext.SeniorityLevels
            .Where(seniority => seniority.Id == specialization.SpecializationSeniorityLevelId)
            .AnyAsync(cancellationToken);

        if (!isSeniorityLevelExists)
        {
            throw new EntityNotFoundException($"Seniority with id {specialization.SpecializationSeniorityLevelId} does not exist");
        }

        await _hardSkillsExistBusinessRule.IsCorrespondsToBusiness(specialization.HardSkills, cancellationToken);
    }
}
