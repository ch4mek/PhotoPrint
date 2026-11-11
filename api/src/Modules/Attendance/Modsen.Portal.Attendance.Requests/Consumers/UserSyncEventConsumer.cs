using Modsen.Portal.Attendance.DataAccess.Npgsql;
using Modsen.Portal.Attendance.DataAccess.Npgsql.Entities;
using Modsen.Portal.Core.Communication.ExchangeDefinition;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.Contract.Events;

namespace Modsen.Portal.Attendance.Requests.Consumers;

internal class UserSyncEventConsumer : UserEventConsumerBase<UserSyncEvent>
{
    public UserSyncEventConsumer(IConnectionFactory connectionFactory, AttendanceDbContext dbContext)
        : base(connectionFactory, dbContext)
    {
    }

    protected override int GetUserId(UserSyncEvent @event)
    {
        return @event.UserId;
    }

    protected override string? GetUserPosition(UserSyncEvent @event)
    {
        return @event.Position;
    }

    protected override UserEntity CreateUser(UserSyncEvent userSyncEvent)
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

    protected override void UpdateUser(UserEntity user, UserSyncEvent userSyncEvent)
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

    internal sealed class UserSyncEventConsumerDefinition() :
        ConsumerDefinitionBase<UserSyncEventConsumer>("user-sync-attendance-queue", "user-sync-exchange");

    internal sealed class RegistrationDefinition : ConsumerConfigurationRegistration<
        UserSyncEventConsumer,
        UserSyncEventConsumerDefinition>;
}
