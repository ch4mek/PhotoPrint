using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Core.CoreDto;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Loaders;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSpecializations.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSpecialization.CreateUserSpecialization;

public class CreateUserSpecializationRequestTests
{
    private readonly Mock<ITeamBoardDbContext> _teamBoardDbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<ISpecializationSnapshotComposer> _snapshotComposerMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IAreAllHardSkillsListedBusinessRule> _areAllHardSkillsListedBusinessRuleMock = new();
    private readonly Mock<IUserSpecializationLoader> _userSpecializationLoaderMock = new();
    private readonly Mock<IClockService> _clockServiceMock = new();
    private readonly Mock<IHardSkillsExistBusinessRule> _hardSkillsExistBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateUserSpecializationRequest _handler;

    public CreateUserSpecializationRequestTests()
    {
        _handler = new CreateUserSpecializationRequest(
            _teamBoardDbContextMock.Object,
            _snapshotComposerMock.Object,
            _authenticatedUserContextMock.Object,
            _clockServiceMock.Object,
            _areAllHardSkillsListedBusinessRuleMock.Object,
            _userSpecializationLoaderMock.Object,
            _hardSkillsExistBusinessRuleMock.Object,
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserCantBeRetrievedShouldThrowActionNotAllowedException()
    {
        //Arrange
        var command = new CreateUserSpecializationRequest.Command(
            UserId: 1,
            SpecializationTemplateId: 1,
            SpecializationSeniorityLevelId: "Junior",
            HardSkills: []);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(command.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var action = async () => await _handler.Handle(command, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<ActionNotAllowedException>();

    }

    [Fact]
    public async Task HandleWhenNotAllHardSkillsListedThrowsEntityConflictException()
    {
        //Arrange
        var command = new CreateUserSpecializationRequest.Command(
            UserId: 1,
            SpecializationTemplateId: 1,
            SpecializationSeniorityLevelId: "Junior",
            HardSkills: []);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(command.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _areAllHardSkillsListedBusinessRuleMock
            .Setup(rule => rule.IsCorrespondsToBusiness(
                    It.IsAny<int>(),
                    Array.Empty<HardSkillSeniorityDto>(),
                    _cancellationToken))
            .ReturnsAsync(false);

        //Act
        var action = async () => await _handler.Handle(command, _cancellationToken);

        //Assert
        await action.Should().ThrowAsync<EntityConflictException>("Not all hard skills are listed in request");

    }

    [Theory]
    [InlineData("Junior")]
    [InlineData("Senior")]
    public async Task HandleWhenSpecializationSeniorityLevelIdIsNotSigned_Exception(string? specializationSeniorityLevelId)
    {
        //Arrange
        var command = new CreateUserSpecializationRequest.Command(
            UserId: 1,
            SpecializationTemplateId: 1,
            SpecializationSeniorityLevelId: specializationSeniorityLevelId!,
            HardSkills: []
        );

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(command.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _areAllHardSkillsListedBusinessRuleMock.Setup(x => x.IsCorrespondsToBusiness(
            It.IsAny<int>(),
            It.IsAny<IReadOnlyCollection<HardSkillSeniorityDto>>(),
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        var seniorityLevelsIds = new[] { "Junior", "JuniorL1", "JuniorL2", "JuniorL3", "Middle", "None", "Senior" };
        _teamBoardDbContextMock
            .Setup(x => x.SeniorityLevels)
            .Returns(seniorityLevelsIds
                .Select(id => new SeniorityLevel
                {
                    Id = id,
                    Description = id,
                    ExternalName = id,
                    InternalName = id,
                    ShortName = id
                })
                .ToArray().BuildMockDbSet().Object
            );

        //Act
        var action = () => _handler.Handle(command, _cancellationToken);

        //Assert
        await action
            .Should()
            .ThrowExactlyAsync<InvalidDataAppException>("Specialization seniority level without sign can't be used");
    }
}
