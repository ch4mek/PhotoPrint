using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingProjectsToTeamBoardMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly TeamBoardDbContext _teamBoardDbContext;

    public PublishExistingProjectsToTeamBoardMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        TeamBoardDbContext teamBoardDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _teamBoardDbContext = teamBoardDbContext;
    }

    public string TaskName => "PublishExistingProjectsToTeamBoard";


    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyProject;

        do
        {
            var timeTrackProjects = _timeTrackDbContext.Projects
                .Include(project => project.Client)
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var projects = new List<TeamBoardProject>();

            page++;

            foreach (var timeTrackProject in timeTrackProjects)
            {
                var project = _teamBoardDbContext.TeamBoardProjects.FirstOrDefault(x => x.Id == timeTrackProject.Id);

                if (project == null)
                {
                    project = new TeamBoardProject { Id = timeTrackProject.Id, ProjectName = "test", ClientName = "test", };

                    CopyFields(timeTrackProject, project);
                    _teamBoardDbContext.Add(project);
                }
                else
                {
                    CopyFields(timeTrackProject, project);
                }

                projects.Add(project);
            }

            _teamBoardDbContext.SaveChanges();

            foreach (var project in projects)
            {
                _teamBoardDbContext.TeamBoardProjects.Entry(project).State = EntityState.Detached;
            }

            hasAnyProject = _timeTrackDbContext.Projects
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyProject);
    }

    private static void CopyFields(Project timeTrackProject, TeamBoardProject teamBoardProject)
    {
        teamBoardProject.ProjectName = timeTrackProject.Name;
        teamBoardProject.ClientName = timeTrackProject.Client!.Name;
        teamBoardProject.IsActive = timeTrackProject.IsActive;
        teamBoardProject.Color = timeTrackProject.Color;
    }
}
