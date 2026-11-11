using FluentValidation;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class CreateHardSkillRequest : ICommandHandler<CreateHardSkillRequest.Command, HardSkillDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IHardSkillUpdateValidator _hardSkillUpdateValidator;
    private readonly ITeamBoardDbContext _dbContext;
    private readonly IClockService _clockService;

    public CreateHardSkillRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext dbContext,
        IClockService clockService,
        IHardSkillUpdateValidator hardSkillUpdateValidator)
    {

        _authenticatedUserContext = authenticatedUserContext;
        _dbContext = dbContext;
        _clockService = clockService;
        _hardSkillUpdateValidator = hardSkillUpdateValidator;

    }

    public async Task<HardSkillDto> Handle(Command request, CancellationToken cancellationToken)
    {
        await _hardSkillUpdateValidator.ValidateAsync(id: null, name: request.Name, groupId: request.HardSkillGroupId,
            cancellationToken);

        var hardSkillEntity = new HardSkill
        {
            Name = request.Name,
            IsActive = true,
            HardSkillGroupId = request.HardSkillGroupId,
            CreatedAtUtc = _clockService.UtcNow,
            CreatedById = _authenticatedUserContext.UserId
        };

        _dbContext.HardSkills.Add(hardSkillEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new HardSkillDto
        {
            Id = hardSkillEntity.Id,
            Name = hardSkillEntity.Name,
            HardSkillGroupId = hardSkillEntity.HardSkillGroupId
        };
    }

    internal sealed record Command(string Name, string HardSkillGroupId)
        : ICommand<HardSkillDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .SpacesOrPunctuationSymbolsRule()
                .AlphanumericWithSpecialChars()
                .WithMessage("Name");

            RuleFor(x => x.HardSkillGroupId)
                .NotEmpty()
                .NotNull();
        }
    }
}
