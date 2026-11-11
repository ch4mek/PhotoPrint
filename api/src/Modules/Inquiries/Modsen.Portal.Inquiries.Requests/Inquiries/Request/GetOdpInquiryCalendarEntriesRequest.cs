using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;
using Modsen.Portal.Inquiries.Requests.InquiriesPortalSettings;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetOdpInquiryCalendarEntriesRequest
    : IQueryHandler<GetOdpInquiryCalendarEntriesRequest.Query, PageableListOfItems<OdpUserInquiryCalendarEntryDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITimeTrackContract _timeTrackContract;
    private readonly IInquiriesPortalSettingsAccessor _inquiriesPortalSettingsAccessor;

    public GetOdpInquiryCalendarEntriesRequest(IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        ITimeTrackContract timeTrackContract,
        IInquiriesPortalSettingsAccessor inquiriesPortalSettingsAccessor)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _timeTrackContract = timeTrackContract;
        _inquiriesPortalSettingsAccessor = inquiriesPortalSettingsAccessor;
    }

    public async Task<PageableListOfItems<OdpUserInquiryCalendarEntryDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var filterFrom = new DateOnly(request.Year, request.Month, 1);
        var filterTo = filterFrom.AddMonths(1);

        var usersQuery = _inquiriesDbContext.InquiriesUsers
            .AsQueryable()
            .Where(x => x.IsActive);

        var inquiries = _inquiriesDbContext.UserInquiries
            .Where(x => x.DateFrom < filterTo && x.DateTo >= filterFrom)
            .Where(x => UserInquiryEntity.InquiryStatusesThatCanBeViewedOnCalendar.Contains(x.Status))
            .AsQueryable();

        if (request.TemplateId.HasValue)
        {
            inquiries = inquiries.Where(x => x.InquiryTemplateId == request.TemplateId);
        }

        if (_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageRequests.Name)
            && !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ManageAllRequests.Name)
            && !_authenticatedUserContext.HasClaim(RequestsClaimTypeNames.ViewAllRequests.Name))
        {
            (usersQuery, inquiries) = await GetFilteredUsersAndInquiriesQuery(usersQuery, inquiries, cancellationToken);
        }

        var filteredUsersDto = usersQuery
            .Where(u => inquiries.Any(i => i.UserId == u.UserId))
            .Select(OdpUserInquiryCalendarEntryDto.Map);

        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(filteredUsersDto)
            .OfType<OdpUserInquiryCalendarEntryDto>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpUserInquiryCalendarEntryDto>.Value;
        }

        var pagedUsers = await request.QueryOptions
            .ApplyTo(filteredUsersDto)
            .OfType<OdpUserInquiryCalendarEntryDto>()
            .ToArrayAsync(cancellationToken);

        var pagedUserIds = pagedUsers.Select(u => u.UserId).ToArray();

        var rawInquiries = await inquiries
            .Where(i => pagedUserIds.Contains(i.UserId))
            .Select(i => new
            {
                i.UserId,
                i.Id,
                i.TotalHours,
                i.DateFrom,
                i.DateTo,
                InquiryType = i.InquiryTemplate!.InquiryType,
                InquiryName = i.InquiryTemplate.Name,
                Status = i.Status,
                TotalAcceptedCount = i.UserInquiryAccepts.Count,
                TotalAcceptUsersCount = i.UserInquiryAccepts.Count(f => f.Status == UserInquiryAcceptStatus.Accepted)
            })
            .ToArrayAsync(cancellationToken);

        var inquiriesByUserId = rawInquiries
            .GroupBy(r => r.UserId)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyCollection<UserInquiryBaseDto>)g.Select(r => new UserInquiryBaseDto
                {
                    Id = r.Id,
                    InquiryType = r.InquiryType.GetDescription(),
                    InquiryName = r.InquiryName,
                    Status = r.Status.GetDescription(),
                    TotalAcceptedCount = r.TotalAcceptedCount,
                    TotalAcceptUsersCount = r.TotalAcceptUsersCount,
                    TotalHours = r.TotalHours,
                    DateFrom = r.DateFrom,
                    DateTo = r.DateTo
                }).ToArray());

        var isHod = _authenticatedUserContext.GetRoles().IsHeadOfDepartment();
        var isGm = _authenticatedUserContext.GetRoles().IsGroupManager();

        var result = pagedUsers.Select(u => new OdpUserInquiryCalendarEntryDto
        {
            UserId = u.UserId,
            UserFullName = u.UserFullName,
            UserFullNameRu = u.UserFullNameRu,
            DepartmentId = u.DepartmentId,
            UserPosition = u.UserPosition,
            ResourceGroupId = u.ResourceGroupId,
            IsManagedByRoleRequest = isHod && u.DepartmentId == _authenticatedUserContext.DepartmentId
                || isGm && u.ResourceGroupId == _authenticatedUserContext.ResourceGroupId,
            Inquiries = inquiriesByUserId.TryGetValue(u.UserId, out var iq) ? iq : []
        }).ToArray();

        return result.ToPageable(request.QueryOptions, total);
    }

    private async Task<(IQueryable<InquiriesUserEntity> UsersQuery, IQueryable<UserInquiryEntity> InquiriesQuery)> GetFilteredUsersAndInquiriesQuery(
        IQueryable<InquiriesUserEntity> usersQuery,
        IQueryable<UserInquiryEntity> inquiriesQuery,
        CancellationToken cancellationToken)
    {
        var portalSettings = await _inquiriesPortalSettingsAccessor.GetSettings(cancellationToken);

        usersQuery = GetFilteredUsersQuery(usersQuery);
        inquiriesQuery = GetFilteredInquiriesQuery(inquiriesQuery);

        if (_authenticatedUserContext.IsFromDepartment(portalSettings.SalesDepartmentId))
        {
            var salesManagerId = _authenticatedUserContext.GetRequiredUserId();
            var userIdsInSalesManagerClientsProjects = await _timeTrackContract.GetUsersBySalesManagerId(salesManagerId, cancellationToken);

            usersQuery = UnionWithUsersFromSalesManagersClientsProjects(usersQuery, userIdsInSalesManagerClientsProjects);
            inquiriesQuery = UnionWithUsersFromSalesManagersClientsProjects(inquiriesQuery, userIdsInSalesManagerClientsProjects);
        }

        return (usersQuery, inquiriesQuery);
    }

    private IQueryable<InquiriesUserEntity> GetFilteredUsersQuery(IQueryable<InquiriesUserEntity> query)
    {
        var authRoles = _authenticatedUserContext.GetRoles();

        if (authRoles.IsHeadOfDepartment())
        {
            return query.Where(x => x.DepartmentId == _authenticatedUserContext.DepartmentId
                || x.UserInquiries
                .Any(i => i.UserInquiryAccepts.Any(a =>
                    a.AcceptUserId == _authenticatedUserContext.UserId
                    || a.AcceptedById == _authenticatedUserContext.UserId
                    || a.RejectedById == _authenticatedUserContext.UserId)));
        }

        if (authRoles.IsGroupManager())
        {
            return query.Where(x => x.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                || x.UserInquiries
                    .Any(i => i.UserInquiryAccepts.Any(a =>
                        a.AcceptUserId == _authenticatedUserContext.UserId
                        || a.AcceptedById == _authenticatedUserContext.UserId
                        || a.RejectedById == _authenticatedUserContext.UserId)));
        }

        if (authRoles.IsEmployee() || authRoles.IsSystemAdministrator())
        {
            return query.Where(x =>
                    x.UserId == _authenticatedUserContext.UserId
                    || x.UserInquiries.Any(i => i.UserInquiryAccepts.Any(a =>
                        a.AcceptUserId == _authenticatedUserContext.UserId
                        || a.AcceptedById == _authenticatedUserContext.UserId
                        || a.RejectedById == _authenticatedUserContext.UserId)));
        }

        return query;
    }

    private IQueryable<UserInquiryEntity> GetFilteredInquiriesQuery(IQueryable<UserInquiryEntity> query)
    {
        var authRoles = _authenticatedUserContext.GetRoles();

        if (authRoles.IsHeadOfDepartment())
        {
            return query.Where(x =>
                x.User!.DepartmentId == _authenticatedUserContext.DepartmentId
                || x.UserInquiryAccepts.Any(a =>
                    a.AcceptUserId == _authenticatedUserContext.UserId
                    || a.AcceptedById == _authenticatedUserContext.UserId
                    || a.RejectedById == _authenticatedUserContext.UserId));
        }

        if (authRoles.IsGroupManager())
        {
            return query.Where(x =>
                x.User!.ResourceGroupId == _authenticatedUserContext.ResourceGroupId
                || x.UserInquiryAccepts.Any(a =>
                    a.AcceptUserId == _authenticatedUserContext.UserId
                    || a.AcceptedById == _authenticatedUserContext.UserId
                    || a.RejectedById == _authenticatedUserContext.UserId));
        }

        if (authRoles.IsEmployee() || authRoles.IsSystemAdministrator())
        {
            return query.Where(x => x.UserInquiryAccepts.Any(a =>
                a.AcceptUserId == _authenticatedUserContext.UserId
                || a.AcceptedById == _authenticatedUserContext.UserId
                || a.RejectedById == _authenticatedUserContext.UserId));
        }

        return query;
    }

    private IQueryable<UserInquiryEntity> UnionWithUsersFromSalesManagersClientsProjects(
        IQueryable<UserInquiryEntity> query,
        IEnumerable<int> userIdsInSalesManagerClientsProjects)
    {
        var subQuery = _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(inquiry => userIdsInSalesManagerClientsProjects.Contains(inquiry.UserId));

        query = query.Union(subQuery);

        return query;
    }

    private IQueryable<InquiriesUserEntity> UnionWithUsersFromSalesManagersClientsProjects(
        IQueryable<InquiriesUserEntity> query,
        IEnumerable<int> userIdsInSalesManagerClientsProjects)
    {
        var subQuery = _inquiriesDbContext.InquiriesUsers
            .AsNoTracking()
            .Where(user => userIdsInSalesManagerClientsProjects.Contains(user.UserId));

        query = query.Union(subQuery);

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpUserInquiryCalendarEntryDto> QueryOptions, int Year, int Month, int? TemplateId)
        : IQuery<PageableListOfItems<OdpUserInquiryCalendarEntryDto>>;
}
