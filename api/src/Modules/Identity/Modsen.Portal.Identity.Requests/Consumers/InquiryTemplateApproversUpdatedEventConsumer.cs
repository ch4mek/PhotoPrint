using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Inquiries.Contract.Events;

namespace Modsen.Portal.Identity.Requests.Consumers;

internal sealed class InquiryTemplateApproversUpdatedEventConsumer : IConsumer<InquiryTemplateApproversUpdatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IAuthenticationTokenContext _authenticationTokenContext;

    public InquiryTemplateApproversUpdatedEventConsumer(
        IConnectionFactory connectionFactory,
        IIdentityDbContext identityDbContext,
        IAuthenticationTokenContext authenticationTokenContext)
    {
        _connectionFactory = connectionFactory;
        _identityDbContext = identityDbContext;
        _authenticationTokenContext = authenticationTokenContext;
    }

    public async Task Consume(ConsumeContext<InquiryTemplateApproversUpdatedEvent> context)
    {
        var @event = context.Message;
        var claimTypeName = RequestsClaimTypeNames.ManageRequests.Name;

        var usersWithClaimsIds = await _identityDbContext.UserClaims
            .AsNoTracking()
            .Where(claim =>
                claim.ClaimType == claimTypeName &&
                @event.ApproversIds.Contains(claim.UserId))
            .Select(claim => claim.UserId)
            .ToListAsync(context.CancellationToken);

        var approverIds = @event.ApproversIds.ToList();

        approverIds.RemoveAll(id => usersWithClaimsIds.Contains(id));

        foreach (var approverId in approverIds)
        {
            _identityDbContext.UserClaims.Add(new UserClaim
            {
                ClaimValue = "any",
                UserId = approverId,
                ClaimType = claimTypeName
            });
        }

        await _identityDbContext.SaveChanges(context.CancellationToken);

        foreach (var approverId in approverIds)
        {
            await _authenticationTokenContext.RevokeAllUserTokens(
                approverId,
                RevokeReasons.ClaimsChanged,
                context.CancellationToken);
        }

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class InquiryTemplateApproversUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<InquiryTemplateApproversUpdatedEventConsumer>("inquiry-template-approvers-updated-identity-queue", "inquiry-template-approvers-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        InquiryTemplateApproversUpdatedEventConsumer,
        InquiryTemplateApproversUpdatedEventConsumerDefinition>;
}
