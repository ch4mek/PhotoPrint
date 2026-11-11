using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.Core;

public class IsSpecializationNotAssignedToAnyUserBusinessRuleTests
{
    private readonly Mock<IDepartmentsSpecializationAssignmentsDbQuery> _departmentsSpecializationAssignmentsDbQuery = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly IsSpecializationNotAssignedToAnyUserBusinessRule _businessRule;

    public IsSpecializationNotAssignedToAnyUserBusinessRuleTests()
    {
        _businessRule = new IsSpecializationNotAssignedToAnyUserBusinessRule(
                _departmentsSpecializationAssignmentsDbQuery.Object);
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenNotUpdatedDepartmentIdsAreEmptyReturnsTrue()
    {
        //Arrange
        var notUpdatedDepartmentIds = Array.Empty<string>();

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            templateId: 1,
            notUpdatedDepartmentIds: notUpdatedDepartmentIds,
            _token);

        //Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSpecializationIsAssignedToUserReturnsFalse()
    {
        //Arrange
        var specializationTemplateId = 1;
        var notUpdatedDepartmentIds = new[]
        {
            "Net",
            "Java"
        };

        _departmentsSpecializationAssignmentsDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(specializationTemplateId, notUpdatedDepartmentIds, _token))
            .ReturnsAsync(
            [
                new DepartmentSpecializationAssignment("Java", true),
                new DepartmentSpecializationAssignment("Net", false)
            ]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            templateId: specializationTemplateId,
            notUpdatedDepartmentIds: notUpdatedDepartmentIds,
            _token);

        //Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsCorrespondsToBusinessWhenSpecializationIsNotAssignedToAnyUserReturnsTrue()
    {
        //Arrange
        var specializationTemplateId = 1;
        var notUpdatedDepartmentIds = new[]
        {
            "Net",
            "Java"
        };

        _departmentsSpecializationAssignmentsDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(specializationTemplateId, notUpdatedDepartmentIds, _token))
            .ReturnsAsync(
            [
                new DepartmentSpecializationAssignment("Java", false),
                new DepartmentSpecializationAssignment("Net", false)
            ]);

        //Act
        var result = await _businessRule.IsCorrespondsToBusiness(
            templateId: specializationTemplateId,
            notUpdatedDepartmentIds: notUpdatedDepartmentIds,
            _token);

        //Assert
        result.Should().BeTrue();
    }
}
