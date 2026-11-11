using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IUpdateUserAsApproverAcceptResolver
{
    Task ResolveAsync(UpdateUserAcceptResolverRequest request, CancellationToken cancellationToken);
}

internal class UpdateUserAsApproverAcceptResolver : IUpdateUserAsApproverAcceptResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUpdateUserInfoInquiryAcceptsResolver _updateOpenInquiriesAcceptsResolver;

    public UpdateUserAsApproverAcceptResolver(IInquiriesDbContext inquiriesDbContext,
        IUpdateUserInfoInquiryAcceptsResolver updateOpenInquiriesAcceptsResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _updateOpenInquiriesAcceptsResolver = updateOpenInquiriesAcceptsResolver;
    }

    public async Task ResolveAsync(UpdateUserAcceptResolverRequest request, CancellationToken cancellationToken)
    {
        var isSame = request.User.Position == request.PrevPosition
            && request.User.DepartmentId == request.PrevDepartmentId
            && request.User.ResourceGroupId == request.PrevResourceGroupId
            && request.User.IsActive == request.IsPrevActive;
        
        if (isSame)
        {
            return;
        }

        var prevOpenInquiries = _inquiriesDbContext.UserInquiries
            .Where(x => x.UserInquiryAccepts.Any(x =>
                x.AcceptUserId == request.User.UserId && (x.Status == UserInquiryAcceptStatus.WaitingForApprove
                    || x.Status == UserInquiryAcceptStatus.Pending)))
            .Select(x => x.Id);

        if (IsManager(request.User.Position) && request.User.IsActive)
        {
            var templateIds = _inquiriesDbContext.InquiryTemplateAcceptPolicies
                .Where(x => x.Scope == PositionConvert(request.User.Position))
                .Select(x => x.InquiryTemplateId);

            var inquiriesQuery = _inquiriesDbContext.UserInquiries
                .Where(x => templateIds.Contains(x.InquiryTemplateId))
                .Where(x => x.Status == UserInquiryStatus.Open);

            if (request.User.Position.IsHeadOfDepartment())
            {
                inquiriesQuery = inquiriesQuery.Where(x => x.User!.DepartmentId == request.User.DepartmentId);
            }
            else if (request.User.Position.IsGroupManager() && request.User.ResourceGroupId.HasValue)
            {
                inquiriesQuery = inquiriesQuery.Where(x => x.User!.ResourceGroupId.HasValue &&
                    x.User!.ResourceGroupId == request.User.ResourceGroupId);
            }

            prevOpenInquiries = prevOpenInquiries.Concat(inquiriesQuery.Select(x => x.Id));
        }

        var inquiriesToUpdate = await prevOpenInquiries.ToArrayAsync(cancellationToken);

        if (inquiriesToUpdate.Length > 0)
        {
            await _updateOpenInquiriesAcceptsResolver.ResolveAsync(inquiriesToUpdate, cancellationToken);
        }
    }

    private static InquiryAcceptPolicyScopes PositionConvert(string? position)
    {
        return position switch
        {
            PortalRoleNames.Ceo or PortalRoleNames.Cto => InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO,
            PortalRoleNames.HeadOfDepartment => InquiryAcceptPolicyScopes.HeadOfDepartment,
            PortalRoleNames.GroupManager => InquiryAcceptPolicyScopes.GroupManager,
            PortalRoleNames.Employee or PortalRoleNames.SystemAdministrator => InquiryAcceptPolicyScopes.User,
            _ => throw new ArgumentException(position),
        };
    }

    private static bool IsManager(string? position)
    {
        return position.IsCeo() || position.IsCto() || position.IsHeadOfDepartment() || position.IsGroupManager();
    }
}
