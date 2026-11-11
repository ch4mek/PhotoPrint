using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Services;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.Core.Composers;
using Moq;
using UserSpecializationEntity = Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities.UserSpecialization;

namespace Modsen.Portal.TeamBoard.UnitTests.Skills;

public class SpecializationSnapshotComposerTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock;
    private readonly Mock<IAuthenticatedUserContext> _userContextMock;
    private readonly Mock<IClockService> _clockServiceMock;
    private readonly SpecializationSnapshotComposer _composer;
    private readonly UserSpecializationEntity _userSpecializationEntity = new ()
    {
        UserId = 20,
        SpecializationTemplateId = 101,
        SeniorityLevelId = "JuniorL2",
        IsPrimary = true,
        SpecializationHardSkills =
        [
            new UserSpecializationHardSkill { HardSkillId = 1001, SeniorityLevelId = "Junior" }
        ]
    };

    public SpecializationSnapshotComposerTests()
    {
        _dbContextMock = new Mock<ITeamBoardDbContext>();
        _userContextMock = new Mock<IAuthenticatedUserContext>();
        _clockServiceMock = new Mock<IClockService>();

        var userSpecializations = new List<UserSpecializationEntity>
        {
            _userSpecializationEntity
        };

        _dbContextMock.Setup(db => db.UserSpecializations)
            .Returns(userSpecializations.BuildMockDbSet().Object);

        _composer = new SpecializationSnapshotComposer(
            _dbContextMock.Object,
            _userContextMock.Object,
            _clockServiceMock.Object);
    }

    [Fact]
    public async Task CreateForDateWhenDataIsValidShouldCreateSnapshot()
    {
        // Arrange
        var userId = 1;
        var snapshotDate = new DateOnly(2024, 10, 24);

        _dbContextMock.Setup(db => db.UserSkillsSnapshots)
            .Returns(Array.Empty<UserSkillsSnapshot>().BuildMockDbSet().Object);
        _userContextMock.Setup(ctx => ctx.UserId).Returns(123);
        _clockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        await _composer.CreateForDate(userId, snapshotDate, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(db => db.UserSkillsSnapshots.Add(It.IsAny<UserSkillsSnapshot>()), Times.Once);
    }

    [Fact]
    public async Task CreateWhenSnapshotForTodayExistsShouldDeleteLastSnapshotAndCreateNew()
    {
        // Arrange
        var userId = 1;
        var todaysDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var lastSnapshot = new UserSkillsSnapshot
        {
            UserId = userId,
            SnapshotDate = todaysDate,
            IsDeleted = false
        };

        var query = new[] { lastSnapshot };
        _dbContextMock.Setup(db => db.UserSkillsSnapshots)
            .Returns(query.BuildMockDbSet().Object);
        _userContextMock.Setup(ctx => ctx.UserId).Returns(123);
        _clockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        await _composer.Create(userId, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        _dbContextMock.Verify(db => db.UserSkillsSnapshots.Update(It.IsAny<UserSkillsSnapshot>()), Times.Once);
    }

    [Fact]
    public async Task CreateWhenNoSnapshotExistsForTodayShouldCreateSnapshot()
    {
        // Arrange
        var userId = 1;
        DateOnly.FromDateTime(DateTime.UtcNow);
        var query = Array.Empty<UserSkillsSnapshot>();

        _dbContextMock.Setup(db => db.UserSkillsSnapshots)
            .Returns(query.BuildMockDbSet().Object);
        _userContextMock.Setup(ctx => ctx.UserId).Returns(123);
        _clockServiceMock.Setup(clock => clock.UtcNow).Returns(DateTime.UtcNow);

        // Act
        await _composer.Create(userId, CancellationToken.None);

        // Assert
        _dbContextMock.Verify(db => db.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _dbContextMock.Verify(db => db.UserSkillsSnapshots.Add(It.IsAny<UserSkillsSnapshot>()), Times.Once);
    }
}
