using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Core.Validators;
using Modsen.Portal.TeamBoard.Requests.Skills.UserSoftSkills.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.UserSoftSkills.Core;

public class UserSoftSkillSnapshotValidatorTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly UserSoftSkillSnapshotValidator _validator;

    public UserSoftSkillSnapshotValidatorTests()
    {
        _validator = new UserSoftSkillSnapshotValidator(_dbContextMock.Object);
    }

    [Fact]
    public async Task ValidateAsyncWhenNotAllSoftSkillsListedThrowsInvalidDataAppException()
    {
        //Arrange
        var seniorityLevels = new List<UserSoftSkillSeniorityLevelDto>()
        {
            new(1, "level1"),
            new(2, "level2"),
            new(3, "level3"),
            new(4, "level1"),
        };

        _dbContextMock
            .Setup(context => context.SoftSkills)
            .Returns(new List<SoftSkill>()
            {
                new() {  Name = "test", Id = 1 },
                new() {  Name = "test", Id = 2 },
                new() {  Name = "test", Id = 3 },
            }.BuildMockDbSet().Object);

        //Act
        var action = async () => await _validator.ValidateAsync(seniorityLevels, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Some soft skills not found");
    }

    [Fact]
    public async Task ValidateAsyncWhenNotAllSenioritiesListedThrowsInvalidDataAppException()
    {
        //Arrange
        var seniorityLevels = new List<UserSoftSkillSeniorityLevelDto>()
        {
            new(1, "level1"),
            new(2, "level2"),
            new(3, "level3"),
            new(4, "level1"),
        };

        _dbContextMock
            .Setup(context => context.SoftSkills)
            .Returns(new List<SoftSkill>()
            {
                new() {  Name = "test", Id = 1 },
                new() {  Name = "test", Id = 2 },
                new() {  Name = "test", Id = 3 },
                new() {  Name = "test", Id = 4 },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.SeniorityLevels)
            .Returns(new List<SeniorityLevel>()
            {
                new() { Id = "level1", InternalName = "level1", Description = "test", ShortName = "test", ExternalName = "test" },
                new() { Id = "level2", InternalName = "level2", Description = "test", ShortName = "test", ExternalName = "test"  },
                new() { Id = "level4", InternalName = "level4", Description = "test", ShortName = "test", ExternalName = "test"   },
            }.BuildMockDbSet().Object);

        //Act
        var action = async () => await _validator.ValidateAsync(seniorityLevels, _token);

        //Assert
        await action.Should().ThrowAsync<InvalidDataAppException>("Some soft skill seniorities not found");
    }

    [Fact]
    public async Task ValidateAsyncWhenRequestValidNotThrowAnyException()
    {
        //Arrange
        var seniorityLevels = new List<UserSoftSkillSeniorityLevelDto>()
        {
            new(1, "level1"),
            new(2, "level2"),
            new(3, "level3"),
            new(4, "level1"),
        };

        _dbContextMock
            .Setup(context => context.SoftSkills)
            .Returns(new List<SoftSkill>()
            {
                new() {  Name = "test", Id = 1 },
                new() {  Name = "test", Id = 2 },
                new() {  Name = "test", Id = 3 },
                new() {  Name = "test", Id = 4 },
            }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.SeniorityLevels)
            .Returns(new List<SeniorityLevel>()
            {
                new() { Id = "level1", InternalName = "level1", Description = "test", ShortName = "test", ExternalName = "test" },
                new() { Id = "level2", InternalName = "level2", Description = "test", ShortName = "test", ExternalName = "test" },
                new() { Id = "level3", InternalName = "level3", Description = "test", ShortName = "test", ExternalName = "test"  },
            }.BuildMockDbSet().Object);

        //Act
        var action = async () => await _validator.ValidateAsync(seniorityLevels, _token);

        //Assert
        await action.Should().NotThrowAsync();
    }
}
