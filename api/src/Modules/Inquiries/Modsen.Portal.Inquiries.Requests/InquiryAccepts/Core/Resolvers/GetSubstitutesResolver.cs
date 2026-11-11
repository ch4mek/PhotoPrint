using System.Collections.Frozen;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Identity.Contract.Models;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Dto;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Helpers;

namespace Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

internal interface IGetSubstitutesResolver
{
    Task<ICollection<InquiryUserNotifyDto>> ResolveAsync(ICollection<InquiryUserNotifyDto> inquiryUserNotifyDtos, CancellationToken cancellationToken);
}

internal class GetSubstitutesResolver : IGetSubstitutesResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IIdentityContract _identityContract;

    public GetSubstitutesResolver(IInquiriesDbContext inquiriesDbContext, IIdentityContract identityContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _identityContract = identityContract;
    }

    public async Task<ICollection<InquiryUserNotifyDto>> ResolveAsync(
        ICollection<InquiryUserNotifyDto> inquiryUserNotifyDtos, CancellationToken cancellationToken)
    {
        if (inquiryUserNotifyDtos.Count == 0)
        {
            return [];
        }

        var inquiryIds = inquiryUserNotifyDtos.Select(x => x.UserInquiryId);
        var approverIds = inquiryUserNotifyDtos.Select(x => x.AcceptUserId);

        var inquiries = await _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(x => inquiryIds.Contains(x.Id))
            .ToArrayAsync(cancellationToken);

        var accepts = await _inquiriesDbContext.UserInquiryAccepts
            .AsNoTracking()
            .Where(x => approverIds.Contains(x.AcceptUserId))
            .Where(x => x.Status == UserInquiryAcceptStatus.WaitingForApprove)
            .ToArrayAsync(cancellationToken);

        var inquiriesAccepts = inquiries.Join(accepts.DistinctBy(x => x.UserInquiryId),
            i => i.Id,
            a => a.UserInquiryId,
            (i, a) => new
            {
                i.Id,
                InquiryForUserId = i.UserId,
                i.InquiryTemplateId,
                a.AcceptUserId,
                MainScope = a.AcceptPolicyScope
            });

        var policies = await _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .AsNoTracking()
            .Where(p => inquiriesAccepts.Select(ia => ia.InquiryTemplateId).Contains(p.InquiryTemplateId))
            .Where(p => p.SubstituteScope.HasValue)
            .ToArrayAsync(cancellationToken);

        var inquiriesPolicies = policies
            .Join(inquiriesAccepts,
                p => new AcceptScopeUserHelper(p.Scope, p.UserId, p.InquiryTemplateId),
                ia => new AcceptScopeUserHelper(ia.MainScope, ia.AcceptUserId, ia.InquiryTemplateId),
                (p, ia) => new
                {
                    ia.Id,
                    ia.InquiryForUserId,
                    ia.AcceptUserId,
                    p.SubstituteScope,
                    p.SubstituteUserId
                })
            .ToArray();

        var userIdsFor = inquiriesPolicies.Select(ip => ip.InquiryForUserId).ToArray();

        if (userIdsFor.Length == 0)
        {
            return [];
        }

        var usersManagers = (await _identityContract.GetUsersWithManagersAsync(userIdsFor, cancellationToken))
            .ToFrozenDictionary(x => x.UserId);

        var substituteNotifyDtos = new List<InquiryUserNotifyDto>();

        foreach (var inquiryPolicy in inquiriesPolicies)
        {
            var substituteId = inquiryPolicy.SubstituteScope == InquiryAcceptPolicyScopes.User
                ? inquiryPolicy.SubstituteUserId!.Value
                : PickManagerId(usersManagers, inquiryPolicy.InquiryForUserId, inquiryPolicy.SubstituteScope!.Value);

            if (substituteId != 0)
            {
                substituteNotifyDtos.Add(new InquiryUserNotifyDto
                {
                    UserInquiryId = inquiryPolicy.Id,
                    AcceptUserId = substituteId,
                    CreatedForUserId = inquiryPolicy.InquiryForUserId,
                });
            }
        }

        return substituteNotifyDtos;
    }

    private static int PickManagerId(FrozenDictionary<int, UserWithManagersDto> usersManagers, int userIdFor,
        InquiryAcceptPolicyScopes scope)
    {
        var userManagers = usersManagers[userIdFor];

        return scope switch
        {
            InquiryAcceptPolicyScopes.GroupManager => userManagers.GroupManagerId,
            InquiryAcceptPolicyScopes.HeadOfDepartment => userManagers.HeadOfDepartmentUserId,
            InquiryAcceptPolicyScopes.CTO | InquiryAcceptPolicyScopes.CEO => userManagers.ChiefId,
            _ => 0,
        };
    }
}
