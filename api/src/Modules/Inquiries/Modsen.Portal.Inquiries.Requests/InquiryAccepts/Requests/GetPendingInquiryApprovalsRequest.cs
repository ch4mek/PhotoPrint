using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests.RequestDtos;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Requests;

internal sealed class
    GetPendingInquiryApprovalsRequest : IQueryHandler<GetPendingInquiryApprovalsRequest.Query,
    List<PendingInquiryApprovalDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetInquiriesCanBeApprovedAsSubstituteResolver _getInquiriesCanBeApprovedAsSubstituteResolver;
    private readonly ITeamBoardContract _teamBoardContract;

    public GetPendingInquiryApprovalsRequest(
        IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IGetInquiriesCanBeApprovedAsSubstituteResolver getInquiriesCanBeApprovedAsSubstituteResolver,
        ITeamBoardContract teamBoardContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _getInquiriesCanBeApprovedAsSubstituteResolver = getInquiriesCanBeApprovedAsSubstituteResolver;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<List<PendingInquiryApprovalDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        ValidateCurrentUserRole();

        var currentUserId = _authenticatedUserContext.GetRequiredUserId();
        var substituteInquiryIds =
            await _getInquiriesCanBeApprovedAsSubstituteResolver.ResolveAsync(onlyAsAnyUser: false, cancellationToken);

        var result = await ComposeQuery(request, currentUserId, substituteInquiryIds)
            .OrderBy(x => x.DateFrom)
            .ToListAsync(cancellationToken);

        await EnrichResponse(result, cancellationToken);

        return result;
    }

    private void ValidateCurrentUserRole()
    {
        var roles = _authenticatedUserContext.GetRoles();
        var hasOnlyRestrictedRoles =
            roles.Count != 0 && roles.All(role => role.IsEmployee() || role.IsSystemAdministrator());

        if (hasOnlyRestrictedRoles)
        {
            throw new ActionNotAllowedException(
                "Employee and SystemAdministrator roles are not allowed to access pending approvals");
        }
    }

    private IQueryable<PendingInquiryApprovalDto> ComposeQuery(Query request, int currentUserId,
        List<int> substituteInquiryIds)
    {
        var userInquiryAcceptsWaitingForApprovalQuery = _inquiriesDbContext.UserInquiryAccepts
            .Where(x =>
                x.Status == UserInquiryAcceptStatus.WaitingForApprove &&
                x.UserInquiryId.HasValue &&
                (x.AcceptUserId == currentUserId || substituteInquiryIds.Contains(x.UserInquiryId.Value))
            );

        var openInquiriesQuery = _inquiriesDbContext.UserInquiries
            .Where(inquiry => inquiry.User != null && inquiry.InquiryTemplate != null)
            .Where(inquiry => inquiry.Status == UserInquiryStatus.Open);

        var query = userInquiryAcceptsWaitingForApprovalQuery
            .Join(
                openInquiriesQuery,
                accept => accept.UserInquiryId!.Value,
                inquiry => inquiry.Id,
                (accept, inquiry) => new PendingInquiryApprovalDto
                {
                    UserId = inquiry.UserId,
                    DepartmentId = inquiry.User!.DepartmentId,
                    ResourceGroupId = inquiry.User.ResourceGroupId,
                    InquiryId = inquiry.Id,
                    InquiryTemplateId = inquiry.InquiryTemplateId,
                    InquiryTemplateName = inquiry.InquiryTemplate!.Name,
                    UserFullName = inquiry.User.LastNameRu == null && inquiry.User.FirstNameRu == null
                        ? inquiry.User.LastName + " " + inquiry.User.FirstName
                        : (inquiry.User.LastNameRu + " " + inquiry.User.FirstNameRu + " (" + inquiry.User.LastName +
                           " " + inquiry.User.FirstName + ")").Trim(),
                    InquiryType = inquiry.InquiryTemplate.InquiryType,
                    DateFrom = inquiry.DateFrom
                });

        if (request.DepartmentId is not null)
        {
            query = query.Where(x => x.DepartmentId == request.DepartmentId);
        }

        return query;
    }

    private async Task EnrichResponse(List<PendingInquiryApprovalDto> result, CancellationToken cancellationToken)
    {
        var usersIds = result.Select(x => x.UserId).Distinct().ToArray();
        var accessOptions = AccessOptions.Self | AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        var canUsersBeRetrievedByRequesterResult = await _teamBoardContract.CanUsersBeRetrievedByUser(usersIds, accessOptions, cancellationToken);

        foreach (var userInfo in result)
        {
            userInfo.CanUserInfoBeRetrievedByCurrentUser = false;

            if (canUsersBeRetrievedByRequesterResult.TryGetValue(userInfo.UserId, out var canUserBeRetrieved))
            {
                userInfo.CanUserInfoBeRetrievedByCurrentUser = canUserBeRetrieved;
            }
        }
    }

    internal sealed record Query(string? DepartmentId = null) : IQuery<List<PendingInquiryApprovalDto>>;
}
