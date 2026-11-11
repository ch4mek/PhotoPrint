using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class CreateSoftSkillRequest : ICommandHandler<CreateSoftSkillRequest.Command, SoftSkillDto>
{
    private readonly IIsSoftSkillNameUniqueBusinessRule _isSoftSkillNameUniqueBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IClockService _clockService;

    public CreateSoftSkillRequest(IAuthenticatedUserContext authenticatedUserContext,
        IIsSoftSkillNameUniqueBusinessRule isSoftSkillNameUniqueBusinessRule,
        ITeamBoardDbContext dbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _isSoftSkillNameUniqueBusinessRule = isSoftSkillNameUniqueBusinessRule;
        _dbContext = dbContext;
        _clockService = clockService;
    }

    public async Task<SoftSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var isNameUnique = await _isSoftSkillNameUniqueBusinessRule.IsCorrespondsToBusiness(id: null, request.Name,
            cancellationToken);

        if (!isNameUnique)
        {
            throw new EntityConflictException(PortalMessages.SoftSkillNameAlreadyOccupied);
        }

        var softSkill = new SoftSkill
        {
            Name = request.Name,
            IsActive = true,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId
        };

        _dbContext.SoftSkills.Add(softSkill);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new SoftSkillDto
        {
            Id = softSkill.Id,
            Name = softSkill.Name
        };
    }

    internal sealed record Command(string Name) : ICommand<SoftSkillDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .WithMessage("Enter Skill name")
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars();
        }
    }
}
