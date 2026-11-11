using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Builders;

internal interface IHardSkillQueryBuilder
{
    IHardSkillQueryBuilder WithDepartment(string? departmentId);

    IHardSkillQueryBuilder WithSpecializationTemplate(int? specializationTemplateId);

    IQueryable<HardSkill> Build();
}

internal class HardSkillQueryBuilder(ITeamBoardDbContext teamBoardDbContext) : IHardSkillQueryBuilder
{
    private int? _specializationTemplateId;
    private string? _departmentId;

    public IQueryable<HardSkill> Build()
    {
        var query = teamBoardDbContext.HardSkills
            .Where(x => x.IsActive)
            .AsNoTracking();

        query = FilterByParams(query);

        return query;
    }

    public IHardSkillQueryBuilder WithDepartment(string? departmentId)
    {
        _departmentId = departmentId;
        return this;
    }

    public IHardSkillQueryBuilder WithSpecializationTemplate(int? specializationTemplateId)
    {
        _specializationTemplateId = specializationTemplateId;
        return this;
    }

    private IQueryable<HardSkill> FilterByParams(IQueryable<HardSkill> hardSkills)
    {
        if (_departmentId is not null)
        {
            hardSkills = hardSkills.Where(hardSkill => hardSkill.SpecializationTemplateHardSkills.Any(x =>
                x.SpecializationTemplate!.SpecializationTemplateDepartments.Any(y => y.DepartmentId == _departmentId)));
        }

        if (_specializationTemplateId is not null)
        {
            hardSkills = hardSkills.Where(hardSkill =>
                hardSkill.SpecializationTemplateHardSkills.Any(x => x.SpecializationTemplateId == _specializationTemplateId));
        }

        return hardSkills;
    }
}
