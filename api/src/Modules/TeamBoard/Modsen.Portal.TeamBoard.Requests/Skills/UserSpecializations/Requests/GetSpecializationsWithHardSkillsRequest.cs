using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;

internal sealed class GetSpecializationsWithHardSkillsRequest :
    IQueryHandler<GetSpecializationsWithHardSkillsRequest.Query, SpecializationTemplateWithHardSkillsDto[]>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    private CancellationToken _cancellationToken;

    public GetSpecializationsWithHardSkillsRequest(
        ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<SpecializationTemplateWithHardSkillsDto[]> Handle(Query request,
        CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        var user = await _teamBoardDbContext.TeamBoardUsers
            .FirstOrDefaultAsync(user => user.UserId == request.UserId, cancellationToken);

        if (user is null)
        {
            throw new EntityNotFoundException($"User with id {request.UserId} was not found");
        }

        var baseQuery = _teamBoardDbContext.SpecializationTemplates
            .AsNoTracking()
            .AsSplitQuery()
            .Include(x => x.SpecializationTemplateHardSkills)
                .ThenInclude(x => x.HardSkill)
            .Include(x => x.UserSpecializations)
            .AsQueryable();

        var areSpecializationsAssigned = request.AreAssigned.HasValue && request.AreAssigned.Value;

        var specializations = areSpecializationsAssigned
            ? await GetAssignedSpecializationsAsync(baseQuery, request.UserId)
            : await GetNonAssignedSpecializationsAsync(baseQuery, request.UserId, user.DepartmentId);

        var result = specializations
            .Select(SpecializationTemplateWithHardSkillsDto.Of).ToArray();

        return result;
    }

    private Task<SpecializationTemplate[]> GetAssignedSpecializationsAsync(
        IQueryable<SpecializationTemplate> baseQuery,
        int userId)
    {
        return baseQuery
            .Where(template =>
                template.UserSpecializations.Any(us => us.UserId == userId) &&
                template.IsActive)
            .Select(template => new
                {
                    Template = template,
                    SortingFlag = template.UserSpecializations
                        .Where(us => us.UserId == userId)
                        .OrderByDescending(us => us.IsPrimary)
                        .ThenByDescending(us => us.CreatedAtUtc)
                        .FirstOrDefault()
                })
            .OrderByDescending(x => x.SortingFlag!.IsPrimary)
            .ThenByDescending(x => x.SortingFlag!.CreatedAtUtc)
            .Select(x => x.Template)
            .ToArrayAsync(_cancellationToken);
    }

    private Task<SpecializationTemplate[]> GetNonAssignedSpecializationsAsync(
        IQueryable<SpecializationTemplate> baseQuery,
        int userId,
        string? departmentId)
    {
        return baseQuery
            .Where(template =>
                template.UserSpecializations.All(us => us.UserId != userId) &&
                template.SpecializationTemplateDepartments
                    .Any(templateDepartment => templateDepartment.DepartmentId == departmentId) &&
                template.IsActive)
            .OrderBy(template => template.Id)
            .ToArrayAsync(_cancellationToken);
    }

    internal sealed record Query(int UserId, bool? AreAssigned) : IQuery<SpecializationTemplateWithHardSkillsDto[]>;
}
