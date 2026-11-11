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
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Consumers;

internal sealed class UserUpdatedEventConsumer : IConsumer<UserUpdatedEvent>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IConnectionFactory _connectionFactory;
    private readonly IUpdateUserAsInquiryForAcceptResolver _updateUserAsInquiryForAcceptResolver;
    private readonly IUpdateUserAsApproverAcceptResolver _updateUserAsApproverAcceptResolver;
    private readonly IAssignDefaultInquiryTemplatesResolver _assignDefaultInquiryTemplatesComposer;
    private readonly ITeamBoardContract _teamBoardContract;

    public UserUpdatedEventConsumer(
        IInquiriesDbContext inquiriesDbContext,
        IConnectionFactory connectionFactory,
        IUpdateUserAsInquiryForAcceptResolver updateUserAsInquiryForAcceptResolver,
        IUpdateUserAsApproverAcceptResolver updateUserAsApproverAcceptResolver,
        IAssignDefaultInquiryTemplatesResolver assignDefaultInquiryTemplatesResolver,
        ITeamBoardContract teamBoardContract)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _connectionFactory = connectionFactory;
        _updateUserAsInquiryForAcceptResolver = updateUserAsInquiryForAcceptResolver;
        _updateUserAsApproverAcceptResolver = updateUserAsApproverAcceptResolver;
        _assignDefaultInquiryTemplatesComposer = assignDefaultInquiryTemplatesResolver;
        _teamBoardContract = teamBoardContract;
    }

    public async Task Consume(ConsumeContext<UserUpdatedEvent> context)
    {
        var @event = context.Message;

        if (@event.Position.IsIntern())
        {
            return;
        }

        var user = await _inquiriesDbContext.InquiriesUsers
            .Where(x => x.UserId == @event.UserId)
            .FirstOrDefaultAsync(context.CancellationToken);

        if (user != null)
        {
            var resolverRequest = new UpdateUserAcceptResolverRequest
            {
                User = user,
                PrevPosition = user.Position,
                PrevDepartmentId = user.DepartmentId,
                PrevResourceGroupId = user.ResourceGroupId,
                IsPrevActive = user.IsActive,
            };

            CreateOrUpdate(user, @event);

            await _inquiriesDbContext.SaveChangesAsync(context.CancellationToken);

            await _updateUserAsInquiryForAcceptResolver.ResolveAsync(resolverRequest, context.CancellationToken);
            await _updateUserAsApproverAcceptResolver.ResolveAsync(resolverRequest, context.CancellationToken);

            await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);

            return;
        }

        // case: Intern -> other role
        user = CreateOrUpdate(user, @event);

        _inquiriesDbContext.InquiriesUsers.Add(user!);

        await _inquiriesDbContext.SaveChangesAsync(context.CancellationToken);

        await _updateUserAsApproverAcceptResolver.ResolveAsync(new UpdateUserAcceptResolverRequest
        {
            User = user!,
            PrevDepartmentId = null,
            PrevPosition = null,
            PrevResourceGroupId = null,
            IsPrevActive = false
        }, context.CancellationToken);

        var contracts = await _teamBoardContract.GetUsersActiveContractDates(null, context.CancellationToken);
        var userContract = contracts.FirstOrDefault(x => x.UserId == user!.UserId);

        if (userContract != null)
        {
            //create reset transaction because created user have contract
            await _assignDefaultInquiryTemplatesComposer
                .ComposeWithAccrualsAsync(
                user!.UserId,
                userContract.ContractStartDate!.Value,
                userContract.ContractEndDate,
                context.CancellationToken);
        }

        await _connectionFactory.CommitCurrentTransaction(context.CancellationToken);
    }

    private static InquiriesUserEntity CreateOrUpdate(InquiriesUserEntity? user, UserUpdatedEvent @event)
    {
        if (user is null)
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
        }
        else
        {
            user.FirstName = @event.FirstName;
            user.LastName = @event.LastName;
            user.Email = @event.Email;
            user.DepartmentId = @event.DepartmentId;
            user.ResourceGroupId = @event.ResourceGroupId;
            user.AvatarFileId = @event.AvatarFileId;
            user.Color = @event.Color;
            user.Position = @event.Position;
            user.PositionOrderWeight = @event.PositionOrderWeight;
            user.RegisteredLocationId = @event.RegisteredLocationId;
            user.HireDate = @event.HireDate;
            user.FirstNameRu = @event.FirstNameRu;
            user.LastNameRu = @event.LastNameRu;
            user.PatronymicRu = @event.PatronymicRu;
        }

        return user;
    }

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-inquiries-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
