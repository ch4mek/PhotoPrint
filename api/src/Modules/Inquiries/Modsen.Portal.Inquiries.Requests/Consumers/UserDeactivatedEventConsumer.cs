using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserDeactivatedEventConsumer : IConsumer<UserDeactivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IUnassignUserFromInquiryTemplatesResolver _unassignUserFromInquiryTemplatesResolver;
    private readonly IUpdateUserAsApproverAcceptResolver _updateUserAsApproverAcceptResolver;
    private readonly IResetUserAgreementBalanceComposer _resetUserAgreementBalanceComposer;
    
    public UserDeactivatedEventConsumer(
        IConnectionFactory connectionFactory,
        IInquiriesDbContext dbContext,
        IUnassignUserFromInquiryTemplatesResolver unassignUserFromInquiryTemplatesResolver,
        IUpdateUserAsApproverAcceptResolver updateUserAsApproverAcceptResolver,
        IResetUserAgreementBalanceComposer resetUserAgreementBalanceComposer)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
        _unassignUserFromInquiryTemplatesResolver = unassignUserFromInquiryTemplatesResolver;
        _updateUserAsApproverAcceptResolver = updateUserAsApproverAcceptResolver;
        _resetUserAgreementBalanceComposer = resetUserAgreementBalanceComposer;
    }

    public async Task Consume(ConsumeContext<UserDeactivatedEvent> context)
    {
        var @event = context.Message;

        var user = await _dbContext.InquiriesUsers.FirstOrDefaultAsync(
            user => user.UserId == @event.UserId,
            context.CancellationToken);

        if (user != null)
        {
            user.IsActive = false;

            _dbContext.InquiriesUsers.Update(user);
            
            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _updateUserAsApproverAcceptResolver.ResolveAsync(new UpdateUserAcceptResolverRequest
            {
                User = user,
                IsPrevActive = true
            }, context.CancellationToken);

            await _resetUserAgreementBalanceComposer.Compose(user.UserId, context.CancellationToken);
            await _unassignUserFromInquiryTemplatesResolver.ResolveAsync(user.UserId, @event.DeactivatedById, context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserDeactivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserDeactivatedEventConsumer>("user-deactivated-inquiries-queue", "user-deactivated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserDeactivatedEventConsumer,
        UserDeactivatedEventConsumerDefinition>;
}
