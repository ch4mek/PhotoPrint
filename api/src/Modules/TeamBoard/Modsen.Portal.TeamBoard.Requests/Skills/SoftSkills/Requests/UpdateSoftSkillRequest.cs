using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class UpdateSoftSkillRequest : ICommandHandler<UpdateSoftSkillRequest.Command, SoftSkillDto>
{
    private readonly IIsSoftSkillNameUniqueBusinessRule _isSoftSkillNameUniqueBusinessRule;
    private readonly IGetSoftSkillDbQuery _getSoftSkillDbQuery;
    private readonly ITeamBoardDbContext _dbContext;

    public UpdateSoftSkillRequest(IIsSoftSkillNameUniqueBusinessRule isSoftSkillNameUniqueBusinessRule,
        IGetSoftSkillDbQuery getSoftSkillDbQuery,
        ITeamBoardDbContext dbContext)
    {
        _isSoftSkillNameUniqueBusinessRule = isSoftSkillNameUniqueBusinessRule;
        _getSoftSkillDbQuery = getSoftSkillDbQuery;
        _dbContext = dbContext;

    }

    public async Task<SoftSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var softSkill = await _getSoftSkillDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException();

        var isNameUnique = await _isSoftSkillNameUniqueBusinessRule.IsCorrespondsToBusiness(softSkill.Id, request.Name,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new EntityConflictException(PortalMessages.SoftSkillNameAlreadyOccupied);
        }

        if (softSkill.Name != request.Name)
        {
            softSkill.Name = request.Name;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new SoftSkillDto { Id = softSkill.Id, Name = softSkill.Name };
    }

    internal sealed record Command(int Id, string Name) : ICommand<SoftSkillDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars()
                .WithMessage("Name");
        }
    }
}
