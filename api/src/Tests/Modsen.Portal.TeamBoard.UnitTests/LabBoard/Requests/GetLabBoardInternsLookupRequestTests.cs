using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.Requests.LabBoard.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.LabBoard.Requests;

public class GetLabBoardInternsLookupRequestTests
{
    private readonly GetLabBoardInternsLookupRequest _handler;
    private readonly Mock<ITeamBoardDbContext> _dbContextMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetLabBoardInternsLookupRequestTests()
    {
        _handler = new GetLabBoardInternsLookupRequest(_dbContextMock.Object, _authenticatedUserContextMock.Object);

        SetupDbContext();
    }

    [Fact]
    public async Task HandleWhenIncludeInactiveFalseReturnActiveIntern()
    {
        // Arrange
        var includeInactive = false;

        // Act
        var result = await _handler.Handle(new GetLabBoardInternsLookupRequest.Query(includeInactive),
            _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.Id == 4);
    }

    [Fact]
    public async Task HandleWhenIncludeInactiveTrueReturnAllInterns()
    {
        // Arrange
        var includeInactive = true;

        // Act
        var result = await _handler.Handle(new GetLabBoardInternsLookupRequest.Query(includeInactive),
            _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == 4);
        result.Should().Contain(x => x.Id == 5);
    }

    [Fact]
    public async Task HandleWhenGroupManagerRequestsReturnInternFromHisGroup()
    {
        // Arrange
        var includeInactive = true;

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock.Setup(x => x.GetUserClaimsNames())
            .Returns([]);

        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
            .Returns(1);

        // Act
        var result = await _handler.Handle(new GetLabBoardInternsLookupRequest.Query(includeInactive),
            _cancellationToken);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(x => x.Id == 4);
    }

    [Fact]
    public async Task HandleWhenGroupManagerWithManageAllInternsClaimRequestsReturnAllInterns()
    {
        // Arrange
        var includeInactive = true;

        _authenticatedUserContextMock.Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.GroupManager]);

        _authenticatedUserContextMock.Setup(x => x.GetUserClaimsNames())
            .Returns([LabBoardClaimTypeNames.ManageAllInterns.Name]);

        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
            .Returns(1);

        // Act
        var result = await _handler.Handle(new GetLabBoardInternsLookupRequest.Query(includeInactive),
            _cancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(x => x.Id == 4);
        result.Should().Contain(x => x.Id == 5);
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
                Email = "Email2",
                PersonalEmail = "PersonalEmail2",
                DepartmentId = "Js",
                Position = "Employee",
                IsActive = true,
            },
            new()
            {
                UserId = 3,
                FirstName = "FirstName3",
                LastName = "LastName3",
                FirstNameRu = "FirstNameRu3",
                Email = "Email3",
                PersonalEmail = "PersonalEmail3",
                DepartmentId = "Js",
                Position = "Employee",
                IsActive = true,
            },
            new()
            {
                UserId = 4,
                FirstName = "FirstName4",
                LastName = "LastName4",
                FirstNameRu = "FirstNameRu4",
                Email = "PersonalEmail4",
                PersonalEmail = "PersonalEmail4",
                DepartmentId = "LD",
                Position = "Intern",
                ResourceGroupId = 1,
                IsActive = true,
            },
            new()
            {
                UserId = 5,
                FirstName = "FirstName5",
                LastName = "LastName5",
                FirstNameRu = "FirstNameRu5",
                Email = "PersonalEmail5",
                PersonalEmail = "PersonalEmail5",
                DepartmentId = "LD",
                Position = "Intern",
                ResourceGroupId = 2,
                IsActive = false,
            },
        };

        _dbContextMock.SetupGet(x => x.TeamBoardUsers)
            .Returns(users.BuildMockDbSet().Object);
    }
}
