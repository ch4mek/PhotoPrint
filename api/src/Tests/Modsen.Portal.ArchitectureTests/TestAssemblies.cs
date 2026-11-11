using System.Reflection;
using Modsen.Portal.Equipment.Requests;
using Modsen.Portal.Identity.Requests;
using Modsen.Portal.Inquiries.Requests;
using Modsen.Portal.KnowledgeBase.Requests;
using Modsen.Portal.Notification.UseCases;
using Modsen.Portal.Storage.Requests;
using Modsen.Portal.TeamBoard.Requests;
using Modsen.Portal.TimeTrack.UseCases;

namespace Modsen.Portal.ArchitectureTests;

internal static class TestAssemblies
{
    public static IEnumerable<Assembly> GetAssemblies()
    {
        yield return typeof(EquipmentRequestsModule).Assembly;
        yield return typeof(IdentityUseCaseModule).Assembly;
        yield return typeof(InquiriesRequestsModule).Assembly;
        yield return typeof(KnowledgeBaseRequestsModule).Assembly;
        yield return typeof(NotificationUseCasesModule).Assembly;
        yield return typeof(StorageUseCaseModule).Assembly;
        yield return typeof(TeamBoardRequestsModule).Assembly;
        yield return typeof(TimeTrackUseCasesModule).Assembly;
    }
}
