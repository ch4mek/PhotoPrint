using MockQueryable.Moq;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.DbQueries;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.Specializations.Core;

public class DepartmentsSpecializationAssignmentsDbQueryTests
{
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly DepartmentsSpecializationAssignmentsDbQuery _dbQuery;

    public DepartmentsSpecializationAssignmentsDbQueryTests()
    {
        _dbQuery = new DepartmentsSpecializationAssignmentsDbQuery(_dbContextMock.Object);
    }

    [Fact]
    public async Task ExecuteAsyncShouldReturnAllAssignments()
    {
        //Arrange
        var specializationTemplateId = 1;
        string[] departments = ["Java", "Net", "JS"];

        _dbContextMock
            .Setup(dbContext => dbContext.UserSpecializations)
            .Returns(new List<DataAccess.Npgsql.DatabaseEntities.UserSpecialization>()
            {
                new()
                {
                    SpecializationTemplateId = specializationTemplateId,
                    User = new TeamBoardUser()
                    {
                        IsActive = true,
                        DepartmentId = "Java",
                        FirstName = "test",
                        LastName = "test",
                        Email = "test"
                    }
                },
                new()
                {
                    SpecializationTemplateId = specializationTemplateId,
                    User = new TeamBoardUser()
                    {
                        IsActive = false,
                        DepartmentId = "Net",
                        FirstName = "test",
                        LastName = "test",
                        Email = "test"
                    }
                },
                new()
                {
                    SpecializationTemplateId = specializationTemplateId,
                    User = new TeamBoardUser()
                    {
                        IsActive = true,
                        DepartmentId = "JS",
                        FirstName = "test",
                        LastName = "test",
                        Email = "test"
                    }
                },
            }.BuildMockDbSet().Object);

        var expectedResult = new DepartmentSpecializationAssignment[]
        {
            new("Java", true),
            new("Net", false),
            new("JS", true)
        };

        //Act
        var result = await _dbQuery.ExecuteAsync(specializationTemplateId, departments, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);

    }
}
