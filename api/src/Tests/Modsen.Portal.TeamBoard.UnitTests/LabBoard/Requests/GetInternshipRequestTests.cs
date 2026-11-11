using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.TeamBoard;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.LabBoard.Requests;

public class GetInternshipRequestTests
{
    private readonly GetInternshipRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule> _canTeamBoardUserBeRetrievedBusinessRuleMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetInternshipRequestTests()
    {
        _handler = new GetInternshipRequest(_canTeamBoardUserBeRetrievedBusinessRuleMock.Object, _dbContextMock.Object);

        SetupDbContext();
    }

    [Fact]
    public async Task HandleWhenUserNotFoundShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new GetInternshipRequest.Query(1);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var action = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await action.Should().ThrowAsync<EntityNotFoundException>(PortalMessages.UserNotFound);
    }

    [Fact]
    public async Task HandleWhenInternshipNotFoundShouldReturnNull()
    {
        // Arrange
        var request = new GetInternshipRequest.Query(1);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.SetupGet(x => x.InternshipSpecializations)
            .Returns(new List<InternshipSpecialization>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task HandleWhenInternshipExistsShouldReturnInternship()
    {
        // Arrange
        var request = new GetInternshipRequest.Query(2);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int>(), It.IsAny<AccessOptions>(), _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.SetupGet(x => x.InternshipSpecializations)
            .Returns(new List<InternshipSpecialization>().BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Specialization.Should().Be("Test");
        result.Type.Should().Be(InternshipType.Extended);
        result.EngineerStage.Should().Be(1);
        result.CompletedPoints.Should().Be(1);
        result.ApprovedPoints.Should().Be(1);
        result.FinalResult.Should().BeNull();
        result.StartDate.Should().Be(DateOnly.MinValue);
        result.PlannedEndDate.Should().Be(DateOnly.MaxValue);
        result.UserInternshipSpecializations.Should().BeEmpty();
    }

    private void SetupDbContext()
    {
        var internships = new List<InternshipDetail>()
        {
            new()
            {
                Id = 1,
                UserId = 2,
                User = new TeamBoardUser
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
                },
                Specialization = "Test",
                EngineerStage = 1,
                ApprovedPoints = 1,
                CompletedPoints = 1,
                StartDate = DateOnly.MinValue,
                PlannedEndDate = DateOnly.MaxValue,
                Type = InternshipType.Extended
            }
        };

        _dbContextMock.SetupGet(x => x.InternshipDetails)
            .Returns(internships.BuildMockDbSet().Object);
    }
}
