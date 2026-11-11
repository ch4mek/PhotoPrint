using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Requests.Claims.Core;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class GetUserClaimsRequest : IQueryHandler<GetUserClaimsRequest.Query, List<GroupClaimsCoreDto>>
    {
        private readonly IUserGroupClaimsLoader _userGroupClaimsLoader;

        public GetUserClaimsRequest(
            IUserGroupClaimsLoader userGroupClaimsLoader)
        {
            _userGroupClaimsLoader = userGroupClaimsLoader;
        }

        public async Task<List<GroupClaimsCoreDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            return await _userGroupClaimsLoader.Load(request.UserId, cancellationToken);
        }

        internal sealed record Query(int UserId) : IQuery<List<GroupClaimsCoreDto>>, IIdentityRequest;
    }
}
