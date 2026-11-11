using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingResourceGroupsToTimeTrackMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly TimeTrackDbContext _timeTrackDbContext;

    public PublishExistingResourceGroupsToTimeTrackMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        TimeTrackDbContext teamBoardDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _timeTrackDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingResourceGroupsToTimeTrack";

    public void Execute()
    {
        var groups = _identityModuleDbContext.ResourceGroups.ToArray();
        var groupIds = new List<int>();

        foreach (var group in groups)
        {
            var teamboardResourceGroup = _timeTrackDbContext.TimeTrackResourceGroups
                .FirstOrDefault(x => x.ResourceGroupId == group.Id);
            if (teamboardResourceGroup == null)
            {
                teamboardResourceGroup = new TimeTrackResourceGroup
                {
                    ResourceGroupId = group.Id,
                    ResourceGroupName = group.Name,
                    IsActive = group.IsActive
                };
                _timeTrackDbContext.Add(teamboardResourceGroup);
            }
            else
            {
                teamboardResourceGroup.IsActive = group.IsActive;
                teamboardResourceGroup.ResourceGroupName = group.Name;
                _timeTrackDbContext.Update(teamboardResourceGroup);
            }

            groupIds.Add(group.Id);
        }

        _timeTrackDbContext.TimeTrackResourceGroups
            .Where(g => !groupIds.Contains(g.ResourceGroupId))
            .ExecuteDelete();
        _timeTrackDbContext.SaveChanges();
    }
}
