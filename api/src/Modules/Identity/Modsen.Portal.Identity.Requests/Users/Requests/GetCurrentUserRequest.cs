using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetCurrentUserRequest : IQueryHandler<GetCurrentUserRequest.Query, UserDto>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IUserLoader _userLoader;

    public GetCurrentUserRequest(IAuthenticatedUserContext authenticatedUserContext, IUserLoader userLoader)
    {
        _userLoader = userLoader;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<UserDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var user = await _userLoader.LoadAsync(_authenticatedUserContext.GetRequiredUserId(), cancellationToken);

        return user;
    }

    internal sealed record Query : IQuery<UserDto>, IIdentityRequest;
}
