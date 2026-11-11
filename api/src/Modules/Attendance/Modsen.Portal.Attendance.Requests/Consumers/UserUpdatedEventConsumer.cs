using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal class UserUpdatedEventConsumer : UserEventConsumerBase<UserUpdatedEvent>
{
    public UserUpdatedEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext)
    {
    }

    protected override int GetUserId(UserUpdatedEvent @event)
    {
        return @event.UserId;
    }

    protected override string? GetUserPosition(UserUpdatedEvent @event)
    {
        return @event.Position;
    }

    protected override UserEntity CreateUser(UserUpdatedEvent userSyncEvent)
    {
        return new UserEntity
        {
            UserId = userSyncEvent.UserId,
            FirstName = userSyncEvent.FirstName,
            LastName = userSyncEvent.LastName,
            FirstNameRu = userSyncEvent.FirstNameRu,
            LastNameRu = userSyncEvent.LastNameRu,
            Email = userSyncEvent.Email,
            DepartmentId = userSyncEvent.DepartmentId,
            ResourceGroupId = userSyncEvent.ResourceGroupId,
            Position = userSyncEvent.Position
        };
    }

    protected override void UpdateUser(UserEntity user, UserUpdatedEvent userSyncEvent)
    {
        user.FirstName = userSyncEvent.FirstName;
        user.LastName = userSyncEvent.LastName;
        user.FirstNameRu = userSyncEvent.FirstNameRu;
        user.LastNameRu = userSyncEvent.LastNameRu;
        user.Email = userSyncEvent.Email;
        user.DepartmentId = userSyncEvent.DepartmentId;
        user.ResourceGroupId = userSyncEvent.ResourceGroupId;
        user.Position = userSyncEvent.Position;
    }

    internal sealed class UserUpdatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserUpdatedEventConsumer>("user-updated-attendance-queue", "user-updated-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserUpdatedEventConsumer,
        UserUpdatedEventConsumerDefinition>;
}
