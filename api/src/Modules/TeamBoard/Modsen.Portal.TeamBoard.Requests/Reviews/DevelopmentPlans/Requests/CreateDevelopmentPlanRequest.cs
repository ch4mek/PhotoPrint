using FluentValidation;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests;

internal sealed class CreateDevelopmentPlanRequest : ICommandHandler<CreateDevelopmentPlanRequest.Command, DevelopmentPlanDto>{

    private readonly IIsTeamBoardUserExistWithAccessControlDbQuery _isTeamBoardUserExistingWithAccessControlDbQuery;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    public CreateDevelopmentPlanRequest(
        IIsTeamBoardUserExistWithAccessControlDbQuery isTeamBoardUserExistingWithAccessControlDbQuery,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule,
        ITeamBoardDbContext teamBoardDbContext)
    {
        _isTeamBoardUserExistingWithAccessControlDbQuery = isTeamBoardUserExistingWithAccessControlDbQuery;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<DevelopmentPlanDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!await _isTeamBoardUserExistingWithAccessControlDbQuery.ExecuteAsync(request.UserId, cancellationToken: cancellationToken))
        {
            throw new EntityNotFoundException($"There is no user with Id {request.UserId}");
        }

        var canBeRetrieved = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusiness(request.UserId,
            Portal.Core.AccessOptions.Manager,
            cancellationToken);
        if (!canBeRetrieved)
        {
            throw new ActionNotAllowedException();
        }

        var source = request.Source;
        if (!request.Source.HasValue)
        {
            source = DevelopmentPlanSource.Unknown;
        }

        var developmentPlan = await CreateDevelopmentPlanAsync(request, source, cancellationToken);

        return new DevelopmentPlanDto(developmentPlan.Id, developmentPlan.StartDate, developmentPlan.DeadlineDate,
            developmentPlan.EndDate, developmentPlan.Summary, developmentPlan.Notes, developmentPlan.UserId);
    }

    private async Task<DataAccess.Npgsql.DatabaseEntities.DevelopmentPlan> CreateDevelopmentPlanAsync(
        Command request,
        DevelopmentPlanSource? source,
        CancellationToken cancellationToken)
    {
        var developmentPlan = new DataAccess.Npgsql.DatabaseEntities.DevelopmentPlan
        {
            StartDate = request.StartDate,
            DeadlineDate = request.DeadlineDate,
            Summary = request.Summary,
            Notes = request.Notes,
            UserId = request.UserId,
            Source = source ?? DevelopmentPlanSource.Unknown,
            ConversationId = request.ConversationId
        };

        await _teamBoardDbContext.DevelopmentPlans.AddAsync(developmentPlan, cancellationToken);
        await _teamBoardDbContext.SaveChangesAsync(cancellationToken);
        return developmentPlan;
    }

    internal sealed record Command(DateOnly StartDate, DateOnly? DeadlineDate, string Summary,
        string? Notes, int UserId, DevelopmentPlanSource? Source, int? ConversationId) : ICommand<DevelopmentPlanDto>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.StartDate).NotNull();
            When(x => x.DeadlineDate is not null, () => RuleFor(x => x.DeadlineDate)
                .GreaterThan(x => x.StartDate));
            RuleFor(x => x.Summary).NotNull().NotEmpty()
                .LongTextType()
                .SpacesOrPunctuationSymbolsRule();
            RuleFor(x => x.Notes)
                .BeEmptyNullOrBeLongTextType()
                .SpacesOrPunctuationSymbolsRule();
        }
    }
}
