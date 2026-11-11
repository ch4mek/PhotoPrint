using MockQueryable.Moq;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Queries;

public class GetEquipmentsRequestTests
{
    private readonly Mock<IAuthenticatedUserContext> _authedUserMock = new();
    private readonly Mock<ICanAccessUnassignedEquipmentBusinessRule> _canAccessUnassignedEquipmentBusinessRuleMock = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetEquipmentsRequest _handler;

    public GetEquipmentsRequestTests()
    {
        _handler = new GetEquipmentsRequest(
            _canAccessUnassignedEquipmentBusinessRuleMock.Object,
            _authedUserMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task GetEquipmentsShouldCallEquipmentDbQueries()
    {
        // Arrange
        var pageableEquipments = EmptyPageableListOfItems<OdpEquipment>.Value;
        var queryOptions = new Dictionary<string, string?>
        {
            ["top"] = "10",
            ["skip"] = "0"
        };

        var oDataQueryOptions = ODataQueryOptionsMock.BuildMock<OdpEquipment>(queryOptions);
        var isAvailableEquipmentsWithoutAssigners = true;

        _canAccessUnassignedEquipmentBusinessRuleMock.Setup(x =>
            x.IsCorrespondsToBusiness(_authedUserMock.Object))
            .Returns(isAvailableEquipmentsWithoutAssigners);

        var equipments = new List<EquipmentEntity>();
        _dbContextMock.Setup(x => x.Equipments).Returns(equipments.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(
            new GetEquipmentsRequest.Query(oDataQueryOptions),
            _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.Page.Should().Be(pageableEquipments.Page);
        result.PageSize.Should().Be(pageableEquipments.PageSize);
        result.Total.Should().Be(pageableEquipments.Total);

        _canAccessUnassignedEquipmentBusinessRuleMock.Verify(x => x.IsCorrespondsToBusiness(_authedUserMock.Object), Times.Once);
    }
}
