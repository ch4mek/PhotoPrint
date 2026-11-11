using Modsen.Portal.Core.Api.Context;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.DeleteSpecialization;

public class DeleteSpecializationTemplateRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new Mock<AuthenticatedUserContext> { CallBase = true }.As<IAuthenticatedUserContext>();
    private readonly Mock<ISpecializationNotAssignedToAnyUserBusinessRule> _specializationNotAssignedToAnyUserBusinessRule = new();
    private readonly Mock<IDeleteSpecializationTemplateHardSkillsDbQuery> _deleteSpecializationTemplateHardSkillsDbQueryMock = new();
    private readonly Mock<IGetSpecializationTemplateByIdDbQuery> _getSpecializationTemplateDbQuery = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly DeleteSpecializationTemplateRequest _handler;

    public DeleteSpecializationTemplateRequestTests()
    {
        _handler = new DeleteSpecializationTemplateRequest(_authenticatedUserContextMock.Object,
            _specializationNotAssignedToAnyUserBusinessRule.Object,
            _deleteSpecializationTemplateHardSkillsDbQueryMock.Object,
            _getSpecializationTemplateDbQuery.Object,
            _teamboardDbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenSpecializationTemplateDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var specializationTemplateId = 100;
        var command = new DeleteSpecializationTemplateRequest.Command(specializationTemplateId);

        _getSpecializationTemplateDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(specializationTemplateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SpecializationTemplate?)null);

        // Act
        var action = async () => await _handler.Handle(command, _token);

        // Assert

        await action.Should()
            .ThrowAsync<EntityNotFoundException>($"Specialization by id {specializationTemplateId} is not found");
    }

    [Fact]
    public async Task HandleWhenHeadFromAnotherDepartmentShouldThrowActionNotAllowedException()
    {
        // Arrange
        var specializationTemplateId = 100;
        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(1);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns("Net");

        var command = new DeleteSpecializationTemplateRequest.Command(specializationTemplateId);

        _getSpecializationTemplateDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(specializationTemplateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpecializationTemplate
            {
                CreatedById = 2,
                Name = "test",
                SpecializationTemplateDepartments =
                [
                    new SpecializationTemplateDepartment { DepartmentId = "Java" }
                ]
            });

        // Act
        var action = async () => await _handler.Handle(command, _token);

        // Assert

        await action.Should()
            .ThrowAsync<ActionNotAllowedException>("Specialization hasn't been created by current user or your department" +
                " is not related to specialization departments, you don't have permissions to perform this action");
    }

    [Fact]
    public async Task HandleWhenSpecializationTemplateAssignedToUserShouldThrowActionNotAllowedException()
    {
        // Arrange
        var specializationTemplateId = 1;
        var departmentId = "Net";
        var userId = 1;

        _authenticatedUserContextMock.Setup(context => context.UserId).Returns(userId);
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock.Setup(context => context.DepartmentId).Returns(departmentId);

        _getSpecializationTemplateDbQuery
            .Setup(dbQuery => dbQuery.ExecuteAsync(specializationTemplateId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SpecializationTemplate
            {
                Name = "test",
                CreatedById = userId
            });

        _specializationNotAssignedToAnyUserBusinessRule
            .Setup(rule => rule.IsCorrespondsToBusiness(specializationTemplateId, _token))
            .ReturnsAsync(true);

        var command = new DeleteSpecializationTemplateRequest.Command(specializationTemplateId);

        // Act
        var action = async () => await _handler.Handle(command, _token);

        // Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>("You can't delete specialization that is assigned to a user");
    }
}
