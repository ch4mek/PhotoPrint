using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Helpers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IUpdatePolicyInquiryAcceptsResolver
{
    Task<(List<InquiryUserNotifyDto>, List<InquiryUserNotifyDto>)> ResolveAsync(int[] userInquiryIds, CancellationToken cancellationToken);
}

internal class UpdatePolicyInquiryAcceptsResolver : IUpdatePolicyInquiryAcceptsResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryAcceptsComposer _inquiryAcceptsComposer;
    private readonly IInquiryAcceptedStatusResolver _inquiryAcceptedStatusResolver;

    public UpdatePolicyInquiryAcceptsResolver(IInquiriesDbContext inquiriesDbContext,
        IInquiryAcceptsComposer inquiryAcceptsComposer, IInquiryAcceptedStatusResolver inquiryAcceptedStatusResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryAcceptsComposer = inquiryAcceptsComposer;
        _inquiryAcceptedStatusResolver = inquiryAcceptedStatusResolver;
    }
    public async Task<(List<InquiryUserNotifyDto>, List<InquiryUserNotifyDto>)> ResolveAsync(int[] userInquiryIds, CancellationToken cancellationToken)
    {
        var approversToNotifyAboutOrderChange = new List<InquiryUserNotifyDto>();
        var approversCurrentTurnToNotify = new List<InquiryUserNotifyDto>();

        var inquiries = await _inquiriesDbContext.UserInquiries
            .Include(x => x.UserInquiryAccepts)
            .Where(x => userInquiryIds.Contains(x.Id))
            .ToListAsync(cancellationToken);

        var acceptsToRemove = inquiries.SelectMany(x => x.UserInquiryAccepts).ToArray();

        foreach (var inquiry in inquiries)
        {
            var inquiryAcceptsLookup = inquiry!.UserInquiryAccepts
                .ToLookup(x => new AcceptScopeUserHelper(x.AcceptPolicyScope, x.AcceptUserId));

            var orderedInquiryAcceptScopes = inquiry.UserInquiryAccepts
                .OrderBy(x => x.Id)
                .Select(x => new AcceptScopeUserHelper(x.AcceptPolicyScope, x.AcceptUserId))
                .Distinct()
                .ToArray();

            var composedAccepts = await _inquiryAcceptsComposer
                .ComposeAsync(inquiry.UserId, inquiry.InquiryTemplateId, cancellationToken);

            var composedAcceptsLookup = composedAccepts
                .ToLookup(x => new AcceptScopeUserHelper(x.AcceptPolicyScope, x.AcceptUserId));

            var orderedComposedAcceptScopes = composedAccepts
                .OrderBy(x => x.Id)
                .Select(x => new AcceptScopeUserHelper(x.AcceptPolicyScope, x.AcceptUserId))
                .Distinct()
                .ToArray();

            var differFrom = DifferFromIndex(orderedInquiryAcceptScopes, orderedComposedAcceptScopes);

            // all same AcceptEntity copy from old values in order to save accepted.
            // Non-accepted entities do not corrupt logic
            var newAccepts = CopyUserInquiryAccepts(orderedInquiryAcceptScopes
                    .Take(differFrom)
                    .SelectMany(x => inquiryAcceptsLookup[x]))
                .ToList();

            if (differFrom == 0 || inquiryAcceptsLookup[orderedInquiryAcceptScopes[differFrom - 1]]
                .All(x => x.Status == UserInquiryAcceptStatus.Accepted))
            {
                if (differFrom == orderedComposedAcceptScopes.Length)
                {
                    await _inquiryAcceptedStatusResolver.ResolveAsync(inquiry, cancellationToken);
                }
                else
                {
                    SetScopeStatus(composedAcceptsLookup, orderedComposedAcceptScopes[differFrom],
                        UserInquiryAcceptStatus.WaitingForApprove);
                }
            }

            newAccepts.AddRange(orderedComposedAcceptScopes
                .Skip(differFrom)
                .SelectMany(x => composedAcceptsLookup[x]));

            var savedScopes = orderedInquiryAcceptScopes.Skip(differFrom)
                .Intersect(orderedComposedAcceptScopes.Skip(differFrom));

            var inquiryApproversToNotifyAboutOrderChange = savedScopes
                .SelectMany(scope => inquiryAcceptsLookup[scope])
                .Where(x => x.Status == UserInquiryAcceptStatus.Accepted)
                .Select(x => new InquiryUserNotifyDto
                {
                    AcceptUserId = x.AcceptUserId,
                    UserInquiryId = inquiry.Id
                });

            var inquiryApproverCurrentTurnToNotify = newAccepts
                .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove)
                .Select(x => new InquiryUserNotifyDto
                {
                    AcceptUserId = x.AcceptUserId,
                    UserInquiryId = inquiry.Id,
                    CreatedForUserId = inquiry.UserId
                });
            
            approversToNotifyAboutOrderChange.AddRange(inquiryApproversToNotifyAboutOrderChange);
            approversCurrentTurnToNotify.AddRange(inquiryApproverCurrentTurnToNotify);
            inquiry.UserInquiryAccepts = newAccepts;
        }

        _inquiriesDbContext.UserInquiryAccepts.RemoveRange(acceptsToRemove);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        return (approversToNotifyAboutOrderChange, approversCurrentTurnToNotify);
    }

    private static IEnumerable<UserInquiryAcceptEntity> CopyUserInquiryAccepts(
        IEnumerable<UserInquiryAcceptEntity> userInquiryAccepts)
    {
        foreach (var userInquiryAccept in userInquiryAccepts)
        {
            yield return new UserInquiryAcceptEntity
            {
                AcceptedById = userInquiryAccept.AcceptedById,
                AcceptedAtUtc = userInquiryAccept.AcceptedAtUtc,
                AcceptUserId = userInquiryAccept.AcceptUserId,
                RejectedById = userInquiryAccept.RejectedById,
                RejectedAtUtc = userInquiryAccept.RejectedAtUtc,
                AcceptPolicyScope = userInquiryAccept.AcceptPolicyScope,
                AcceptUserPosition = userInquiryAccept.AcceptUserPosition,
                Status = userInquiryAccept.Status,
                UserInquiryId = userInquiryAccept.UserInquiryId,
            };
        }
    }


    private static int DifferFromIndex(AcceptScopeUserHelper[] orderedInquiryAcceptScopes,
        AcceptScopeUserHelper[] orderedComposedAcceptScopes)
    {
        for (var i = 0; i < Math.Min(orderedInquiryAcceptScopes.Length, orderedComposedAcceptScopes.Length); i++)
        {
            var scopeBefore = orderedInquiryAcceptScopes[i];
            var composedScope = orderedComposedAcceptScopes[i];

            if (!scopeBefore.Equals(composedScope))
            {
                return i;
            }
        }

        return Math.Min(orderedInquiryAcceptScopes.Length, orderedComposedAcceptScopes.Length);
    }

    private static void SetScopeStatus(ILookup<AcceptScopeUserHelper, UserInquiryAcceptEntity> acceptsLookup,
        AcceptScopeUserHelper scope, UserInquiryAcceptStatus status)
    {
        foreach (var accept in acceptsLookup[scope])
        {
            accept.Status = status;
        }
    }
}
