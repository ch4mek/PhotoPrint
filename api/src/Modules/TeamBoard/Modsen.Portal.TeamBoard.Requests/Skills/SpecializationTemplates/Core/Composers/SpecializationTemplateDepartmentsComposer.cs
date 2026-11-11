using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.Composers;

internal interface ISpecializationTemplateDepartmentsComposer
{
    Task<string[]> ComposeAsync(int templateId, string[] newDepartmentsIds, CancellationToken cancellationToken);
}

internal sealed class SpecializationTemplateDepartmentsComposer : ISpecializationTemplateDepartmentsComposer
{
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly IClockService _clockService;

    public SpecializationTemplateDepartmentsComposer(IAuthenticatedUserContext authenticatedUserContext,
        ITeamBoardDbContext teamboardDbContext,
        IClockService clockService)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _teamboardDbContext = teamboardDbContext;
        _clockService = clockService;
    }

    public async Task<string[]> ComposeAsync(int templateId, string[] newDepartmentsIds, CancellationToken cancellationToken)
    {
        var templateDepartments = newDepartmentsIds.Select(departmentId =>
            new SpecializationTemplateDepartment
            {
                DepartmentId = departmentId,
                SpecializationTemplateId = templateId,
                CreatedAtUtc = _clockService.UtcNow,
                CreatedById = _authenticatedUserContext.GetRequiredUserId()
            }).ToList();

        await _teamboardDbContext.SpecializationTemplateDepartments.AddRangeAsync(templateDepartments, cancellationToken);
        await _teamboardDbContext.SaveChangesAsync(cancellationToken);

        return templateDepartments.Select(department => department.DepartmentId).ToArray();
    }
}
