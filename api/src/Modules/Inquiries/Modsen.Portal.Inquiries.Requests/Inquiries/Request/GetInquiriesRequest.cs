using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request
{
    internal sealed class GetInquiriesRequest : IQueryHandler<GetInquiriesRequest.Query, PageableListOfItems<InquiryCoreDto>>
    {
        private readonly IAuthenticatedUserContext _authenticatedUserContext;
        private readonly IInquiriesDbContext _inquiriesDbContext;
        private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;
        private readonly IInquiriesPortalSettingsAccessor _inquiriesPortalSettingsAccessor;
        private readonly ITimeTrackContract _timeTrackContract;

        public GetInquiriesRequest(
            IAuthenticatedUserContext authenticatedUserContext,
            IInquiriesDbContext inquiriesDbContext,
            IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver,
            IInquiriesPortalSettingsAccessor inquiriesPortalSettingsAccessor,
            ITimeTrackContract timeTrackContract)
        {
            _authenticatedUserContext = authenticatedUserContext;
            _inquiriesDbContext = inquiriesDbContext;
            _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
            _inquiriesPortalSettingsAccessor = inquiriesPortalSettingsAccessor;
            _timeTrackContract = timeTrackContract;
        }

        public async Task<PageableListOfItems<InquiryCoreDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            var queryOptions = request.QueryOptions;

            var query = _inquiriesDbContext.UserInquiries
                .AsNoTracking();

            if (_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageRequests.Name) &&
                !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name) &&
                !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name))
            {
                query = await GetFilteredQuery(query, cancellationToken);

                var inquiryIdsCanBeApprovedAsSubstitute = await _getInquiriesCanBeApprovedAsSubstituteResolver
                    .ResolveAsync(onlyAsAnyUser: true, cancellationToken);

                var inquiryIdsToGet = await query.Select(x => x.Id)
                    .Concat(inquiryIdsCanBeApprovedAsSubstitute)
                    .ToArrayAsync(cancellationToken);

                query = _inquiriesDbContext.UserInquiries
                    .AsNoTracking()
                    .Where(x => inquiryIdsToGet.Contains(x.Id));
            }

            var convertedQuery = query
                .Include(ui => ui.User)
                .Include(ui => ui.InquiryTemplate)
                .Include(ui => ui.UserInquiryAccepts)
                .Select(InquiryCoreDto.Map)
                .AsSplitQuery();

            var queryResult = await queryOptions
                .ApplyTo(convertedQuery)
                .Cast<InquiryCoreDto>()
                .ToListAsync(cancellationToken);

            var totalCount = await queryOptions
                .RebuildWithoutPagination()
                .ApplyTo(convertedQuery)
                .OfType<InquiryCoreDto>()
                .CountAsync(cancellationToken);

            if (totalCount == 0)
            {
                return EmptyPageableListOfItems<InquiryCoreDto>.Value;
            }

            var departmentId = _authenticatedUserContext.DepartmentId;
            var resourceGroupId = _authenticatedUserContext.ResourceGroupId;
            var isHead = _authenticatedUserContext.GetRoles().IsHeadOfDepartment();
            var isManager = _authenticatedUserContext.GetRoles().IsGroupManager();

            foreach (var inquiry in queryResult)
            {
                inquiry.ManagedByRoleRequest =
                    (isHead && inquiry.UserDepartmentId == departmentId)
                    || (isManager && inquiry.UserResourceGroupId == resourceGroupId);
            }

            return queryResult.ToPageable(queryOptions, totalCount);
        }

        private async Task<IQueryable<UserInquiryEntity>> GetFilteredQuery(IQueryable<UserInquiryEntity> query, CancellationToken cancellationToken)
        {
            var portalSettings = await _inquiriesPortalSettingsAccessor.GetSettings(cancellationToken);
            var salesDepartmentId = portalSettings.SalesDepartmentId;

            if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
            {
                query = query.Where(inquiry =>
                    inquiry.User!.DepartmentId == _authenticatedUserContext.DepartmentId ||
                    inquiry.UserInquiryAccepts.Any(accept =>
                        accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId));
            }

            if (_authenticatedUserContext.GetRoles().IsGroupManager())
            {
                query = query.Where(inquiry =>
                    inquiry.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId ||
                    inquiry.UserInquiryAccepts.Any(accept =>
                        accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId));
            }

            if (_authenticatedUserContext.GetRoles().IsEmployee() ||
                _authenticatedUserContext.GetRoles().IsSystemAdministrator())
            {
                query = query.Where(inquiry =>
                    inquiry.UserId == _authenticatedUserContext.UserId
                    || inquiry.UserInquiryAccepts.Any(accept =>
                        accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId));
            }

            if (_authenticatedUserContext.IsFromDepartment(salesDepartmentId))
            {
                query = await UnionWithUsersFromSalesManagersClientsProjects(query, cancellationToken);
            }

            return query;
        }

        private async Task<IQueryable<UserInquiryEntity>> UnionWithUsersFromSalesManagersClientsProjects(IQueryable<UserInquiryEntity> query, CancellationToken cancellationToken)
        {
            var salesManagerId = _authenticatedUserContext.GetRequiredUserId();

            var userIdsInSalesManagerClientsProjects = await _timeTrackContract.GetUsersBySalesManagerId(salesManagerId, cancellationToken);

            var subQuery = _inquiriesDbContext.UserInquiries
                .AsNoTracking()
                .Where(inquiry => userIdsInSalesManagerClientsProjects.Contains(inquiry.UserId));

            query = query.Union(subQuery);

            return query;
        }

        internal sealed record Query(ODataQueryOptions<InquiryCoreDto> QueryOptions)
            : IQuery<PageableListOfItems<InquiryCoreDto>>;

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
