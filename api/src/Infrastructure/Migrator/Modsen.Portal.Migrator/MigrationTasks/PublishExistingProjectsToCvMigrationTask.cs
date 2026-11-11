using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal sealed class PublishExistingProjectsToCvMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly CvDbContext _cvDbContext;

    public PublishExistingProjectsToCvMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        CvDbContext cvDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _cvDbContext = cvDbContext;
    }

    public string TaskName => "PublishExistingProjectsToCv";

    public void Execute()
    {
        var page = 0;
        var size = MigratorTaskConstants.BatchSize;
        bool hasAnyProject;

        do
        {
            var timeTrackProjects = _timeTrackDbContext.Projects
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Take(size)
                .ToList();

            var projects = new List<CvProjectEntity>();

            page++;

            foreach (var timeTrackProject in timeTrackProjects)
            {
                var project = _cvDbContext.CvProjects
                    .FirstOrDefault(x => x.ProjectId == timeTrackProject.Id.ToString(CultureInfo.InvariantCulture));

                if (project == null)
                {
                    project = new CvProjectEntity
                    {
                        ProjectId = timeTrackProject.Id.ToString(CultureInfo.InvariantCulture),
                        Name = timeTrackProject.Name
                    };

                    CopyFields(timeTrackProject, project);
                    _cvDbContext.Add(project);
                }
                else
                {
                    CopyFields(timeTrackProject, project);
                }

                projects.Add(project);
            }

            _cvDbContext.SaveChanges();

            foreach (var project in projects)
            {
                _cvDbContext.CvProjects.Entry(project).State = EntityState.Detached;
            }

            hasAnyProject = _timeTrackDbContext.Projects
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyProject);
    }

    private static void CopyFields(Project timeTrackProject, CvProjectEntity cvProject)
    {
        cvProject.Name = timeTrackProject.Name;
        cvProject.IsActive = timeTrackProject.IsActive;
    }
}
