using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.Requests.Users.Core;
using Modsen.Portal.Identity.Requests.Users.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Users.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetUserLookupRequest : IQueryHandler<GetUserLookupRequest.Query, List<UserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IUserLookupLoader _userLookupLoader;

    public GetUserLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IUserLookupLoader userLookupLoader,
        IIdentityDbContext identityDbContext)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _identityDbContext = identityDbContext;
        _userLookupLoader = userLookupLoader;
    }

    public async Task<List<UserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        string? departmentId = null;
        int? resourceGroupId = null;

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            departmentId = request.Roles.Count == 0
                || !await _identityDbContext.Roles
                    .Where(x => request.Roles.Contains(x.Name!))
                    .Select(x => x.IsCompanyManager)
                    .AllAsync(isManager => isManager, cancellationToken)
                    ? _authenticatedUserContext.DepartmentId
                    : null;
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            resourceGroupId = _authenticatedUserContext.ResourceGroupId;
        }

        var loaderRequestDto = new UserLookupLoaderRequestDto()
        {
            KeyWord = request.KeyWord,
            IsActive = request.IsActive,
            DepartmentId = departmentId,
            ResourceGroupId = resourceGroupId,
            Roles = request.Roles
        };

        return await _userLookupLoader.LoadAsync(loaderRequestDto, cancellationToken);
    }

    internal sealed record Query(string? KeyWord, bool? IsActive, List<string> Roles) : IQuery<List<UserLookupDto>>;
}
