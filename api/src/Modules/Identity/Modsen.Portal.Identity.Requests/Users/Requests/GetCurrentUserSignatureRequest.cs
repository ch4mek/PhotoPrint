using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;

namespace Modsen.Portal.Identity.Requests.Users.Requests;

internal sealed class GetCurrentUserSignatureRequest : IQueryHandler<GetCurrentUserSignatureRequest.Query, UserSignatureDto>
{
    private readonly IUserLoader _userLoader;
    private readonly IAuthenticatedUserContext _authenticatedUser;

    public GetCurrentUserSignatureRequest(
        IUserLoader userLoader,
        IAuthenticatedUserContext authenticatedUser)
    {
        _userLoader = userLoader;
        _authenticatedUser = authenticatedUser;
    }

    public async Task<UserSignatureDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var currentUser = await _userLoader.LoadAsync(_authenticatedUser.GetRequiredUserId(), cancellationToken);

        return new UserSignatureDto
        {
            FullName = !currentUser.Signature.FullName.IsNullOrWhiteSpace()
                ? currentUser.Signature.FullName
                : ToFullName.Reverse(currentUser.FirstName, currentUser.LastName),
            Position = currentUser.Signature.Position,
            Phone = currentUser.Signature.Phone,
            Email = currentUser.Email
        };
    }

    internal sealed record Query : IQuery<UserSignatureDto>, IIdentityRequest;
}
