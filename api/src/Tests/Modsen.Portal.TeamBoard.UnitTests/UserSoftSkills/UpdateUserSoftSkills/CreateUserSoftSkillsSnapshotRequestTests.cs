using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Composers;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSoftSkills.UpdateUserSoftSkills;

public class CreateUserSoftSkillsSnapshotRequestTests
{
    private readonly Mock<ICanTeamBoardUserBeRetrievedBusinessRule>
        _canTeamBoardUserBeRetrievedBusinessRuleMock = new();
    private readonly Mock<IUserSoftSkillSnapshotValidator> _snapshotValidator = new();
    private readonly Mock<IUserSoftSkillsSnapshotComposer> _snapshotComposer = new();
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private readonly CreateUserSoftSkillsSnapshotRequest _handler;

    public CreateUserSoftSkillsSnapshotRequestTests()
    {
        _handler = new CreateUserSoftSkillsSnapshotRequest(
            _canTeamBoardUserBeRetrievedBusinessRuleMock.Object,
            _snapshotValidator.Object,
            _snapshotComposer.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserCantBeRetrievedShouldThrowActionNotAllowedException()
    {
        // Arrange
        var requestDto = new CreateUserSoftSkillsSnapshotDto(1, DateOnly.FromDateTime(DateTime.UtcNow), []);
        var command = new CreateUserSoftSkillsSnapshotRequest.Command(requestDto);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(requestDto.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    [Fact]
    public async Task HandleWhenSnapshotForDateExistsShouldThrowEntityConflictException()
    {
        // Arrange
        var userId = 1;
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var requestDto = new CreateUserSoftSkillsSnapshotDto(userId, date, []);
        var command = new CreateUserSoftSkillsSnapshotRequest.Command(requestDto);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(requestDto.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(context => context.UserSoftSkillsSnapshots)
            .Returns(new List<UserSoftSkillsSnapshot>()
            {
                new()
                {
                    SnapshotDate = date,
                    UserId = userId,
                    SoftSkills = new List<UserSoftSkill>()
                    {
                        new()
                        {
                            SoftSkill = new SoftSkill()
                            {
                                Name = "test",IsActive = true
                            }
                        }
                    }
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityConflictException>("An entry for the selected date already exists");
    }

    [Fact]
    public async Task HandleWhenNewSnapshotInPastAndThereAreNoExistedSnapshotsShouldThrowInvalidDataAppException()
    {
        // Arrange
        var userId = 1;
        var date = DateOnly.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(15));
        var requestDto = new CreateUserSoftSkillsSnapshotDto(userId, date, []);
        var command = new CreateUserSoftSkillsSnapshotRequest.Command(requestDto);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(requestDto.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(context => context.UserSoftSkillsSnapshots)
            .Returns(new List<UserSoftSkillsSnapshot>()
            {
                new()
                {
                    UserId = userId,
                    SoftSkills = new List<UserSoftSkill>()
                    {
                        new()
                        {
                            SoftSkill = new SoftSkill()
                            {
                                Name = "test",IsActive = false
                            }
                        }
                    }
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>(
            "You cannot create entries for dates in the past if there is no existed snapshots");
    }

    [Fact]
    public async Task HandleWhenNewSnapshotInPastBeforeLastColumnShouldThrowInvalidDataAppException()
    {
        // Arrange
        var userId = 1;
        var date = DateOnly.FromDateTime(DateTime.UtcNow - TimeSpan.FromDays(15));
        var requestDto = new CreateUserSoftSkillsSnapshotDto(userId, date, []);
        var command = new CreateUserSoftSkillsSnapshotRequest.Command(requestDto);

        _canTeamBoardUserBeRetrievedBusinessRuleMock
            .Setup(br => br.IsCorrespondsToBusiness(requestDto.UserId, AccessOptions.Manager, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(context => context.UserSoftSkillsSnapshots)
            .Returns(new List<UserSoftSkillsSnapshot>()
            {
                new()
                {
                    SnapshotDate = date.AddDays(4),
                    UserId = userId,
                    SoftSkills = new List<UserSoftSkill>()
                    {
                        new()
                        {
                            SoftSkill = new SoftSkill()
                            {
                                Name = "test",IsActive = true
                            }
                        }
                    }
                }
            }.BuildMockDbSet().Object);

        // Act
        var result = async () => await _handler.Handle(command, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>(
            "You cannot create entries for dates before the date in the last column");
    }
}
