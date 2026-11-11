using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

internal interface IGetSpecializationTemplateByIdDbQuery
{
    Task<SpecializationTemplate?> ExecuteAsync(int id, CancellationToken cancellationToken);
}

internal sealed class GetSpecializationTemplateByIdDbQuery : IGetSpecializationTemplateByIdDbQuery
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetSpecializationTemplateByIdDbQuery(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SpecializationTemplate?> ExecuteAsync(int id, CancellationToken cancellationToken)
    {
        var specializationTemplate = await _dbContext.SpecializationTemplates
            .Where(template => template.Id == id && template.IsActive)
            .Select(template => new SpecializationTemplate
            {
                Id = template.Id,
                Name = template.Name,
                IsActive = template.IsActive,
                CreatedById = template.CreatedById,
                CreatedAtUtc = template.CreatedAtUtc,
                UpdatedAtUtc = template.UpdatedAtUtc,
                UpdatedById = template.UpdatedById,
                SpecializationTemplateDepartments = template.SpecializationTemplateDepartments
                    .Select(department => new SpecializationTemplateDepartment
                    {
                        DepartmentId = department.DepartmentId
                    }).ToList(),
                SpecializationTemplateHardSkills = template.SpecializationTemplateHardSkills
                    .Select(hardSkill => new SpecializationTemplateHardSkill
                    {
                        HardSkillId = hardSkill.HardSkillId,
                        HardSkill = new HardSkill
                        {
                            Name = hardSkill.HardSkill == null ? string.Empty : hardSkill.HardSkill.Name,
                            HardSkillGroupId = string.Empty
                        }
                    }).ToList()
            })
            .AsSingleQuery()
            .FirstOrDefaultAsync(cancellationToken);

        return specializationTemplate;
    }
}
