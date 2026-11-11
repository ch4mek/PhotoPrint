using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class GetSpecializationTemplateByIdRequest
    : IQueryHandler<GetSpecializationTemplateByIdRequest.Query, GetSpecializationTemplateByIdRequest.Response>
{
    private readonly IGetSpecializationTemplateByIdDbQuery _getSpecializationTemplateByIdDbQuery;
    private readonly IDepartmentsSpecializationAssignmentsDbQuery _departmentsSpecializationAssignmentsDbQuery;

    public GetSpecializationTemplateByIdRequest(IGetSpecializationTemplateByIdDbQuery getSpecializationTemplateByIdDbQuery,
        IDepartmentsSpecializationAssignmentsDbQuery departmentsSpecializationAssignmentsDbQuery)
    {
        _getSpecializationTemplateByIdDbQuery = getSpecializationTemplateByIdDbQuery;
        _departmentsSpecializationAssignmentsDbQuery = departmentsSpecializationAssignmentsDbQuery;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var template = await _getSpecializationTemplateByIdDbQuery.ExecuteAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"Specialization by id {request.Id} is not found");

        var departmentSpecializationAssignments = await _departmentsSpecializationAssignmentsDbQuery.ExecuteAsync(
            request.Id,
            template.SpecializationTemplateDepartments.Select(department => department.DepartmentId).ToArray(),
            cancellationToken);

        return new Response(
            template.Id,
            template.Name,
            DepartmentDtos: departmentSpecializationAssignments
                .Select(assignment => new SpecializationTemplateDepartmentDto
                {
                    DepartmentId = assignment.DepartmentId,
                    IsUserFromDepartmentAssigned = assignment.IsUserFromDepartmentAssigned
                }).ToList(),
            HardSkills: template.SpecializationTemplateHardSkills
                .Select(hardSkill => new SpecializationTemplateHardSkillDto
                {
                    Id = hardSkill.HardSkillId,
                    Name = hardSkill.HardSkill!.Name
                }).ToList()
            );
    }

    internal sealed record Query(int Id) : IQuery<Response>;

    internal record Response(
        int Id,
        string Name,
        List<SpecializationTemplateHardSkillDto> HardSkills,
        List<SpecializationTemplateDepartmentDto> DepartmentDtos);
}
