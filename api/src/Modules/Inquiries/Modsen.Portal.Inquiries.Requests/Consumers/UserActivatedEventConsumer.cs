using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserActivatedEventConsumer : IConsumer<UserActivatedEvent>
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly IInquiriesDbContext _dbContext;
    private readonly IUpdateUserAsApproverAcceptResolver _updateUserAsApproverAcceptResolver;
    private readonly IUnassignUserFromInquiryTemplatesResolver _unassignUserFromInquiryTemplatesResolver;
    private readonly IAssignDefaultInquiryTemplatesResolver _assignDefaultInquiryTemplatesComposer;

    public UserActivatedEventConsumer(
        IConnectionFactory connectionFactory,
        IInquiriesDbContext dbContext,
        IUpdateUserAsApproverAcceptResolver updateUserAsApproverAcceptResolver,
        IUnassignUserFromInquiryTemplatesResolver unassignUserFromInquiryTemplatesResolver,
        IAssignDefaultInquiryTemplatesResolver assignDefaultInquiryTemplatesComposer)
    {
        _connectionFactory = connectionFactory;
        _dbContext = dbContext;
        _updateUserAsApproverAcceptResolver = updateUserAsApproverAcceptResolver;
        _unassignUserFromInquiryTemplatesResolver = unassignUserFromInquiryTemplatesResolver;
        _assignDefaultInquiryTemplatesComposer = assignDefaultInquiryTemplatesComposer;
    }

    public async Task Consume(ConsumeContext<UserActivatedEvent> context)
    {
        var @event = context.Message;

        var user = await _dbContext.InquiriesUsers.FirstOrDefaultAsync(
            user => user.UserId == @event.UserId,
            context.CancellationToken);

        if (user != null)
        {
            user.IsActive = true;

            await _dbContext.SaveChangesAsync(context.CancellationToken);

            await _updateUserAsApproverAcceptResolver.ResolveAsync(new UpdateUserAcceptResolverRequest
            {
                User = user,
                PrevDepartmentId = null,
                PrevPosition = null,
                PrevResourceGroupId = null,
                IsPrevActive = false
            }, context.CancellationToken);

            await _unassignUserFromInquiryTemplatesResolver.ResolveAsync(user.UserId, @event.ActivatedById, context.CancellationToken);
            await _assignDefaultInquiryTemplatesComposer.ComposeWithAccrualsAsync(user.UserId, context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserActivatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserActivatedEventConsumer>("user-activated-inquiries-queue", "user-activated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserActivatedEventConsumer,
        UserActivatedEventConsumerDefinition>;
}
