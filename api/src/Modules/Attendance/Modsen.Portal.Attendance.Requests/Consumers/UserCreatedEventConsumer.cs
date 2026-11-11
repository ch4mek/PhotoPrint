using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal class UserCreatedEventConsumer : UserEventConsumerBase<UserCreatedEvent>
{
    public UserCreatedEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext)
    {
    }

    protected override int GetUserId(UserCreatedEvent @event)
    {
        return @event.UserId;
    }

    protected override string? GetUserPosition(UserCreatedEvent @event)
    {
        return @event.Position;
    }

    protected override UserEntity CreateUser(UserCreatedEvent userSyncEvent)
    {
        return new UserEntity
        {
            UserId = userSyncEvent.UserId,
            FirstName = userSyncEvent.FirstName,
            LastName = userSyncEvent.LastName,
            FirstNameRu = userSyncEvent.FirstNameRu,
            LastNameRu = userSyncEvent.LastNameRu,
            PatronymicRu = userSyncEvent.PatronymicRu,
            Email = userSyncEvent.Email,
            DepartmentId = userSyncEvent.DepartmentId,
            ResourceGroupId = userSyncEvent.ResourceGroupId,
            Position = userSyncEvent.Position,
            IsActive = true,
        };
    }

    protected override void UpdateUser(UserEntity user, UserCreatedEvent userSyncEvent)
    {
        user.FirstName = userSyncEvent.FirstName;
        user.LastName = userSyncEvent.LastName;
        user.FirstNameRu = userSyncEvent.FirstNameRu;
        user.LastNameRu = userSyncEvent.LastNameRu;
        user.PatronymicRu = userSyncEvent.PatronymicRu;
        user.Email = userSyncEvent.Email;
        user.DepartmentId = userSyncEvent.DepartmentId;
        user.ResourceGroupId = userSyncEvent.ResourceGroupId;
        user.Position = userSyncEvent.Position;
        user.IsActive = true;
    }

    internal sealed class UserCreatedEventConsumerDefinition() :
        ConsumerDefinitionBase<UserCreatedEventConsumer>("user-created-attendance-queue", "user-created-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserCreatedEventConsumer,
        UserCreatedEventConsumerDefinition>;
}
