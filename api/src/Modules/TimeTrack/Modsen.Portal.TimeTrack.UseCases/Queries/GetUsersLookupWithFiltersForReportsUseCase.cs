using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Core.DbQueries;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Queries;

internal sealed class GetUsersLookupWithFiltersForReportsUseCase : IQueryHandler<GetUsersLookupWithFiltersForReportsUseCase.Query, List<UserDto>>
{
    private readonly ITimeTrackUserProvider _userProvider;
    private readonly IProjectsProvider _projectsProvider;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ICanUserBeProjectManagerBusinessRule _canUserBeProjectManagerBusinessRule;
    private readonly IIsUserManagerOfAnyPublicProjectDbQuery _isAnyPublicProjectManagedByUserIdDbQuery;

    public GetUsersLookupWithFiltersForReportsUseCase(
        ITimeTrackUserProvider userProvider,
        IProjectsProvider projectsProvider,
        IAuthenticatedUserContext authenticatedUserContext,
        ICanUserBeProjectManagerBusinessRule canUserBeProjectManagerBusinessRule,
        IIsUserManagerOfAnyPublicProjectDbQuery isAnyPublicProjectManagedByUserIdDbQuery)
    {
        _userProvider = userProvider;
        _projectsProvider = projectsProvider;
        _authenticatedUserContext = authenticatedUserContext;
        _canUserBeProjectManagerBusinessRule = canUserBeProjectManagerBusinessRule;
        _isAnyPublicProjectManagedByUserIdDbQuery = isAnyPublicProjectManagedByUserIdDbQuery;
    }

    public async Task<List<UserDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        await ValidateRequest(request, cancellationToken);

        var isPublicProject = false;
        var isClientWithPublicProject = false;
        var isAnyPublicProjectManagedByUser = false;

        if (request.OnlyManagedUsers == true)
        {
            isAnyPublicProjectManagedByUser = await _isAnyPublicProjectManagedByUserIdDbQuery.ExecuteAsync(
                _authenticatedUserContext.UserId, cancellationToken);
        }

        if (request.ClientId is not null)
        {
            var clientProjects = await _projectsProvider.GetProjectsByClientIdAsync(
                request.ClientId.Value,
                cancellationToken);

            isClientWithPublicProject = clientProjects.Exists(x => x.IsPublic &&
                (request.OnlyManagedUsers.HasValue && request.OnlyManagedUsers.Value &&
                x.ManagerId == _authenticatedUserContext.UserId ||
                (!request.OnlyManagedUsers.HasValue || !request.OnlyManagedUsers.Value) &&
                !x.ManagerId.HasValue));
        }

        if (request.ProjectId is not null)
        {
            var project = await _projectsProvider.GetProjectByIdAsync(
                request.ProjectId.Value,
                cancellationToken);

            isPublicProject = project?.IsPublic ?? false;
        }

        if (isClientWithPublicProject && request.ProjectId is not null && !isPublicProject)
        {
            isClientWithPublicProject = false;
        }

        var filtersParams = new FilterParams
        {
            DepartmentId = request.DepartmentId,
            ResourceGroupId = request.ResourceGroupId,
            ManagerId = _authenticatedUserContext.UserId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            OnlyManagedUsers = request.OnlyManagedUsers,
            ReportForDateFrom = request.ReportForDateFrom,
            ReportForDateTo = request.ReportForDateTo,
        };

        var users = await _userProvider.GetLookupWithParametersForReportsAsync(
            filtersParams,
            isClientWithPublicProject,
            isPublicProject,
            isAnyPublicProjectManagedByUser,
            request.Keyword,
            cancellationToken);

        return users.ConvertAll(UserDto.Of);
    }

    internal sealed record Query(
        int? ClientId,
        int? ProjectId,
        string? DepartmentId,
        int? ResourceGroupId,
        string? Keyword,
        bool? OnlyManagedUsers,
        DateOnly? ReportForDateFrom,
        DateOnly? ReportForDateTo)

        : IQuery<List<UserDto>>, ITimeTrackRequest;

    private async Task ValidateRequest(Query request, CancellationToken cancellationToken)
    {
        if (!_authenticatedUserContext.GetRoles().IsEmployee() &&
           !_authenticatedUserContext.GetRoles().IsHeadOfDepartment() &&
            request.OnlyManagedUsers != null)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagedUsersUsageRestriction);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee())
        {
            var canUserBeManager = await _canUserBeProjectManagerBusinessRule
                .IsCorrespondsToBusiness(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

            if (!canUserBeManager ||
                request.OnlyManagedUsers is null ||
                request.OnlyManagedUsers == false)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCantBeManager);
            }
        }
    }
}
