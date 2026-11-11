using Modsen.Portal.Core.Identity;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests;
using Modsen.Portal.TimeTrack.UseCases.Projects.Requests.RequestDtos;

namespace Modsen.Portal.TimeTrack.UnitTests.Projects;

public class GetProjectManagersLookupRequestTests
{
    private readonly Mock<IIdentityContract> _identityContract = new();

    private readonly GetProjectManagersLookupRequest _useCase;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetProjectManagersLookupRequestTests()
    {
        _useCase = new GetProjectManagersLookupRequest(_identityContract.Object);
    }

    [Fact]
    public async Task HandleWhenProjectManagersExistShouldReturnListOfProjectManagersInOrderHeadThenEmployeeById()
    {
        // Arrange
        //remove when will be fixed in identity
        var projectManagers = new List<Identity.Contract.Models.ProjectManagerLookupDto>
        {
            new() { UserId = 1, LastName = "Employee", FirstName = "Test", Role = PortalRoleNames.Employee },
            new() { UserId = 2, LastName = "TestLastName", FirstName = "TestFirstName", Role = PortalRoleNames.Employee },
            new() { UserId = 3, LastName = "Head", FirstName = "Test", Role = PortalRoleNames.HeadOfDepartment }
        };

        _identityContract
            .Setup(q => q.GetProjectManagersLookup(_cancellationToken))
            .ReturnsAsync(projectManagers);

        var expectedProjectManagers = new List<ProjectManagerLookupDto>
        {
            new(ManagerUserId: 3, ManagerUserFullName: "Head Test"),
            new(ManagerUserId: 1, ManagerUserFullName: "Employee Test"),
            new(ManagerUserId: 2, ManagerUserFullName : "TestLastName TestFirstName")
        };

        // Act
        var result = await _useCase.Handle(
            new GetProjectManagersLookupRequest.Query(), _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProjectManagers);
    }

    [Fact]
    public async Task HandleWhenPmoDepartmentHasNoHeadShouldReturnListOfProjectManagers()
    {
        // Arrange
        var projectManagers = new List<Identity.Contract.Models.ProjectManagerLookupDto>
        {
            new() { UserId = 1, LastName = "Employee", FirstName = "Test", Role = PortalRoleNames.Employee },
            new() { UserId = 2, LastName = "TestLastName", FirstName = "TestFirstName", Role = PortalRoleNames.Employee },
            new() { UserId = 3, LastName = "LastName", FirstName = "Test", Role = PortalRoleNames.HeadOfDepartment }
        };

        _identityContract
            .Setup(q => q.GetProjectManagersLookup(_cancellationToken))
            .ReturnsAsync(projectManagers);

        var expectedProjectManagers = new List<ProjectManagerLookupDto>
        {
            new(ManagerUserId: 1, ManagerUserFullName: "Employee Test"),
            new(ManagerUserId: 2, ManagerUserFullName : "TestLastName TestFirstName"),
            new(ManagerUserId: 3, ManagerUserFullName: "LastName Test")
        };

        // Act
        var result = await _useCase.Handle(
            new GetProjectManagersLookupRequest.Query(), _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedProjectManagers);
    }
}
