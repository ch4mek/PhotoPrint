using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Composers;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Helpers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IUpdateUserInfoInquiryAcceptsResolver
{
    Task ResolveAsync(int[] userInquiryIds, CancellationToken cancellationToken);
}

internal class UpdateUserInfoInquiryAcceptsResolver : IUpdateUserInfoInquiryAcceptsResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IInquiryAcceptsComposer _inquiryAcceptsComposer;
    private readonly IInquiryAcceptedStatusResolver _inquiryAcceptedStatusResolver;
    private readonly IInquiryRejectedStatusResolver _inquiryRejectedStatusResolver;

    public UpdateUserInfoInquiryAcceptsResolver(IInquiriesDbContext inquiriesDbContext,
        IInquiryAcceptsComposer inquiryAcceptsComposer, IInquiryAcceptedStatusResolver inquiryAcceptedStatusResolver,
        IInquiryRejectedStatusResolver inquiryRejectedStatusResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _inquiryAcceptsComposer = inquiryAcceptsComposer;
        _inquiryAcceptedStatusResolver = inquiryAcceptedStatusResolver;
        _inquiryRejectedStatusResolver = inquiryRejectedStatusResolver;
    }
    public async Task ResolveAsync(int[] userInquiryIds, CancellationToken cancellationToken)
    {
        var inquiries = _inquiriesDbContext.UserInquiries
            .Include(x => x.InquiryTemplate)
                .ThenInclude(x => x!.InquiryTemplateAcceptPolicyPolicies)
            .Include(x => x.UserInquiryAccepts.OrderBy(a => a.Id))
            .Where(x => userInquiryIds.Contains(x.Id))
            .AsSplitQuery();

        var acceptsToRemove = inquiries.SelectMany(x => x.UserInquiryAccepts);

        foreach (var inquiry in inquiries)
        {
            var orderedPolicyScopes = inquiry.InquiryTemplate!.InquiryTemplateAcceptPolicyPolicies
                .OrderBy(x => x.OrderWeight)
                .Select(x => new AcceptScopeUserHelper(x.Scope, x.UserId));

            var composedAccepts = await _inquiryAcceptsComposer.ComposeAsync(inquiry.UserId, inquiry.InquiryTemplateId,
                cancellationToken);

            var composedAcceptsLookup = composedAccepts.ToLookup(accept =>
                new AcceptScopeUserHelper(accept.AcceptPolicyScope, accept.AcceptUserId));

            var inquiryAcceptsLookup = inquiry.UserInquiryAccepts.ToLookup(accept =>
                new AcceptScopeUserHelper(accept.AcceptPolicyScope, accept.AcceptUserId));

            var allScopes = composedAcceptsLookup.Select(x => x.Key)
                .Union(inquiryAcceptsLookup.Select(x => x.Key));

            var compareAcceptMap = allScopes.Select(scope => new
            {
                Scope = scope,
                Before = inquiryAcceptsLookup[scope],
                After = composedAcceptsLookup[scope],
            }).ToDictionary(x => x.Scope);

            IEnumerable<UserInquiryAcceptEntity> newAccepts = [];

            var isAllAccepted = true;
            foreach (var scope in orderedPolicyScopes)
            {
                if (compareAcceptMap.TryGetValue(scope, out var compareMapEntry))
                {
                    var composeResult = ComposeScopeAccepts(scope, compareMapEntry.Before,
                        compareMapEntry.After, isAllAccepted);

                    if (composeResult.IsAllAccepted is not null //can be null if there is no accept for the scope
                        && composeResult.IsAllAccepted.Value && !isAllAccepted)
                    {
                        newAccepts = newAccepts.Where(x => x.Status == UserInquiryAcceptStatus.Accepted);
                    }

                    isAllAccepted = composeResult.IsAllAccepted ?? isAllAccepted; //don't change isAllAccepted if composeResult.IsAllAccepted is null
                    newAccepts = newAccepts.Concat(composeResult.ResultAccepts);
                }
            }

            inquiry.UserInquiryAccepts = newAccepts.ToList();

            if (inquiry.UserInquiryAccepts.Count == 0)
            {
                //there are no acceptsers for this inquiry
                await _inquiryRejectedStatusResolver.ResolveAsync(inquiry, cancellationToken);
                continue;
            }

            if (isAllAccepted)
            {
                await _inquiryAcceptedStatusResolver.ResolveAsync(inquiry, cancellationToken);
            }
        }

        _inquiriesDbContext.UserInquiryAccepts.RemoveRange(acceptsToRemove);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
    }

    private static (IEnumerable<UserInquiryAcceptEntity> ResultAccepts, bool? IsAllAccepted) ComposeScopeAccepts(
        AcceptScopeUserHelper scope, IEnumerable<UserInquiryAcceptEntity> acceptsBefore,
        IEnumerable<UserInquiryAcceptEntity> acceptsAfter, bool isPrevAllAccepted)
    {
        if (acceptsBefore.Select(accept => accept.Status == UserInquiryAcceptStatus.Accepted)
            .DefaultIfEmpty(false)
            .All(accepted => accepted))
        {
            return (CopyUserInquiryAccepts(acceptsBefore), true);
        }

        if (isPrevAllAccepted)
        {
            foreach(var accept in acceptsAfter)
            {
                accept.Status = UserInquiryAcceptStatus.WaitingForApprove;
            }
        }

        var composedScopes = scope.Scope == InquiryAcceptPolicyScopes.ProjectManager
            ? CopyUserInquiryAccepts(acceptsBefore
                    .Where(accept => accept.Status == UserInquiryAcceptStatus.Accepted))
                .Concat(acceptsAfter)
                .DistinctBy(accept => accept.AcceptUserId)
            : acceptsAfter;

        var acceptsAccepted = composedScopes.Select(accept => accept.Status == UserInquiryAcceptStatus.Accepted).ToList();

        var isAllAccepted = (acceptsAccepted is not null && acceptsAccepted.Count != 0)
            ? (bool ?)acceptsAccepted.All(accepted => accepted)
            : null;        

        return (composedScopes, isAllAccepted);
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
}
