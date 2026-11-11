using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Request.RequestDto;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetApprovedAbsencesRequest : IQueryHandler<GetApprovedAbsencesRequest.Query, List<ApprovedAbsenceDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;

    public GetApprovedAbsencesRequest(
        IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
    }

    public async Task<List<ApprovedAbsenceDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        ValidateCurrentUserRole();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var currentRoles = _authenticatedUserContext.GetRoles();
        var currentDepartmentId = _authenticatedUserContext.DepartmentId;
        var currentResourceGroupId = _authenticatedUserContext.ResourceGroupId;

        var absenceRows = await _inquiriesDbContext.UserInquiries
            .Where(x => x.Status == UserInquiryStatus.Approved &&
                        x.DateFrom <= today &&
                        x.DateTo >= today &&
                        x.User != null &&
                        x.InquiryTemplate != null &&
                        (x.InquiryTemplate.ActivityType == ActivityType.NonWorkingPaid ||
                         x.InquiryTemplate.ActivityType == ActivityType.NonWorkingUnpaid))
            .Select(x => new
            {
                x.Id,
                x.UserId,
                InquiryTemplateName = x.InquiryTemplate!.Name,
                x.InquiryTemplate!.InquiryType,
                x.User!.LastNameRu,
                x.User.FirstNameRu,
                x.User.LastName,
                x.User.FirstName,
                x.User.Position
            })
            .ToListAsync(cancellationToken);

        var userIds = absenceRows.Select(x => x.UserId).Distinct().ToArray();
        var activeUsers = await _identityContract.GetUsersWithManagersAsync(userIds, cancellationToken);
        var activeUsersById = activeUsers.ToDictionary(x => x.UserId);

        var filteredRows = absenceRows
            .Where(row => activeUsersById.ContainsKey(row.UserId))
            .Where(row => IsAccessibleForCurrentUser(currentRoles, currentDepartmentId, currentResourceGroupId, activeUsersById[row.UserId]));

        if (request.DepartmentId != null)
        {
            filteredRows = filteredRows.Where(row => activeUsersById[row.UserId].DepartmentId == request.DepartmentId);
        }

        return filteredRows
            .Select(row => new ApprovedAbsenceDto
            {
                UserId = row.UserId,
                InquiryId = row.Id,
                InquiryTemplateName = row.InquiryTemplateName,
                UserFullName = row.LastNameRu == null && row.FirstNameRu == null
                    ? row.LastName + " " + row.FirstName
                    : (row.LastNameRu + " " + row.FirstNameRu + " (" + row.LastName + " " + row.FirstName + ")").Trim(),
                InquiryType = row.InquiryType,
                UserPosition = row.Position
            })
            .OrderBy(x => x.UserFullName)
            .ToList();
    }

    private static bool IsAccessibleForCurrentUser(
        IReadOnlyCollection<string> currentRoles,
        string? currentDepartmentId,
        int? currentResourceGroupId,
        Identity.Contract.Models.UserWithManagersDto activeUser)
    {
        if (currentRoles.IsCeo() || currentRoles.IsCto())
        {
            return true;
        }

        if (currentRoles.IsHeadOfDepartment())
        {
            return activeUser.DepartmentId == currentDepartmentId;
        }

        if (currentRoles.IsGroupManager())
        {
            return activeUser.ResourceGroupId == currentResourceGroupId;
        }

        return false;
    }

    private void ValidateCurrentUserRole()
    {
        var roles = _authenticatedUserContext.GetRoles();

        if (!roles.IsCeo() && !roles.IsCto() && !roles.IsHeadOfDepartment() && !roles.IsGroupManager())
        {
            throw new ActionNotAllowedException("Only CEO, CTO, HeadOfDepartment and GroupManager can access approved absences");
        }
    }

    internal sealed record Query(string? DepartmentId = null) : IQuery<List<ApprovedAbsenceDto>>;
}
