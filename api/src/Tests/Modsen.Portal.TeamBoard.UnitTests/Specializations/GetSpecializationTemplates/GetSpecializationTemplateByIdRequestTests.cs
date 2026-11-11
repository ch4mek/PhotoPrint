using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.GetSpecializationTemplates;

public class GetSpecializationTemplateByIdRequestTests
{
    private readonly Mock<IGetSpecializationTemplateByIdDbQuery> _getSpecializationTemplateByIdDbQueryMock = new();
    private readonly Mock<IDepartmentsSpecializationAssignmentsDbQuery> _departmentsSpecializationAssignmentsDbQuery = new();
    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetSpecializationTemplateByIdRequest _handler;

    public GetSpecializationTemplateByIdRequestTests()
    {
        _handler = new GetSpecializationTemplateByIdRequest(
            _getSpecializationTemplateByIdDbQueryMock.Object,
            _departmentsSpecializationAssignmentsDbQuery.Object);
    }

    [Fact]
    public async Task HandleWhenSpecializationDoesNotExistThrowEntityNotFoundException()
    {
        // Arrange
        var id = 1;

        var query = new GetSpecializationTemplateByIdRequest.Query(id);

        _getSpecializationTemplateByIdDbQueryMock
            .Setup(x => x.ExecuteAsync(query.Id, _token))
            .ReturnsAsync((SpecializationTemplate?)null);

        // Act
        var action = () => _handler.Handle(query, _token);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>($"Specialization by id {id} is not found");
    }

    [Fact]
    public async Task HandleWhenSpecializationExistsShouldReturnSpecialization()
    {
        // Arrange
        var id = 2;
        var name = "name";
        var createdById = 1;
        var departmentIds = new[] { "Java" };
        var query = new GetSpecializationTemplateByIdRequest.Query(id);

        _getSpecializationTemplateByIdDbQueryMock.Setup(x => x.ExecuteAsync(query.Id, _token))
                .ReturnsAsync(new SpecializationTemplate()
                {
                    Id = query.Id,
                    Name = name,
                    CreatedById = createdById,
                    SpecializationTemplateHardSkills =
                    [
                        new SpecializationTemplateHardSkill
                        {
                            HardSkillId = 1,
                            HardSkill = new HardSkill
                            {
                                Name = "Hard Skill",HardSkillGroupId = "1",
                            }
                        }
                    ],
                    SpecializationTemplateDepartments =
                    [
                        new SpecializationTemplateDepartment { DepartmentId = departmentIds.First() }
                    ]
                });

        _departmentsSpecializationAssignmentsDbQuery
            .Setup(x => x.ExecuteAsync(query.Id, It.IsAny<string[]>(), _token))
            .ReturnsAsync(
            [
                new DepartmentSpecializationAssignment("Java", true)
            ]);

        // Act
        var result = await _handler.Handle(query, _token);

        // Assert
        result.Name.Should().Be(name);

        result.HardSkills.First().Name.Should().Be("Hard Skill");
        result.HardSkills.First().Id.Should().Be(1);

        result.DepartmentDtos.Should().HaveCount(1);
        result.DepartmentDtos.First().DepartmentId.Should().Be(departmentIds.First());
        result.DepartmentDtos.First().IsUserFromDepartmentAssigned.Should().Be(true);

    }
}
