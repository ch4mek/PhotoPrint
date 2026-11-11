using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.CreateSpecialization;

public class CreateSpecializationTemplateRequestTests
{
    private readonly Mock<ICanUserUpdateSpecializationTemplateBusinessRule> _canTemplateBeUpdatedBusinessRule = new();
    private readonly Mock<IIsSpecializationTemplateNameUniqueBusinessRule> _isTemplateNameUniqueBusinessRule = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<ITeamBoardDbContext> _teamboardDbContextMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateSpecializationTemplateRequest _handler;

    public CreateSpecializationTemplateRequestTests()
    {
        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.HeadOfDepartment]);
        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Net");

        _handler = new CreateSpecializationTemplateRequest(
            _authenticatedUserContextMock.Object,
            _canTemplateBeUpdatedBusinessRule.Object,
            _isTemplateNameUniqueBusinessRule.Object,
            _teamboardDbContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task HandleWhenHeadFromAnotherDepartmentThrowActionNotAllowedException()
    {
        //Arrange
        var specializationName = "Name";

        var departmentIds = new List<string>{ "Java" };
        var hardSkillIds = new List<int>();
        var requestDto = new CreateSpecializationTemplateDto(specializationName, departmentIds, hardSkillIds);

        var command = new CreateSpecializationTemplateRequest.Command(requestDto);

        //Act
        var response = async () => await _handler.Handle(command, _cancellationToken);

        //Assert
        await response.Should().ThrowAsync<ActionNotAllowedException>("You can not create specialization with this department");
    }

    [Fact]
    public async Task HandleWhenNameIsNotUniqueThrowEntityConflictException()
    {
        //Arrange
        var specializationName = "Name";

        var departmentIds = new List<string>{ "Net" };
        var hardSkillIds = new List<int>();
        var requestDto = new CreateSpecializationTemplateDto(specializationName, departmentIds, hardSkillIds);

        var command = new CreateSpecializationTemplateRequest.Command(requestDto);

        _isTemplateNameUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(specializationName, null, _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var response = async () => await _handler.Handle(command, _cancellationToken);

        //Assert
        await response.Should().ThrowAsync<EntityConflictException>("Specialization with the same name already exists");
    }

    [Fact]
    public async Task HandleWhenSomeHardSkillsNotExistThrowEntityNotFoundException()
    {
        //Arrange
        var specializationName = "Name";

        var departmentIds = new List<string>{ "Net" };
        var hardSkillIds = new List<int>(){ 2, 4, 6, 1 };
        var requestDto = new CreateSpecializationTemplateDto(specializationName, departmentIds, hardSkillIds);

        var command = new CreateSpecializationTemplateRequest.Command(requestDto);

        _isTemplateNameUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(specializationName, null, _cancellationToken))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(x => x.HardSkills)
            .Returns(new List<HardSkill>()
                {
                    new() {  Name = "test", HardSkillGroupId = string.Empty, Id = 1 },
                    new() {  Name = "test", HardSkillGroupId = string.Empty, Id = 2 },
                    new() {  Name = "test", HardSkillGroupId = string.Empty, Id = 4 }
                }.BuildMockDbSet().Object);

        //Act
        var response = async () => await _handler.Handle(command, _cancellationToken);

        //Assert
        await response.Should().ThrowAsync<EntityNotFoundException>("A hard skill or some hard skills don't exist");
    }

    [Fact]
    public async Task HandleWhenRequestValidReturnName()
    {
        //Arrange
        var specializationName = "Name";
        var canBeUpdated = true;

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.Cto]);

        var departmentIds = new List<string>();
        var hardSkillIds = new List<int>();
        var requestDto = new CreateSpecializationTemplateDto(specializationName, departmentIds, hardSkillIds);

        var command = new CreateSpecializationTemplateRequest.Command(requestDto);

        _isTemplateNameUniqueBusinessRule
            .Setup(x => x.IsCorrespondsToBusiness(specializationName, null, _cancellationToken))
            .ReturnsAsync(true);

        _teamboardDbContextMock.Setup(
            x => x.SpecializationTemplates.AddAsync(It.IsAny<SpecializationTemplate>(), _cancellationToken));
        _teamboardDbContextMock.Setup(x => x.SaveChangesAsync(_cancellationToken));

        _clockServiceMock.Setup(x => x.UtcNow).Returns(DateTime.UtcNow);

        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(1);

        _canTemplateBeUpdatedBusinessRule.Setup(
            x => x.IsCorrespondsToBusiness(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(canBeUpdated);

        //Act
        var response =  await _handler.Handle(command, _cancellationToken);

        //Assert
        response.Name.Should().Be(specializationName);
        response.CanBeUpdated.Should().Be(canBeUpdated);
    }
}
