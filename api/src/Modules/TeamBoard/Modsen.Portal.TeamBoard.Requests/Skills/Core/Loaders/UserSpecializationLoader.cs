using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;

internal interface IUserSpecializationLoader
{
    Task<UserSpecializationCoreDto?> Load(int id, CancellationToken cancellationToken);
    Task<UserSpecializationCoreDto[]> Load(int[] ids, CancellationToken cancellationToken);
}

internal class UserSpecializationLoader : IUserSpecializationLoader
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UserSpecializationLoader(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<UserSpecializationCoreDto?> Load(int id, CancellationToken cancellationToken)
    {
        var entries = await Load([id], cancellationToken);
        return entries.FirstOrDefault();
    }

    public async Task<UserSpecializationCoreDto[]> Load(int[] ids, CancellationToken cancellationToken)
    {
        var userSpecializations = await _teamBoardDbContext.UserSpecializations
            .AsNoTracking()
            .Where(x => ids.Contains(x.Id))
            .Include(x => x.SpecializationHardSkills)
                .ThenInclude(x => x.HardSkill)
            .Include(x => x.SpecializationHardSkills)
                .ThenInclude(x => x.SeniorityLevel)
            .Include(x => x.SpecializationTemplate)
            .Include(x => x.SeniorityLevel)
            .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);

        return userSpecializations
            .Select(us => new UserSpecializationCoreDto
            {
                Id = us.Id,
                UserId = us.UserId,
                SeniorityLevelId = us.SeniorityLevelId,
                SeniorityLevelName = us.SeniorityLevel!.InternalName,
                SpecializationTemplateId = us.SpecializationTemplateId,
                SpecializationTemplateName = us.SpecializationTemplate!.Name,
                IsPrimary = us.IsPrimary,
                HardSkills = us.SpecializationHardSkills
                    .Select(hs => new HardSkillDto
                    {
                        Id = hs.HardSkillId,
                        Name = hs.HardSkill!.Name,
                        SeniorityLevelId = hs.SeniorityLevelId,
                        SeniorityLevelName = hs.SeniorityLevel?.InternalName
                    }).ToArray()
            }).ToArray();
    }
}
