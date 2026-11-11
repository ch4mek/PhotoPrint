using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.Claims.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.Claims.Requests
{
    internal sealed class GetUsersClaimsRequest : IQueryHandler<GetUsersClaimsRequest.Query, PageableListOfItems<UserClaimsDto>>
    {
        private readonly IIdentityDbContext _identityDbContext;
        public GetUsersClaimsRequest(IIdentityDbContext identityDbContext)
        {
            _identityDbContext = identityDbContext;
        }

        public async Task<PageableListOfItems<UserClaimsDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = _identityDbContext.Users
                .AsNoTracking()
                .Where(x => x.IsActive)
                .Select(u => new UserClaimsDto
                {
                    UserId = u.Id,
                    UserFullName = u.LastName + " " + u.FirstName,
                    UserFullNameRu = u.LastNameRu == null && u.FirstNameRu == null ?
                        null :
                    u.LastNameRu + " " + u.FirstName,
                    DepartmentId = u.DepartmentId,
                    Color = u.Color,
                    Groups = new List<ClaimGroupDto>()
                });

            if (request.ClaimName != null)
            {
                var usersWithClaim = await _identityDbContext.UserClaims
                    .AsNoTracking()
                    .Where(x => x.ClaimType == request.ClaimName)
                    .Select(x => x.UserId)
                    .ToListAsync(cancellationToken);

                query = query
                    .Where(x => usersWithClaim.Contains(x.UserId));
            }

            var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();
            var filteredQuery = request.QueryOptions.ApplyTo(query);
            var result = await filteredQuery.Cast<UserClaimsDto>().ToListAsync(cancellationToken);

            var queryWithoutPagination = queryOptionsWithoutPagination.ApplyTo(query).OfType<UserClaimsDto>();
            var totalCount = await queryWithoutPagination.CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return EmptyPageableListOfItems<UserClaimsDto>.Value;
            }

            var allClaimGroups = await _identityDbContext.ClaimGroups
                .Include(group => group.Claims)
                .ToListAsync(cancellationToken);

            foreach (var user in result)
            {
                var userClaims = await _identityDbContext.UserClaims
                    .Where(uc => uc.UserId == user.UserId)
                    .Select(uc => uc.ClaimType!)
                    .ToListAsync(cancellationToken);

                user.Groups = allClaimGroups.Select(group => new ClaimGroupDto
                {
                    Name = group.Name,
                    Status = GetClaimGroupStatus(group.Claims.ToList(), userClaims)
                })
                .ToList();
            }

            return result.ToPageable(request.QueryOptions, totalCount);
        }

        public static ClaimsGroupStatus GetClaimGroupStatus(List<ClaimEntity> groupClaims, List<string> userClaimTypes)
        {
            var groupClaimTypes = groupClaims.Select(c => c.ClaimType).ToList();

            var userClaimsInGroup = groupClaimTypes.Intersect(userClaimTypes).ToList();

            if (userClaimsInGroup.Count == 0)
            {
                return ClaimsGroupStatus.NoAccess;
            }
            else if (userClaimsInGroup.Count == groupClaimTypes.Count)
            {
                return ClaimsGroupStatus.FullAccess;
            }
            else
            {
                return ClaimsGroupStatus.PartialAccess;
            }
        }

        internal sealed record Query(
            ODataQueryOptions<UserClaimsDto> QueryOptions,
            string? ClaimName)
            : IQuery<PageableListOfItems<UserClaimsDto>>, IIdentityRequest;

        internal sealed class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
                RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
            }
        }
    }
}
