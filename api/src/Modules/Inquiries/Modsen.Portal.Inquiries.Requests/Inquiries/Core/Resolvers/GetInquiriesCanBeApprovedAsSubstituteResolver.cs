using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

internal interface IGetInquiriesCanBeApprovedAsSubstituteResolver
{
    Task<List<int>> ResolveAsync(bool onlyAsAnyUser, CancellationToken cancellationToken);
    Task<List<int>> ResolveAsync(ICollection<int> inquiryIds, CancellationToken cancellationToken);
}

internal class GetInquiriesCanBeApprovedAsSubstituteResolver : IGetInquiriesCanBeApprovedAsSubstituteResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IIdentityContract _identityContract;
    private readonly IGetAbsentApproversResolver _absentApproversResolver;


    public GetInquiriesCanBeApprovedAsSubstituteResolver(IInquiriesDbContext inquiriesDbContext,
        IAuthenticatedUserContext authenticatedUserContext,
        IIdentityContract identityContract,
        IGetAbsentApproversResolver getAbsentApproversResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _authenticatedUserContext = authenticatedUserContext;
        _identityContract = identityContract;
        _absentApproversResolver = getAbsentApproversResolver;
    }

    public async Task<List<int>> ResolveAsync(ICollection<int> inquiryIds, CancellationToken cancellationToken)
    {
        var inquiryIdsCanBeApprovedAsSubstitute = await ResolveAsync(onlyAsAnyUser: false, cancellationToken);
        return inquiryIds.Intersect(inquiryIdsCanBeApprovedAsSubstitute).ToList();
    }

    public async Task<List<int>> ResolveAsync(bool onlyAsAnyUser, CancellationToken cancellationToken)
    {
        var absentUsers = await _absentApproversResolver.ResolveAsync(approverIds: null, cancellationToken);

        if (absentUsers.Count == 0)
        {
            return [];
        }

        var inquiriesWhereMainApproverIsAbsent = await _inquiriesDbContext.UserInquiryAccepts
            .AsNoTracking()
            .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove)
            .Select(x => new
            {
                x.AcceptUserId,
                MainScope = x.AcceptPolicyScope,
                UserInquiryId = x.UserInquiryId!.Value,
                x.UserInquiryAgreementAssign!.InquiryTemplateId,
                x.UserInquiryAgreementAssign.UserId
            })
            .Where(g => absentUsers.Contains(g.AcceptUserId))
            .ToListAsync(cancellationToken);

        if (inquiriesWhereMainApproverIsAbsent.Count == 0)
        {
            return [];
        }

        var substitutePoliciesForInquiry = (await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .AsNoTracking()
            .ToListAsync(cancellationToken))
            .Join(inquiriesWhereMainApproverIsAbsent,
                p => new
                {
                    MainScope = p.Scope,
                    p.InquiryTemplateId,
                    p.UserId
                },
                i => new
                {
                    i.MainScope,
                    i.InquiryTemplateId,
                    UserId = i.MainScope == InquiryAcceptPolicyScopes.User ? i.AcceptUserId : (int?)null
                },
                (p, i) => new
                {
                    i.UserInquiryId,
                    i.UserId,
                    p.SubstituteScope,
                    p.SubstituteUserId,
                })
            .ToList();

        var inquiryIdsCanBeApprovedAsAnyUserSubstitute = substitutePoliciesForInquiry
            .Where(x => x.SubstituteUserId == _authenticatedUserContext.UserId)
            .Select(x => x.UserInquiryId);

        var inquiriesWhereSubstituteIsNotAnyUser = substitutePoliciesForInquiry
            .Where(x => x.SubstituteUserId == null)
            .ToArray();

        var userIdsInquiriesFor = inquiriesWhereSubstituteIsNotAnyUser
            .Select(x => x.UserId)
            .ToArray();

        if (!onlyAsAnyUser && userIdsInquiriesFor.Length > 0)
        {
            var userWithManagers = await _identityContract.GetUsersWithManagersAsync(userIdsInquiriesFor, cancellationToken);
            var inquiriesCanBeApprovedByUser = inquiriesWhereSubstituteIsNotAnyUser
                .Join(userWithManagers,
                    pi => pi.UserId,
                    u => u.UserId,
                    (pi, u) => new
                    {
                        pi.UserInquiryId,
                        pi.SubstituteScope,
                        u.UserId,
                        u.HeadOfDepartmentUserId,
                        u.GroupManagerId
                    })
                .Where(x =>
                    x.SubstituteScope == InquiryAcceptPolicyScopes.GroupManager
                        && x.GroupManagerId == _authenticatedUserContext.UserId
                    || x.SubstituteScope == InquiryAcceptPolicyScopes.HeadOfDepartment
                        && x.HeadOfDepartmentUserId == _authenticatedUserContext.UserId)
                .Select(x => x.UserInquiryId)
                .ToList();

            inquiriesCanBeApprovedByUser.AddRange(inquiryIdsCanBeApprovedAsAnyUserSubstitute);
            return inquiriesCanBeApprovedByUser;
        }

        return inquiryIdsCanBeApprovedAsAnyUserSubstitute.ToList();
    }
}
