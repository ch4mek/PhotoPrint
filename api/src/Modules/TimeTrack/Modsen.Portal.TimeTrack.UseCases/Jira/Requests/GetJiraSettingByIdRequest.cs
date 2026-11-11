using FluentValidation;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Dtos;
using Modsen.Portal.TimeTrack.UseCases.Jira.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class GetJiraSettingByIdRequest : IQueryHandler<GetJiraSettingByIdRequest.Query, JiraSettingDto>
{
    private readonly IGetJiraSettingDbQuery _getJiraSettingDbQuery;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetJiraSettingByIdRequest(IGetJiraSettingDbQuery getJiraSettingDbQuery,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _getJiraSettingDbQuery = getJiraSettingDbQuery;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<JiraSettingDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var jiraSettings = await _getJiraSettingDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.JiraSettingNotFound);

        if (_authenticatedUserContext.GetRequiredUserId() != jiraSettings.UserId)
        {
            throw new ActionNotAllowedException();
        }

        return JiraSettingDto.Of(jiraSettings);
    }

    internal sealed record Query(int Id) : IQuery<JiraSettingDto>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }
}
