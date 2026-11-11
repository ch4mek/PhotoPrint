using Microsoft.EntityFrameworkCore;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;

internal interface IGetSpecializationTemplateDepartmentsDbQuery
{
    Task<List<SpecializationTemplateDepartment>> ExecuteAsync(int specializationTemplateId,
        CancellationToken cancellationToken);
}

internal class GetSpecializationTemplateDepartmentsDbQuery : IGetSpecializationTemplateDepartmentsDbQuery
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;

    public GetSpecializationTemplateDepartmentsDbQuery(ITeamBoardDbContext teamBoardDbContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
    }

    public async Task<List<SpecializationTemplateDepartment>> ExecuteAsync(int specializationTemplateId,
        CancellationToken cancellationToken)
    {
        var departments = await _teamBoardDbContext.SpecializationTemplateDepartments
            .AsNoTracking()
            .Where(templateDepartment => templateDepartment.SpecializationTemplateId == specializationTemplateId)
            .ToListAsync(cancellationToken);

        return departments;
    }
}
