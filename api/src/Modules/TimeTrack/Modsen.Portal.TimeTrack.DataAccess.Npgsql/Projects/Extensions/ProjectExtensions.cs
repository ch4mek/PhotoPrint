using Modsen.Portal.Core;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.DataAccess.Npgsql.Projects.Extensions;

internal static class ProjectExtensions
{
    public static string GetProjectType(this Project project)
    {
        return project.IsPublic
            ? PortalProjectTypeNames.Public
            : PortalProjectTypeNames.Private;
    }
}
