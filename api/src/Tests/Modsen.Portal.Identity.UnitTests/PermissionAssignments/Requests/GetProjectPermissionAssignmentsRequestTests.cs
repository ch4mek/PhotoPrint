using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Core.CoreDto;
using Modsen.Portal.Identity.Requests.PermissionAssignments.Requests;
using Moq;

namespace Modsen.Portal.Identity.UnitTests.PermissionAssignments.Requests;

public class GetProjectPermissionAssignmentsRequestTests
{
    private readonly GetProjectPermissionAssignmentsRequest _handler;
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContext = new();
    private readonly Mock<IIdentityDbContext> _dbContext = new();

    public GetProjectPermissionAssignmentsRequestTests()
    {
        _handler = new(_authenticatedUserContext.Object, _dbContext.Object);
    }

    [Fact]
    public async Task HandleWhenNoPermissionAssinmentsShouldReturnEmptyList()
    {
        // Arrange
        _dbContext
            .Setup(x => x.PermissionAssignments)
            .Returns(Array.Empty<PermissionAssignment>().BuildMockDbSet().Object);

        _authenticatedUserContext
            .Setup(x => x.DepartmentId)
            .Returns((string?)null);

        var queryOptions = ODataQueryOptionsMock.BuildMock<ProjectPermissionAssignmentCoreDto>(
            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "50",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            });

        var request = new GetProjectPermissionAssignmentsRequest.Query(queryOptions);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Total.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleWhenRequestIsValidShouldReturnCorrectData()
    {
        // Arrange
        var permissionsAssings = new List<PermissionAssignment>
        {
            new()
            {
                Id = 1,
                Permission = new PermissionDefinition
                {
                    Name = "1",
                    Id = 1
                },
                PermissionDefinitionId = 1,
                ResourceId = "1",
                ResourceName = "1111",
                ResourceType = PermissionResourceType.Project.ToString(),
                UserId = 1,
                User = new User
                {
                    Id = 1,
                    Color = "#45678d",
                    DepartmentId = "Net",
                    FirstName = "Qwerty",
                    LastName = "Zxcv",
                    ResourceGroupId = 11,
                    IsActive = true
                },
            },
            new()
            {
                Id = 2,
                Permission = new PermissionDefinition
                {
                    Name = "1",
                    Id = 1
                },
                PermissionDefinitionId = 1,
                ResourceId = "222",
                ResourceName = "222222",
                ResourceType = PermissionResourceType.Project.ToString(),
                UserId = 2,
                User = new User
                {
                    Id = 2,
                    Color = "#abcdef",
                    DepartmentId = null,
                    FirstName = "Asdf",
                    LastName = "Tompson",
                    ResourceGroupId = 22,
                    IsActive = true
                }
            },
        };

        _dbContext
            .Setup(x => x.PermissionAssignments)
            .Returns(permissionsAssings.BuildMockDbSet().Object);

        _authenticatedUserContext
            .Setup(x => x.DepartmentId)
            .Returns((string?)null);

        var queryOptions = ODataQueryOptionsMock.BuildMock<ProjectPermissionAssignmentCoreDto>(
            new Dictionary<string, string?>
            {
                [nameof(ODataRawQueryOptions.Top)] = "50",
                [nameof(ODataRawQueryOptions.Skip)] = "0"
            });

        var request = new GetProjectPermissionAssignmentsRequest.Query(queryOptions);

        var expected = new List<ProjectPermissionAssignmentCoreDto>
        {
            new()
            {
                Id = 1,
                UserId = permissionsAssings[0].UserId,
                PermissionId = permissionsAssings[0].PermissionDefinitionId,
                ProjectId = permissionsAssings[0].ResourceId,
                ProjectName = permissionsAssings[0].ResourceName,
                UserResourceGroupId = permissionsAssings[0].User.ResourceGroupId,
                PermissionName = permissionsAssings[0].Permission.Name,
                UserColor = permissionsAssings[0].User.Color,
                UserDepartmentId = permissionsAssings[0].User.DepartmentId,
                UserFullName = ToFullName.Reverse(permissionsAssings[0].User.FirstName, permissionsAssings[0].User.LastName)
            },
            new()
            {
                Id = 2,
                UserId = permissionsAssings[1].UserId,
                PermissionId = permissionsAssings[1].PermissionDefinitionId,
                ProjectId = permissionsAssings[1].ResourceId,
                ProjectName = permissionsAssings[1].ResourceName,
                UserResourceGroupId = permissionsAssings[1].User.ResourceGroupId,
                PermissionName = permissionsAssings[1].Permission.Name,
                UserColor = permissionsAssings[1].User.Color,
                UserDepartmentId = permissionsAssings[1].User.DepartmentId,
                UserFullName = ToFullName.Reverse(permissionsAssings[1].User.FirstName, permissionsAssings[1].User.LastName)
            },
        };

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Total.Should().Be(2);
        result.Items.Should().BeEquivalentTo(expected);
    }
}
