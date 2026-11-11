using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.LabBoard.Requests;

public class EditInternshipRequestTests
{
    private readonly EditInternshipRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public EditInternshipRequestTests()
    {
        _handler = new EditInternshipRequest(_canTeamBoardUserBeRetrievedBusinessRuleMock.Object, _dbContextMock.Object);

        SetupDbContext();
    }

    [Fact]
    public async Task HandleWhenUserNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new EditInternshipRequest.Command(
            1,
            new EditInternshipRequestDto(1, string.Empty, InternshipType.Primary, 1, null, null, null, DateOnly.MinValue, DateOnly.MaxValue),
            []);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenUserIsNotInternShouldThrowInvalidDataAppException()
    {
        // Arrange
        var request = new EditInternshipRequest.Command(
            1,
            new EditInternshipRequestDto(null, string.Empty, InternshipType.Primary, 1, null, null, null, DateOnly.MinValue, DateOnly.MaxValue),
            []);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var action = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<InvalidDataAppException>(PortalMessages.CannotCreateInternshipForNotIntern);
    }

    [Fact]
    public async Task HandleWhenUserIsInternShouldCreateInternship()
    {
        // Arrange
        var request = new EditInternshipRequest.Command(
            2,
            new EditInternshipRequestDto(null, "Internship", InternshipType.Primary, 1, null, null, null, DateOnly.MinValue, DateOnly.MaxValue),
            []);

        _dbContextMock.SetupGet(x => x.SpecializationTemplates)
            .Returns(new List<SpecializationTemplate>().BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.InternshipSpecializations)
            .Returns(new List<InternshipSpecialization>().BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.InternshipSpecializationHardSkills)
            .Returns(new List<InternshipSpecializationHardSkill>().BuildMockDbSet().Object);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Specialization.Should().Be("Internship");
        result.Type.Should().Be(InternshipType.Primary);
        result.EngineerStage.Should().Be(1);
        result.CompletedPoints.Should().BeNull();
        result.ApprovedPoints.Should().BeNull();
        result.FinalResult.Should().BeNull();
        result.StartDate.Should().Be(DateOnly.MinValue);
        result.PlannedEndDate.Should().Be(DateOnly.MaxValue);
        result.UserInternshipSpecializations.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenUserIsInternShouldUpdateInternship()
    {
        // Arrange
        var request = new EditInternshipRequest.Command(
            2,
            new EditInternshipRequestDto(1, "Internship", InternshipType.Primary, 1, null, null, null, DateOnly.MinValue, DateOnly.MaxValue),
            []);

        _dbContextMock.SetupGet(x => x.SpecializationTemplates)
            .Returns(new List<SpecializationTemplate>().BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.InternshipSpecializations)
            .Returns(new List<InternshipSpecialization>().BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.InternshipSpecializationHardSkills)
            .Returns(new List<InternshipSpecializationHardSkill>().BuildMockDbSet().Object);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Specialization.Should().Be("Internship");
        result.Type.Should().Be(InternshipType.Primary);
        result.EngineerStage.Should().Be(1);
        result.CompletedPoints.Should().BeNull();
        result.ApprovedPoints.Should().BeNull();
        result.FinalResult.Should().BeNull();
        result.StartDate.Should().Be(DateOnly.MinValue);
        result.PlannedEndDate.Should().Be(DateOnly.MaxValue);
        result.UserInternshipSpecializations.Should().BeEmpty();
    }

    private void SetupDbContext()
    {
        var users = new List<TeamBoardUser>()
        {
            new()
            {
                UserId = 1,
                FirstName = "FirstName1",
                LastName = "LastName1",
                FirstNameRu = "FirstNameRu1",
                Email = "Email1",
                PersonalEmail = "PersonalEmail1",
                DepartmentId = "Net",
                Position = "Employee",
                IsActive = true,
            },
            new()
            {
                UserId = 2,
                FirstName = "FirstName2",
                LastName = "LastName2",
                FirstNameRu = "FirstNameRu2",
                Email = "PersonalEmail2",
                PersonalEmail = "PersonalEmail2",
                DepartmentId = "LD",
                Position = "Intern",
                IsActive = true,
            }
        };

        var internships = new List<InternshipDetail>()
        {
            new()
            {
                Id = 1,
                UserId = 2,
                Specialization = "Test",
                EngineerStage = 1,
                ApprovedPoints = 1,
                CompletedPoints = 1,
                StartDate = DateOnly.MinValue,
                PlannedEndDate = DateOnly.MaxValue,
                Type = InternshipType.Extended
            }
        };

        _dbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);

        _dbContextMock.SetupGet(x => x.InternshipDetails)
            .Returns(internships.BuildMockDbSet().Object);
    }
}
