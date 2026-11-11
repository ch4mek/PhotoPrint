using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;
using Modsen.Portal.Inquiries.Requests.InquiryUsers.Core.CoreDto;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.InquiryUsers.Requests;

internal sealed class GetInquiryUsersLookupRequest
    : IQueryHandler<GetInquiryUsersLookupRequest.Query, List<InquiryUserLookupDto>>
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly IInquiriesPortalSettingsAccessor _inquiriesPortalSettingsAccessor;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;

    public GetInquiryUsersLookupRequest(
        IAuthenticatedUserContext authenticatedUserContext,
        IInquiriesDbContext inquiriesDbContext,
        ITimeTrackContract timeTrackContract,
        IInquiriesPortalSettingsAccessor inquiriesPortalSettingsAccessor,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _inquiriesDbContext = inquiriesDbContext;
        _timeTrackContract = timeTrackContract;
        _inquiriesPortalSettingsAccessor = inquiriesPortalSettingsAccessor;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
    }

    public async Task<List<InquiryUserLookupDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var inquiryUsers = await GetUsersAsync(request.IsApprovals, cancellationToken);

        return inquiryUsers;
    }

    private async Task<List<InquiryUserLookupDto>> GetUsersAsync(bool? isApprovals, CancellationToken cancellationToken)
    {
        var query = _inquiriesDbContext.InquiriesUsers
            .OrderBy(user => user.UserId)
            .Where(user => user.IsActive);

        var canManageAll = isApprovals is true &&
            _authenticatedUserContext.HasClaim(AbsencesClaimTypeNames.ManageAbsenceSettings.Name);
        if (canManageAll)
        {
            return await GetResultFromQueryAsync(query, cancellationToken);
        }

        var canManageOnlySubordinates =
            _authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageRequests.Name) &&
            !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name) &&
            !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name);

        if (canManageOnlySubordinates)
        {
            query = await ApplySubordinatesFilterAsync(query, cancellationToken);
        }

        return await GetResultFromQueryAsync(query, cancellationToken);
    }

    private async Task<IQueryable<InquiriesUserEntity>> ApplySubordinatesFilterAsync(
        IQueryable<InquiriesUserEntity> query,
        CancellationToken cancellationToken)
    {
        var portalSettings = await _inquiriesPortalSettingsAccessor.GetSettings(cancellationToken);

        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(user => user.DepartmentId == _authenticatedUserContext.DepartmentId ||
                (user.UserInquiries.Any(inquiry => inquiry.UserInquiryAccepts
                    .Any(accept => accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId))));
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(user => user.ResourceGroupId == _authenticatedUserContext.ResourceGroupId ||
                (user.UserInquiries.Any(inquiry => inquiry.UserInquiryAccepts
                    .Any(accept => accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId))));
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(user => user.UserId == _authenticatedUserContext.UserId ||
                (user.UserInquiries.Any(inquiry => inquiry.UserInquiryAccepts
                    .Any(accept => accept.AcceptUserId == _authenticatedUserContext.UserId
                        || accept.AcceptedById == _authenticatedUserContext.UserId
                        || accept.RejectedById == _authenticatedUserContext.UserId))));
        }

        if (_authenticatedUserContext.IsFromDepartment(portalSettings.SalesDepartmentId))
        {
            query = await UnionWithUsersFromSalesManagersClientsProjects(query, cancellationToken);
        }

        var inquiryIdsCanBeApprovedAsSubstitute = await _getInquiriesCanBeApprovedAsSubstituteResolver
            .ResolveAsync(onlyAsAnyUser: true, cancellationToken);

        var inquiryUserIdsToGet = await _inquiriesDbContext.UserInquiries
            .Where(x => inquiryIdsCanBeApprovedAsSubstitute.Contains(x.Id))
            .Select(x => x.UserId)
            .ToListAsync(cancellationToken);

        var userIdsToGet = await query
            .Select(x => x.UserId)
            .Concat(inquiryUserIdsToGet)
            .ToArrayAsync(cancellationToken);

        return _inquiriesDbContext.InquiriesUsers
            .AsNoTracking()
            .Where(x => userIdsToGet.Contains(x.UserId));
    }

    private async Task<IQueryable<InquiriesUserEntity>> UnionWithUsersFromSalesManagersClientsProjects(
        IQueryable<InquiriesUserEntity> query,
        CancellationToken cancellationToken)
    {
        var salesManagerId = _authenticatedUserContext.GetRequiredUserId();

        var userIdsInSalesManagerClientsProjects = await _timeTrackContract.GetUsersBySalesManagerId(salesManagerId, cancellationToken);

        var subQuery = _inquiriesDbContext.InquiriesUsers
            .AsNoTracking()
            .Where(user => userIdsInSalesManagerClientsProjects.Contains(user.UserId));

        query = query.Union(subQuery);

        return query;
    }

    private static async Task<List<InquiryUserLookupDto>> GetResultFromQueryAsync(IQueryable<InquiriesUserEntity> query,
        CancellationToken token)
    {
        return await query.Select(user => new InquiryUserLookupDto
            {
                Id = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FirstNameRu = user.FirstNameRu,
                LastNameRu = user.LastNameRu,
            })
            .ToListAsync(token);
    }

    internal sealed record Query(bool? IsApprovals) : IQuery<List<InquiryUserLookupDto>>;
}
