using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IGetSubstituteResolver
{    
    Task<int?> ResolveAsync(int templateId, int userIdInquiryFor, InquiryAcceptPolicyScopes mainScope,
        int? acceptUserId, CancellationToken cancellationToken);
    Task<int?> ResolveAsync(InquiryTemplateAcceptPolicyEntity? policy, int userIdInquiryFor, CancellationToken cancellationToken);
}

internal class GetSubstituteResolver : IGetSubstituteResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IIdentityContract _identityContract;

    public GetSubstituteResolver(IInquiriesDbContext inquiriesDbContext, IIdentityContract identityContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _identityContract = identityContract;
    }

    public async Task<int?> ResolveAsync(int templateId, int userIdInquiryFor, InquiryAcceptPolicyScopes mainScope,
        int? acceptUserId, CancellationToken cancellationToken)
    {        
        var inquiryPolicy = await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .Where(x => x.InquiryTemplateId == templateId)
            .Where(x => x.Scope == mainScope
                && (mainScope == InquiryAcceptPolicyScopes.User && x.UserId.HasValue && x.UserId == acceptUserId
                    || mainScope != InquiryAcceptPolicyScopes.User))
            .FirstOrDefaultAsync(cancellationToken);

        return await ResolveAsync(inquiryPolicy, userIdInquiryFor, cancellationToken);
    }

    public async Task<int?> ResolveAsync(InquiryTemplateAcceptPolicyEntity? policy, int userIdInquiryFor, CancellationToken cancellationToken)
    {
        return policy?.SubstituteScope switch
        {
            InquiryAcceptPolicyScopes.User =>
                policy.SubstituteUserId,
            InquiryAcceptPolicyScopes.GroupManager =>
                await _identityContract.GetUserGroupManagerId(userIdInquiryFor, cancellationToken),
            InquiryAcceptPolicyScopes.HeadOfDepartment =>
                await _identityContract.GetUserHeadId(userIdInquiryFor, cancellationToken),
            InquiryAcceptPolicyScopes.CEO | InquiryAcceptPolicyScopes.CTO =>
                await _identityContract.GetUserChiefIdAsync(userIdInquiryFor, cancellationToken),
            _ => null,
        };
    }
}
