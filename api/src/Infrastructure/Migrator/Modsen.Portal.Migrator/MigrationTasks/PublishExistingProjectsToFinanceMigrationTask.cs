using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Database;
using Modsen.Portal.Finance.DataAccess.Npgsql;
using Modsen.Portal.Finance.DataAccess.Npgsql.Entities;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Migrator.MigrationTasks;

internal class PublishExistingProjectsToFinanceMigrationTask : IMigrationTask
{
    private readonly TimeTrackDbContext _timeTrackDbContext;
    private readonly FinanceDbContext _financeDbContext;

    public PublishExistingProjectsToFinanceMigrationTask(
        TimeTrackDbContext timeTrackDbContext,
        FinanceDbContext financeDbContext)
    {
        _timeTrackDbContext = timeTrackDbContext;
        _financeDbContext = financeDbContext;
    }

    public string TaskName => "PublishExistingProjectsToFinance";

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

            var projects = new List<FinanceProjectEntity>();

            page++;

            foreach (var timeTrackProject in timeTrackProjects)
            {
                var project = _financeDbContext.FinanceProjects
                    .FirstOrDefault(x => x.Id == timeTrackProject.Id);

                if (project == null)
                {
                    project = new FinanceProjectEntity
                    {
                        Id = timeTrackProject.Id,
                        ProjectName = timeTrackProject.Name,
                        ClientId = timeTrackProject.ClientId,
                    };

                    CopyFields(timeTrackProject, project);
                    _financeDbContext.Add(project);
                }
                else
                {
                    CopyFields(timeTrackProject, project);
                }

                projects.Add(project);
            }

            _financeDbContext.SaveChanges();

            foreach (var project in projects)
            {
                _financeDbContext.FinanceProjects.Entry(project).State = EntityState.Detached;
            }

            hasAnyProject = _timeTrackDbContext.Projects
                .OrderBy(x => x.Id)
                .Skip(page * size)
                .Any();

        } while (hasAnyProject);
    }

    private static void CopyFields(Project timeTrackProject, FinanceProjectEntity financeProject)
    {
        financeProject.ProjectName = timeTrackProject.Name;
        financeProject.ClientId = timeTrackProject.ClientId;
        financeProject.Color = timeTrackProject.Color;
        financeProject.IsActive = timeTrackProject.IsActive;
        financeProject.IsPublic = timeTrackProject.IsPublic;
    }
}

