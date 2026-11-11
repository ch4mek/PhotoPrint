using FluentValidation;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class UpdateHardSkillRequest : ICommandHandler<UpdateHardSkillRequest.Command, HardSkillDto>
{
    private readonly IHardSkillUpdateValidator _hardSkillValidator;
    private readonly IGetHardSkillDbQuery _getHardSkillDbQuery;
    private readonly ITeamBoardDbContext _dbContext;

    public UpdateHardSkillRequest(IGetHardSkillDbQuery getHardSkillDbQuery,
        ITeamBoardDbContext dbContext,
        IHardSkillUpdateValidator hardSkillValidator)
    {
        _getHardSkillDbQuery = getHardSkillDbQuery;
        _hardSkillValidator = hardSkillValidator;
        _dbContext = dbContext;

    }

    public async Task<HardSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var hardSkill = await _getHardSkillDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException();

        await _hardSkillValidator.ValidateAsync(id: request.Id, name: request.Name, groupId: request.HardSkillGroupId,
            cancellationToken);

        var areFieldsChanged = hardSkill.Name != request.Name ||
            hardSkill.HardSkillGroupId != request.HardSkillGroupId;

        if (areFieldsChanged)
        {
            hardSkill.HardSkillGroupId = request.HardSkillGroupId;
            hardSkill.Name = request.Name;

            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        return new HardSkillDto
        {
            Id = hardSkill.Id,
            Name = hardSkill.Name,
            HardSkillGroupId = hardSkill.HardSkillGroupId
        };
    }

    internal sealed record Command(int Id, string Name, string HardSkillGroupId) : ICommand<HardSkillDto>,
        ITransactionalRequest;


    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Name)
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars()
                .WithMessage("Name");

            RuleFor(x => x.HardSkillGroupId)
                .NotEmpty()
                .NotNull();
        }
    }
}
