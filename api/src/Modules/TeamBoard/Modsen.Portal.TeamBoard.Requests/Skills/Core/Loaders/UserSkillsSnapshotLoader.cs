using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;

internal interface IUserSkillsSnapshotLoader
{
    Task<UserSnapshotCoreDto?> Load(int entryId, CancellationToken cancellationToken);
    Task<UserSnapshotCoreDto[]> Load(int[] entryIds, CancellationToken cancellationToken);
}

internal class UserSkillsSnapshotLoader : IUserSkillsSnapshotLoader
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UserSkillsSnapshotLoader(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<UserSnapshotCoreDto?> Load(int entryId, CancellationToken cancellationToken)
    {
        var entries = await Load([entryId], cancellationToken);
        return entries.FirstOrDefault();
    }

    public async Task<UserSnapshotCoreDto[]> Load(int[] entryIds, CancellationToken cancellationToken)
    {
        var entries = await _teamBoardDbContext.UserSkillsSnapshots
            .AsNoTracking()
            .Where(x => entryIds.Contains(x.Id))
            .Where(x => !x.IsDeleted)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SpecializationTemplate)
                    .ThenInclude(st => st!.UserSpecializations)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SpecializationTemplate)
                    .ThenInclude(st => st!.SpecializationTemplateHardSkills)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.HardSkills)
                    .ThenInclude(hs => hs.HardSkill)
                        .ThenInclude(st => st!.SpecializationTemplateHardSkills)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SeniorityLevel)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.HardSkills)
                    .ThenInclude(hs => hs.SeniorityLevel)
            .AsSplitQuery()
            .ToArrayAsync(cancellationToken);

        return entries
            .Select(x => new UserSnapshotCoreDto
            {
                Id = x.Id,
                UserId = x.UserId,
                SnapshotDate = x.SnapshotDate,
                UserSpecializations = x.Specializations
                .Select(s => new UserSnapshotSpecializationCoreDto
                {
                    Id = s.Id,

                    CreatedAtUtc = s.SpecializationTemplate!.UserSpecializations
                        .Where(e => e.SpecializationTemplateId == s.SpecializationTemplateId && e.UserId == x.UserId)
                        .OrderByDescending(e => e.IsPrimary)
                        .ThenByDescending(e => e.CreatedAtUtc)
                        .FirstOrDefault()
                        !.CreatedAtUtc,

                    IsPrimary = s.SpecializationTemplate.UserSpecializations
                        .Where(e => e.SpecializationTemplateId == s.SpecializationTemplateId && e.UserId == x.UserId)
                        .OrderByDescending(e => e.IsPrimary)
                        .ThenByDescending(e => e.CreatedAtUtc)
                        .FirstOrDefault()
                        !.IsPrimary,

                    SeniorityLevelId = s.SeniorityLevelId,
                    SeniorityLevelName = s.SeniorityLevel?.InternalName,
                    WasPrimary = s.UserSpecializationWasPrimary,
                    SpecializationTemplateId = s.SpecializationTemplateId,
                    SpecializationTemplateName = s.SpecializationTemplate.Name,
                    HardSkills = s.HardSkills
                        .Where(HardSkillBusinessRules.ShouldIncludeHardSkill)
                        .Select(
                        hs =>
                        {
                            var hardSkillAddedToSpecializationDateTime = s.SpecializationTemplate
                                .SpecializationTemplateHardSkills
                                .FirstOrDefault(spec => spec.HardSkillId == hs.HardSkillId)?
                                .CreatedAtUtc;

                            return new HardSkillDto
                            {
                                Id = hs.HardSkillId,
                                Name = hs.HardSkill!.Name,
                                SeniorityLevelId = hs.SeniorityLevelId,
                                SeniorityLevelName = hs.SeniorityLevel?.InternalName,
                                AddedAtUtc = hardSkillAddedToSpecializationDateTime,
                                IsActive = HardSkillBusinessRules.IsActiveHardSkill(hs)
                            };
                        })
                    .OrderByDescending(hardSkill => hardSkill.AddedAtUtc)
                    .ThenByDescending(hardSkill => hardSkill.Id)
                    .ToArray(),
                })
                .OrderByDescending(s => s.IsPrimary)
                    .ThenByDescending(s => s.CreatedAtUtc)
                .ToArray(),
            }).ToArray();
    }
}
