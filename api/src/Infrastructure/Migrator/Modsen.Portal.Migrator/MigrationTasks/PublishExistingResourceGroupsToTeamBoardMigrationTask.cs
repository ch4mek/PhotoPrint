using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingResourceGroupsToTeamBoardMigrationTask : IMigrationTask
{
    private readonly IdentityModuleDbContext _identityModuleDbContext;
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public PublishExistingResourceGroupsToTeamBoardMigrationTask(
        IdentityModuleDbContext identityModuleDbContext,
        TeamBoardDbContext teamBoardDbContext)
    {
        _identityModuleDbContext = identityModuleDbContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingResourceGroupsToTeamBoard";

    public void Execute()
    {
        var groups = _identityModuleDbContext.ResourceGroups.ToArray();
        var groupIds = new List<int>();

        foreach (var group in groups)
        {
            var teamboardResourceGroup = _teamBoardDbContext.TeamBoardResourceGroups
                .FirstOrDefault(x => x.ResourceGroupId == group.Id);
            if (teamboardResourceGroup == null)
            {
                teamboardResourceGroup = new TeamBoardResourceGroup
                {
                    ResourceGroupId = group.Id,
                    ResourceGroupName = group.Name,
                    IsActive = group.IsActive
                };
                _teamBoardDbContext.Add(teamboardResourceGroup);
            }
            else
            {
                teamboardResourceGroup.IsActive = group.IsActive;
                teamboardResourceGroup.ResourceGroupName = group.Name;
                _teamBoardDbContext.Update(teamboardResourceGroup);
            }

            groupIds.Add(group.Id);
        }

        _teamBoardDbContext.TeamBoardResourceGroups
            .Where(g => !groupIds.Contains(g.ResourceGroupId))
            .ExecuteDelete();
        _teamBoardDbContext.SaveChanges();
    }
}
