using Modsen.Portal.Core.Outbox;
using Modsen.Portal.Core.Utils.Modules;
using Modsen.Portal.Identity.Contract.Events;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Contract.Events;
using Modsen.Portal.Notification.Contract.Events;
using Modsen.Portal.Storage.Contract.Events;
using Modsen.Portal.Storage.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Contract.Events;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.Contract.Events;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;

namespace Modsen.Portal.Api.Modules;

internal sealed class OutboxEventModule : RegistrationModule
{
    public override void Load(IServiceCollection services)
    {
        RegisterEventsToStoreInTimeTrack(services);
        RegisterEventsToStoreInIdentity(services);
        RegisterEventsToStoreInTeamBoard(services);
        RegisterEventsToStoreInStorage(services);
    }

    private static void RegisterEventsToStoreInTimeTrack(IServiceCollection services)
    {
        services
            .HandleEventViaOutbox<ClientCreatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ClientUpdatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ProjectCreatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ProjectDeactivatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ProjectUpdatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ProjectUserCreatedEvent, TimeTrackDbContext>()
            .HandleEventViaOutbox<ProjectUserUpdatedEvent, TimeTrackDbContext>();
    }

    private static void RegisterEventsToStoreInIdentity(IServiceCollection services)
    {
        services
            .HandleEventViaOutbox<NotificationsCreatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<DevelopmentPlanDeletedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<DeleteFileEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<DepartmentSyncEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserActivatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserCreatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserDeactivatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserLocationUpdatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserSyncEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserUpdatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<UserRegisteredLocationUpdatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<ResourceGroupCreatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<ResourceGroupUpdatedEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<ResourceGroupSyncEvent, IdentityModuleDbContext>()
            .HandleEventViaOutbox<InquiryTemplateApproversUpdatedEvent, IdentityModuleDbContext>();
    }

    private static void RegisterEventsToStoreInTeamBoard(IServiceCollection services)
    {
        services
            .HandleEventViaOutbox<ContractCreatedEvent, TeamBoardDbContext>()
            .HandleEventViaOutbox<ContractUpdatedEvent, TeamBoardDbContext>();
    }

    private static void RegisterEventsToStoreInStorage(IServiceCollection services)
    {
        services
            .HandleEventViaOutbox<ApplicationFileUploadedEvent, StorageDbContext>();
    }
}
