using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Core;
using Modsen.Portal.Identity.Requests.Claims.Core.CoreDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class UpdateUserClaimsRequest : ICommandHandler<UpdateUserClaimsRequest.Command, List<GroupClaimsCoreDto>>
    {
        private readonly IIdentityDbContext _identityDbContext;
        private readonly IAuthenticationTokenContext _authenticationTokenContext;
        private readonly IUserGroupClaimsLoader _userGroupCliamsLoader;
        public UpdateUserClaimsRequest(IIdentityDbContext identityDbContext,
            IAuthenticationTokenContext authenticationTokenContext,
            IUserGroupClaimsLoader userGroupCliamsLoader)
        {
            _identityDbContext = identityDbContext;
            _authenticationTokenContext = authenticationTokenContext;
            _userGroupCliamsLoader = userGroupCliamsLoader;
        }

        public async Task<List<GroupClaimsCoreDto>> Handle(Command request, CancellationToken cancellationToken)
        {
            _ = await _identityDbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new EntityNotFoundException($"User with ID {request.UserId} not found.");

            var existingClaimTypes = await _identityDbContext.ClaimGroups
                .Include(x => x.Claims)
                .SelectMany(g => g.Claims)
                .Select(c => c.ClaimType)
                .ToHashSetAsync(cancellationToken);

            if (request.ClaimTypes.Any(ct => !existingClaimTypes.Contains(ct)))
            {
                throw new InvalidDataAppException("Invalid set of Claim types");
            }

            var userClaimsToRemove = _identityDbContext.UserClaims
                .Where(uc => uc.UserId == request.UserId);

            _identityDbContext.UserClaims.RemoveRange(userClaimsToRemove);

            foreach (var claimType in request.ClaimTypes)
            {
                _identityDbContext.UserClaims.Add(new UserClaim
                {
                    ClaimValue = "any",
                    UserId = request.UserId,
                    ClaimType = claimType
                });
            }

            await _identityDbContext.SaveChanges(cancellationToken);

            await _authenticationTokenContext.RevokeAllUserTokens(request.UserId, RevokeReasons.ClaimsChanged, cancellationToken);

            var userGroups = await _userGroupCliamsLoader.Load(request.UserId, cancellationToken);

            return userGroups;
        }

        internal sealed record Command(int UserId, IReadOnlyCollection<string> ClaimTypes)
            : ICommand<List<GroupClaimsCoreDto>>, ITransactionalRequest;
    }
}
