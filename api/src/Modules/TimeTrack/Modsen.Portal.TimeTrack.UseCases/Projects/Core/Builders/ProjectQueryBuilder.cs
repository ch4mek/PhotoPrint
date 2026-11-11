using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TimeTrack.UseCases.Projects.Core.Builders;

internal interface IProjectQueryBuilder
{
    IProjectQueryBuilder WithKeyWord(string? keyWord);

    IProjectQueryBuilder WithActiveStatus(bool? isActive);

    IProjectQueryBuilder WithPublicStatus(bool? isPublic);

    IProjectQueryBuilder WithManagerId(int? managerId);

    IQueryable<Project> Build();
}

internal class ProjectQueryBuilder(ITimeTrackDbContext timeTrackDbContext) : IProjectQueryBuilder
{
    private string? _keyWord;
    private bool? _isActive;
    private bool? _isPublic;
    private int? _managerId;

    public IProjectQueryBuilder WithKeyWord(string? keyWord)
    {
        _keyWord = keyWord;

        return this;
    }

    public IProjectQueryBuilder WithActiveStatus(bool? isActive)
    {
        _isActive = isActive;

        return this;
    }

    public IProjectQueryBuilder WithPublicStatus(bool? isPublic)
    {
        _isPublic = isPublic;

        return this;
    }

    public IProjectQueryBuilder WithManagerId(int? managerId)
    {
        _managerId = managerId;

        return this;
    }

    public IQueryable<Project> Build()
    {
        var query = timeTrackDbContext.Projects
            .AsNoTracking();

        query = FilterByParams(query);

        return query;
    }

    private IQueryable<Project> FilterByParams(IQueryable<Project> projects)
    {
        if (_keyWord is not null)
        {
            projects = projects.Where(p => EF.Functions.ILike(p.Name, $"{_keyWord}%"));
        }

        if (_isPublic.HasValue)
        {
            projects = projects.Where(p => p.IsPublic == _isPublic);
        }

        if (_isActive.HasValue)
        {
            projects = projects.Where(p => p.IsActive == _isActive);
        }

        if (_managerId.HasValue)
        {
            projects = projects.Where(p => p.ManagerUserId == _managerId);
        }

        return projects;
    }
}
