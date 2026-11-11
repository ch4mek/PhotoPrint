using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Jira.Requests;

internal sealed class GetJiraSettingsRequest : IQueryHandler<GetJiraSettingsRequest.Query, PageableListOfItems<JiraSettingDto>>
{
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetJiraSettingsRequest(ITimeTrackDbContext timeTrackDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<JiraSettingDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var baseQuery = _timeTrackDbContext.JiraSettings
            .Where(x => x.IsActive)
            .Where(x => x.UserId == _authenticatedUserContext.GetRequiredUserId())
            .AsNoTracking();

        var query = baseQuery
            .Select(x => new JiraSettingDto
            {
                Id = x.Id,
                Url = x.Url,
                JiraProjectTag = x.JiraProjectTag,
                ProjectId = x.ProjectId,
                UserId = x.UserId,
                Email = x.Email,
                ProjectName = x.Project != null ? x.Project.Name : null
            });

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<JiraSettingDto>()
            .ToListAsync(cancellationToken);

        var queryOptionsWithoutPagination = request.QueryOptions
            .RebuildWithoutPagination();

        var total = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<JiraSettingDto>()
            .CountAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<JiraSettingDto> QueryOptions) : IQuery<PageableListOfItems<JiraSettingDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
