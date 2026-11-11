using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.ProjectExits.Requests;
using Moq;

namespace Modsen.Portal.TeamBoard.UnitTests.ProjectExits.Requests
{
    public class GetProjectExitsRequestTests
    {
        private readonly Mock<ITeamBoardDbContext> _dbContextMock;
        private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock;
        private readonly GetProjectExitsRequest _handler;

        public GetProjectExitsRequestTests()
        {
            _dbContextMock = new Mock<ITeamBoardDbContext>();
            _authenticatedUserContextMock = new Mock<IAuthenticatedUserContext>();

            _handler = new GetProjectExitsRequest(
                _dbContextMock.Object,
                _authenticatedUserContextMock.Object);

            _projectExits = SetUpDbContext();
        }

        private readonly List<ProjectExit> _projectExits;

        private List<ProjectExit> SetUpDbContext()
        {
            var projectExits = new List<ProjectExit>
            {
                new ProjectExit
                {
                    Id = 1,
                    UserId = 2,
                    User = new TeamBoardUser
                    {
                        FirstName = "Test",
                        LastName = "Testovich",
                        FirstNameRu = "Тест",
                        LastNameRu = "Тестович",
                        IsActive = true,
                        Email = "test@modsen-software.com",
                        DepartmentId = "NET",
                        Department = new TeamboardDepartment
                        {
                            DepartmentId = "NET",
                            ShortName = "net",
                            FullName = ".NET"
                        },
                        ResourceGroupId = 1,
                        ResourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "G1", }
                    },
                    ClientId = 1,
                    Client = new TeamBoardClient
                    {
                        ClientId =1,
                        Name = "Modsen",
                        IsActive = true,
                    },
                    ProjectId = 1,
                    Project = new TeamBoardProject
                    {
                        ProjectName = "Project1null",
                        ClientName = "Modsen"
                    },
                    ClosingDate = new DateOnly(2025, 3, 18),
                    ClosingReasonId = 1,
                    ClosingReason = new ProjectExitClosingReason { Id = 1, Name = "Some reason" },
                    IsActive = true,
                    CreatedAtUtc = new DateTime(2025, 3, 18),
                },
                new ProjectExit
                {
                    Id = 2,
                    UserId = 3,
                    User = new TeamBoardUser
                    {
                        FirstName = "John",
                        LastName = "Testovich",
                        Email = "Testovich@modsen-software.com",
                        DepartmentId = "JS",
                        Department = new TeamboardDepartment
                        {
                            DepartmentId = "JS",
                            ShortName = "js",
                            FullName = "Java Script"
                        },
                        ResourceGroupId = 3,
                        ResourceGroup = new TeamBoardResourceGroup { ResourceGroupName = "JS1", }
                    },
                    ClientId = 1,
                    Client = new TeamBoardClient
                    {
                        ClientId =1,
                        Name = "Modsen",
                        IsActive = true,
                    },
                    ProjectId = 1,
                    Project = new TeamBoardProject
                    {
                        ProjectName = "Project1null",
                        ClientName = "Modsen"
                    },
                    ClosingDate = new DateOnly(2025, 3, 18),
                    ClosingReasonId = 1,
                    ClosingReason = new ProjectExitClosingReason { Id = 2, Name = "Another reason" },
                    IsActive = true,
                    CreatedAtUtc = new DateTime(2025, 3, 18),
                }
            };

            return projectExits;
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoProjectExits()
        {
            // Arrange
            var emptyExits = new List<ProjectExit>().BuildMockDbSet().Object;
            _dbContextMock.Setup(x => x.ProjectExits).Returns(emptyExits);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20", [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnAllExits_WhenProjectExitsExist()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);

            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name))
                .Returns(true);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(
                new Dictionary<string, string?>
                {
                    [nameof(ODataRawQueryOptions.Top)] = "20",
                    [nameof(ODataRawQueryOptions.Skip)] = "0",
                });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyFromDepartment_WhenAuthedAsHoD()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);

            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name))
                .Returns(true);
            _authenticatedUserContextMock.Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.HeadOfDepartment]);
            _authenticatedUserContextMock.Setup(x => x.DepartmentId)
                .Returns("JS");

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(
                new Dictionary<string, string?>
                {
                    [nameof(ODataRawQueryOptions.Top)] = "20",
                    [nameof(ODataRawQueryOptions.Skip)] = "0",
                });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
        }

        [Fact]
        public async Task Handle_ShouldReturnOnlyFromResourceGroup_WhenAuthedAsGroupManager()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);

            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name))
                .Returns(true);
            _authenticatedUserContextMock.Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.GroupManager]);
            _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
                .Returns(1);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(1);
            result.Items.Single().UserId.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnAll_WhenAuthedAsGroupManagerWithManageAllProjectExits()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);

            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name))
                .Returns(true);
            _authenticatedUserContextMock.Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.HeadOfDepartment]);
            _authenticatedUserContextMock.Setup(x => x.ResourceGroupId)
                .Returns(1);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().HaveCount(2);
        }

        [Fact]
        public async Task Handle_ShouldNotReturnEmptyList_WhenAuthedAsEmployee()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);

            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ViewProjectExits.Name))
                .Returns(true);
            _authenticatedUserContextMock.Setup(x => x.GetRoles())
                .Returns([PortalRoleNames.Employee]);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldMapFullNameRu_WhenUserHasRussianName()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);
            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name))
                .Returns(true);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            var item = result.Items.First(x => x.UserId == 2);
            item.FullName.Should().Be("Тестович Тест (Testovich Test)");
        }

        [Fact]
        public async Task Handle_ShouldMapClosingReason_WhenSet()
        {
            // Arrange
            _dbContextMock.Setup(x => x.ProjectExits).Returns(_projectExits.BuildMockDbSet().Object);
            _authenticatedUserContextMock.Setup(x => x.HasClaim(StaffingClaimTypeNames.ManageAllProjectExits.Name))
                .Returns(true);

            var queryOptions = ODataQueryOptionsMock.BuildMock<OdpProjectExit>(new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "20",
                [nameof(ODataRawQueryOptions.Skip)] = "0",
            });

            var query = new GetProjectExitsRequest.Query(queryOptions);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Items.First(x => x.UserId == 2).ClosingReasonName.Should().Be("Some reason");
            result.Items.First(x => x.UserId == 3).ClosingReasonName.Should().Be("Another reason");
        }
    }
}
