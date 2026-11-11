using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;
using Modsen.Portal.Cv.Requests.Cvs.Requests;

namespace Modsen.Portal.Cv.UnitTests.Cvs.Requests;

public class GetPageableCvsRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
    private readonly Mock<ICvDbContext> _cvDbContextMock;
    private readonly GetPageableCvsRequest _handler;
    private readonly CancellationToken _cancellationToken;

    public GetPageableCvsRequestTests()
    {
        _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();
        _cvDbContextMock = new Mock<ICvDbContext>();
        _cancellationToken = CancellationToken.None;

        _handler = new GetPageableCvsRequest(
            _cvDbContextMock.Object,
            _authenticatedUserContextMock.Object);
    }

    private static ODataQueryOptions<OdpCvDto> CreateODataQueryOptions(Dictionary<string, string?> queryOptions)
    {
        return ODataQueryOptionsMock.BuildMock<OdpCvDto>(queryOptions);
    }

    [Fact]
    public async Task HandleWhenUserIsHeadOfDepartmentReturnsPagedCvsForDepartment()
    {
        // Arrange
        const string departmentId = "Net";
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "CV1",
                UserId = 1,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = 1,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    DepartmentId = departmentId,
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
            new()
            {
                Id = 2,
                CvName = "CV2",
                UserId = 2,
                UserFirstName = "Firsname2",
                UserLastName = "Lastname2",
                CvUser = new CvUserEntity()
                {
                    UserId = 2,
                    FirstName = "Firstname2",
                    LastName = "LastName2",
                    DepartmentId = departmentId,
                    Email = "email@asdfgh2",
                    IsActive = true
                },
            }
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["HeadOfDepartment"]);
        _authenticatedUserContextMock.Setup(x => x.DepartmentId).Returns(departmentId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(x => x.Id == 1 && x.CvName == "CV1");
        result.Items.Should().Contain(x => x.Id == 2 && x.CvName == "CV2");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenUserIsGroupManagerReturnsPagedCvsForResourceGroup()
    {
        // Arrange
        const int resourceGroupId = 20;
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "CV1",
                UserId = 1,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = 1,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    ResourceGroupId = resourceGroupId,
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "5",
            ["$skip"] = "0"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["GroupManager"]);
        _authenticatedUserContextMock.Setup(x => x.ResourceGroupId).Returns(resourceGroupId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().ContainSingle(x => x.Id == 1 && x.CvName == "CV1");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(5);
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenUserIsEmployeeReturnsPagedCvsForUser()
    {
        // Arrange
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "CV1",
                UserId = userId,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().ContainSingle(x => x.Id == 1 && x.CvName == "CV1");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
    }

    [Fact]
    public async Task HandleWhenUserIsCtoReturnsPagedCvsForUser()
    {
        // Arrange
        const int userId1 = 1;
        const int userId2 = 2;
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "CV1",
                UserId = userId1,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = userId1,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
            new CvEntity()
            {
                Id = 2,
                CvName = "CV2",
                UserId = userId2,
                UserFirstName = "Firsname2",
                UserLastName = "Lastname2",
                CvUser = new CvUserEntity()
                {
                    UserId = userId2,
                    FirstName = "Firstname2",
                    LastName = "LastName2",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["CTO"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId1);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(2);
        result.Items.Should().Contain(x => x.Id == 1 && x.CvName == "CV1");
        result.Items.Should().Contain(x => x.Id == 2 && x.CvName == "CV2");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(2);
    }

    [Fact]
    public async Task HandleWhenNoCvsFoundReturnsEmptyPagedList()
    {
        // Arrange
        const int userId = 1;
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(new List<CvEntity>().BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(0);
        result.PageSize.Should().Be(0);
        result.Total.Should().Be(0);
    }

    [Fact]
    public async Task HandleWhenODataPaginationAppliedReturnsCorrectPage()
    {
        // Arrange
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "CV1",
                UserId = userId,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
            new CvEntity()
            {
                Id = 2,
                CvName = "CV2",
                UserId = userId,
                UserFirstName = "Firsname2",
                UserLastName = "Lastname2",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname2",
                    LastName = "LastName2",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
            new CvEntity()
            {
                Id = 3,
                CvName = "CV3",
                UserId = userId,
                UserFirstName = "Firsname3",
                UserLastName = "Lastname3",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname3",
                    LastName = "LastName3",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "1",
            ["$skip"] = "1"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().ContainSingle(x => x.Id == 2 && x.CvName == "CV2");
        result.Page.Should().Be(2); // (skip=1 / top=1) + 1
        result.PageSize.Should().Be(1);
        result.Total.Should().Be(3);
    }

    [Fact]
    public async Task HandleWhenODataFilterAndOrderByAppliedReturnsFilteredAndSortedCvs()
    {
        // Arrange
        const int userId = 1;
        var cvs = new List<CvEntity>
        {
            new CvEntity()
            {
                Id = 1,
                CvName = "Alpha",
                UserId = userId,
                UserFirstName = "Firsname1",
                UserLastName = "Lastname1",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname1",
                    LastName = "LastName1",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
            new CvEntity()
            {
                Id = 2,
                CvName = "Beta",
                UserId = userId,
                UserFirstName = "Firsname2",
                UserLastName = "Lastname2",
                CvUser = new CvUserEntity()
                {
                    UserId = userId,
                    FirstName = "Firstname2",
                    LastName = "LastName",
                    Email = "email@asdfgh",
                    IsActive = true
                },
            },
        };
        var queryOptions = CreateODataQueryOptions(new Dictionary<string, string?>
        {
            ["$top"] = "10",
            ["$skip"] = "0",
            ["$filter"] = "CvName eq 'Beta'",
            ["$orderby"] = "CvName desc"
        });
        _authenticatedUserContextMock.Setup(x => x.GetRoles()).Returns(["Employee"]);
        _authenticatedUserContextMock.Setup(x => x.UserId).Returns(userId);
        _cvDbContextMock.Setup(x => x.Cvs).Returns(cvs.BuildMockDbSet().Object);

        var query = new GetPageableCvsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.Should().ContainSingle(x => x.Id == 2 && x.CvName == "Beta");
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.Total.Should().Be(1);
    }
}
