using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Users.BusinessRules;
using Modsen.Portal.Equipment.Requests.Users.Requests;

namespace Modsen.Portal.Equipment.UnitTests.Requests;

public class GetUserEquipmentRequestTests
{
    private readonly Mock<IEquipmentDbContext> _equipmentDbContext = new();
    private readonly Mock<IIsAllowedToGetUserEquipmentBusinessRule> _isAllowedToGetUserEquipmentBusinessRuleMock = new();
    private readonly GetUserEquipmentRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public GetUserEquipmentRequestTests()
    {
        _handler = new(
            _equipmentDbContext.Object,
            _isAllowedToGetUserEquipmentBusinessRuleMock.Object);
    }

    [Fact]
    public async Task HandleWhenUserDoesNotExistsShouldThrowEntityNotFoundException()
    {
        // Arrange
        var request = new GetUserEquipmentRequest.Query(1234, It.IsAny<ODataQueryOptions<OdpUserAssignments>>());

        var users = new List<EquipmentUser>();

        SetupUsers(users);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenUserDoesNotHavePermissionShouldThrowActionNotAllowedException()
    {
        // Arrange
        var request = new GetUserEquipmentRequest.Query(1, It.IsAny<ODataQueryOptions<OdpUserAssignments>>());
        var user = new EquipmentUser
        {
            UserId = 1,
            DepartmentId = "Net",
            Email = "modsen@modsen-software.com",
            FirstName = "Test",
            LastName = "Test"
        };

        SetupUsers([user]);

        _isAllowedToGetUserEquipmentBusinessRuleMock
            .Setup(x => x.IsCorrespondsToBusiness(It.IsAny<int?>(), _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>();
    }

    private void SetupUsers(List<EquipmentUser> users)
    {
        _equipmentDbContext
            .Setup(mock => mock.EquipmentUsers)
            .Returns(users.BuildMockDbSet().Object);
    }
}
