using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Requests;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface IGetTimesheetValidator
{
    Task ValidateAsync(GetTimesheetsRequest.Query request, CancellationToken cancellationToken);
}

internal sealed class GetTimesheetValidator : IGetTimesheetValidator
{
    private readonly ICanUserBeProjectManagerBusinessRule _canUserBeProjectManagerBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetUserDetailsDbQuery _getUserDetailsDbQuery;

    public GetTimesheetValidator(ICanUserBeProjectManagerBusinessRule canUserBeProjectManagerBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetUserDetailsDbQuery getUserDetailsDbQuery)
    {
        _canUserBeProjectManagerBusinessRule = canUserBeProjectManagerBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
        _getUserDetailsDbQuery = getUserDetailsDbQuery;
    }

    public async Task ValidateAsync(GetTimesheetsRequest.Query request, CancellationToken cancellationToken)
    {
        if (request.SearchFilters.UserId.HasValue)
        {
            var userDetails = await _getUserDetailsDbQuery.Execute(request.SearchFilters.UserId.Value,
                    cancellationToken)
                ?? throw new ArgumentException($"User with ID {request.SearchFilters.UserId.Value} does not exists");

            if (_authenticatedUserContext.GetRoles().IsGroupManager() &&
                _authenticatedUserContext.ResourceGroupId != userDetails.ResourceGroupId)
            {
                throw new ActionNotAllowedException(PortalMessages.GroupManagerCanGetOnlyTheSameResourceGroup);
            }
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() && request.SearchFilters.OnlyManagedProjects != true)
        {
            throw new ActionNotAllowedException(PortalMessages.EmployeeCanGetOnlyManagedProjects);
        }

        if (request.SearchFilters.OnlyManagedProjects == true)
        {
            var canUserBeManager = await _canUserBeProjectManagerBusinessRule
                .IsCorrespondsToBusiness(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

            if (!canUserBeManager || _authenticatedUserContext.GetRoles().IsGroupManager())
            {
                throw new ActionNotAllowedException(PortalMessages.CurrentUserCantBeProjectManager);
            }
        }
    }
}
