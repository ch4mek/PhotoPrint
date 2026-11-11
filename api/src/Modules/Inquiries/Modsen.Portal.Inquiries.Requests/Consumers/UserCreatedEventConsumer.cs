using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IUpdateUserAsApproverAcceptResolver _updateUserAsApproverAcceptResolver;
    private readonly IAssignDefaultInquiryTemplatesResolver _assignDefaultInquiryTemplatesComposer;
    public UserCreatedEventConsumer(
        IInquiriesDbContext inquiriesDbContext,
        IConnectionFactory connectionFactory,
        IUpdateUserAsApproverAcceptResolver updateUserAsApproverAcceptResolver,
        IAssignDefaultInquiryTemplatesResolver assignDefaultInquiryTemplatesComposer)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _connectionFactory = connectionFactory;
        _updateUserAsApproverAcceptResolver = updateUserAsApproverAcceptResolver;
        _assignDefaultInquiryTemplatesComposer = assignDefaultInquiryTemplatesComposer;
    }

    public async Task Consume(ConsumeContext<UserCreatedEvent> context)
    {
        var @event = context.Message;

        if (@event.Position.IsIntern())
        {
            return;
        }

        var user = await _inquiriesDbContext.InquiriesUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user == null)
        {
            user = new InquiriesUserEntity
            {
                UserId = @event.UserId,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                Email = @event.Email,
                DepartmentId = @event.DepartmentId,
                AvatarFileId = @event.AvatarFileId,
                Position = @event.Position,
                PositionOrderWeight = @event.PositionOrderWeight,
                ResourceGroupId = @event.ResourceGroupId,
                Color = @event.Color,
                IsActive = true,
                HireDate = @event.HireDate,
                FirstNameRu = @event.FirstNameRu,
                LastNameRu = @event.LastNameRu,
                PatronymicRu = @event.PatronymicRu,
                RegisteredLocationId = @event.RegisteredLocationId
            };

            _inquiriesDbContext.InquiriesUsers.Add(user);

            await _inquiriesDbContext.SaveChangesAsync(context.CancellationToken);

            await _updateUserAsApproverAcceptResolver.ResolveAsync(new UpdateUserAcceptResolverRequest
            {
                User = user,
                PrevDepartmentId = null,
                PrevPosition = null,
                PrevResourceGroupId = null,
                IsPrevActive = false
            }, context.CancellationToken);

            // create reset transaction because created user have contract
            await _assignDefaultInquiryTemplatesComposer
                .ComposeWithAccrualsAsync(
                user.UserId,
                @event.ContractDateFrom!.Value,
                @event.ContractDateTo,
                context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
        }
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-inquiries-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
