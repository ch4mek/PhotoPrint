using FluentValidation;
using MediatR;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class DeleteJiraSettingRequest : ICommandHandler<DeleteJiraSettingRequest.Command, Unit>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IGetJiraSettingDbQuery _getJiraSettingDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public DeleteJiraSettingRequest(ITimeTrackDbContext timeTrackDbContext,
        IGetJiraSettingDbQuery getJiraSettingDbQuery,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _getJiraSettingDbQuery = getJiraSettingDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
    {
        var jiraSetting = await _getJiraSettingDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.JiraSettingNotFound);

        if (_authenticatedUserContext.GetRequiredUserId() != jiraSetting.UserId)
        {
            throw new ActionNotAllowedException();
        }

        jiraSetting.IsActive = false;
        await _timeTrackDbContext.SaveChanges(cancellationToken);

        return Unit.Value;
    }

    internal sealed record Command(int Id) : ICommand<Unit>, ITransactionalRequest;

    internal sealed class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
