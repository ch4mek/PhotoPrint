using MassTransit;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.InquiryAccepts.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserSyncEventConsumer : IConsumer<UserSyncEvent>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IUpdateUserAsApproverAcceptResolver _updateUserAsApproverAcceptResolver;
    private readonly IUpdateUserAsInquiryForAcceptResolver _updateUserAsInquiryForAcceptResolver;

    public UserSyncEventConsumer(
        IInquiriesDbContext inquiriesDbContext,
        IConnectionFactory connectionFactory,
        IUpdateUserAsApproverAcceptResolver updateUserAsApproverAcceptResolver,
        IUpdateUserAsInquiryForAcceptResolver updateUserAsInquiryForAcceptResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _connectionFactory = connectionFactory;
        _updateUserAsApproverAcceptResolver = updateUserAsApproverAcceptResolver;
        _updateUserAsInquiryForAcceptResolver = updateUserAsInquiryForAcceptResolver;
    }

    public async Task Consume(ConsumeContext<UserSyncEvent> context)
    {
        var @event = context.Message;

        if (@event.Position.IsIntern())
        {
            return;
        }

        var user = await _inquiriesDbContext.InquiriesUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user is null)
        {
            user = new InquiriesUserEntity
            {
                UserId = @event.UserId,
                FirstName = @event.FirstName,
                LastName = @event.LastName,
                Email = @event.Email
            };

            _inquiriesDbContext.InquiriesUsers.Add(user);
        }

        var resolverRequest = new UpdateUserAcceptResolverRequest
        {
            User = user,
            PrevDepartmentId = user.DepartmentId,
            PrevPosition = user.Position,
            PrevResourceGroupId = user.ResourceGroupId,
            IsPrevActive = user.IsActive
        };

        user.FirstName = @event.FirstName;
        user.LastName = @event.LastName;
        user.Email = @event.Email;
        user.DepartmentId = @event.DepartmentId!;
        user.RegisteredLocationId = @event.RegisteredLocationId;
        user.AvatarFileId = @event.AvatarFileId;
        user.Color = @event.Color;
        user.Position = @event.Position;
        user.PositionOrderWeight = @event.PositionOrderWeight;
        user.ResourceGroupId = @event.ResourceGroupId;
        user.IsActive = @event.IsActive;
        user.HireDate = @event.HireDate;
        user.FirstNameRu = @event.FirstNameRu;
        user.LastNameRu = @event.LastNameRu;
        user.PatronymicRu = @event.PatronymicRu;

        await _inquiriesDbContext.SaveChangesAsync(context.CancellationToken);

        await _updateUserAsInquiryForAcceptResolver.ResolveAsync(resolverRequest, context.CancellationToken);
        await _updateUserAsApproverAcceptResolver.ResolveAsync(resolverRequest, context.CancellationToken);

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-inquiries-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
