using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Providers;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Queries.GetUsersLookupWithFilters;

internal sealed class GetUsersLookupWithFiltersQueryHandler : IQueryHandler<GetUsersLookupWithFiltersQuery, List<UserDto>>
{
    private readonly ITimeTrackUserProvider _userProvider;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetUsersLookupWithFiltersQueryHandler(
        ITimeTrackUserProvider userProvider,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _userProvider = userProvider;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<List<UserDto>> Handle(GetUsersLookupWithFiltersQuery request, CancellationToken cancellationToken)
    {
        var departmentId = _authenticatedUserContext.GetRoles().IsHeadOfDepartment()
            ? _authenticatedUserContext.DepartmentId
            : request.DepartmentId;

        var resourceGroupId = _authenticatedUserContext.GetRoles().IsGroupManager()
            ? _authenticatedUserContext.ResourceGroupId
            : null;

        var filtersParams = new FilterParams
        {
            DepartmentId = departmentId,
            ResourceGroupId = resourceGroupId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId
        };

        var users = await _userProvider.GetLookupWithParametersAsync(filtersParams, request.Keyword, cancellationToken);

        return users.ConvertAll(UserDto.Of);
    }
}
