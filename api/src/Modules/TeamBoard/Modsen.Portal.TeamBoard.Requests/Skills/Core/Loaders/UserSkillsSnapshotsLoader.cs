using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSnapshots.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;

internal interface IUserSkillsSnapshotsLoader
{
    Task<IEnumerable<UserSnapshotCoreDto>> Load(int userId, CancellationToken cancellationToken);
}

internal class UserSkillsSnapshotsLoader : IUserSkillsSnapshotsLoader
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public UserSkillsSnapshotsLoader(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<IEnumerable<UserSnapshotCoreDto>> Load(int userId, CancellationToken cancellationToken)
    {
        var userSnapshots = (await _teamBoardDbContext.UserSkillsSnapshots
            .AsNoTracking()
            .AsSplitQuery()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.SnapshotDate)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SpecializationTemplate)
                    .ThenInclude(st => st!.UserSpecializations)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SpecializationTemplate)
                    .ThenInclude(st => st!.SpecializationTemplateHardSkills)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.HardSkills)
                    .ThenInclude(hs => hs.HardSkill)
                        .ThenInclude(hs => hs!.SpecializationTemplateHardSkills)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.SeniorityLevel)
            .Include(x => x.Specializations)
                .ThenInclude(s => s.HardSkills)
                    .ThenInclude(hs => hs.SeniorityLevel)
            .AsSplitQuery()
            .ToListAsync(cancellationToken))
            .Select(s => new UserSnapshotCoreDto
            {
                Id = s.Id,
                UserId = s.UserId,
                SnapshotDate = s.SnapshotDate,
                UserSpecializations = s.Specializations
                    .Select(sp => new UserSnapshotSpecializationCoreDto
                    {
                        Id = sp.Id,
                        SeniorityLevelId = sp.SeniorityLevelId,
                        SeniorityLevelName = sp.SeniorityLevel!.InternalName,
                        SpecializationTemplateId = sp.SpecializationTemplateId,
                        SpecializationTemplateName = sp.SpecializationTemplate!.Name,
                        WasPrimary = sp.UserSpecializationWasPrimary,
                        HardSkills = sp.HardSkills
                            .Where(HardSkillBusinessRules.ShouldIncludeHardSkill)
                            .Select(
                            x =>
                            {
                                var hardSkillAddedToSpecializationDateTime = sp.SpecializationTemplate
                                    .SpecializationTemplateHardSkills
                                    .FirstOrDefault(spec => spec.HardSkillId == x.HardSkillId)?
                                    .CreatedAtUtc;

                                return new HardSkillDto
                                {
                                    Id = x.HardSkillId,
                                    Name = x.HardSkill!.Name,
                                    SeniorityLevelId = sp.HardSkills
                                            .Where(hs => hs.HardSkillId == x.HardSkillId)
                                            .Select(hs => hs.SeniorityLevelId)
                                            .FirstOrDefault(),
                                    SeniorityLevelName = sp.HardSkills
                                            .Where(hs => hs.HardSkillId == x.HardSkillId)
                                            .Select(hs => hs.SeniorityLevel?.InternalName)
                                            .FirstOrDefault(),
                                    AddedAtUtc = hardSkillAddedToSpecializationDateTime,
                                    IsActive = HardSkillBusinessRules.IsActiveHardSkill(x)
                                };
                            })
                            .Where(hs => hs.IsActive == true || (hs.IsActive == false && hs.SeniorityLevelId != null))
                        .OrderByDescending(hardSkill => hardSkill.IsActive)
                        .ThenByDescending(hardSkill => hardSkill.AddedAtUtc)
                        .ThenByDescending(hardSkill => hardSkill.Id)
                        .ToArray()

                    }).ToArray()
            });

            return userSnapshots;
    }
}
