using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Core;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.InquiryTemplates.Core.Resolvers;

internal interface IUpdateTemplateAcceptPoliciesResolver
{
    Task ResolveAsync(InquiryTemplateEntity template, List<UpdateInquiryTemplateAcceptPolicyDto> acceptPolicies,
        CancellationToken cancellationToken);
}

internal class UpdateTemplateAcceptPoliciesResolver : IUpdateTemplateAcceptPoliciesResolver
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUpdatePolicyInquiryAcceptsResolver _updatePolicyInquiryAcceptsResolver;
    private readonly INotificationSender _notificationSender;
    private readonly IGetSubstitutesResolver _getSubstitutesResolver;
    private readonly IGetAbsentApproversResolver _getAbsentApproversResolver;

    public UpdateTemplateAcceptPoliciesResolver(
        IInquiriesDbContext inquiriesDbContext,
        IUpdatePolicyInquiryAcceptsResolver updatePolicyInquiryAcceptsResolver,
        INotificationSender notificationSender,
        IGetSubstitutesResolver getSubstitutesResolver,
        IGetAbsentApproversResolver getAbsentApproversResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _updatePolicyInquiryAcceptsResolver = updatePolicyInquiryAcceptsResolver;
        _notificationSender = notificationSender;
        _getSubstitutesResolver = getSubstitutesResolver;
        _getAbsentApproversResolver = getAbsentApproversResolver;
    }

    public async Task ResolveAsync(InquiryTemplateEntity template,
        List<UpdateInquiryTemplateAcceptPolicyDto> acceptPolicies, CancellationToken cancellationToken)
    {
        var acceptPolicyEntities = acceptPolicies
            .Select(policy => new InquiryTemplateAcceptPolicyEntity
            {
                InquiryTemplateId = template.Id,
                OrderWeight = policy.OrderWeight,
                Scope = policy.Scope,
                UserId = policy.UserId,
                SubstituteScope = policy.SubstituteScope,
                SubstituteUserId = policy.SubstituteUserId
            })
            .OrderBy(x => x.OrderWeight);

        var arePoliciesNotUpdated = acceptPolicyEntities
            .Select(x => (x.Scope, x.UserId, x.SubstituteScope, x.SubstituteUserId))
            .SequenceEqual(template.InquiryTemplateAcceptPolicyPolicies
                .Select(y => (y.Scope, y.UserId, y.SubstituteScope, y.SubstituteUserId)));

        if (arePoliciesNotUpdated)
        {
            return;
        }

        _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .RemoveRange(template.InquiryTemplateAcceptPolicyPolicies);

        _inquiriesDbContext.InquiryTemplateAcceptPolicies
            .AddRange(acceptPolicyEntities);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);

        var inquiries = await _inquiriesDbContext.UserInquiries
            .Where(x => x.InquiryTemplateId == template.Id)
            .Where(x => x.Status == UserInquiryStatus.Open)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        var (approversToNotifyAboutOrderChange, approverCurrentTurnToNotify) = await _updatePolicyInquiryAcceptsResolver
            .ResolveAsync(inquiries, cancellationToken);

        await _notificationSender.SendInquiryOrderChangedNotification(template.Name, approversToNotifyAboutOrderChange,
            template.InquiryType, cancellationToken);

        var absentApproversIds = await _getAbsentApproversResolver.ResolveAsync(
            approverCurrentTurnToNotify.Select(x => x.AcceptUserId).ToList(), cancellationToken);
        var absentApprovers = approverCurrentTurnToNotify
            .Where(x => absentApproversIds.Contains(x.AcceptUserId))
            .ToList();
        var substituteIdsCurrentTurnToNotify = await _getSubstitutesResolver
            .ResolveAsync(absentApprovers, cancellationToken);

        approverCurrentTurnToNotify.AddRange(substituteIdsCurrentTurnToNotify);

        await _notificationSender.SendInquiryAcceptNotification(template.Name, approverCurrentTurnToNotify,
            template.InquiryType.ToString(), cancellationToken);
    }
}
