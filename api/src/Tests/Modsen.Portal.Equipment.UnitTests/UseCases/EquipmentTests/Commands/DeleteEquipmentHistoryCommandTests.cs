using MockQueryable.Moq;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.EquipmentHistory.Requests;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Commands;

public class DeleteEquipmentHistoryCommandTests
{
    private readonly Mock<IGetEquipmentByIdDbQuery> _getEquipmentByIdDbQueryMock = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly DeleteEquipmentHistoryRequest _handler;
    public DeleteEquipmentHistoryCommandTests()
    {
        _cancellationToken = CancellationToken.None;

        _handler = new DeleteEquipmentHistoryRequest(
            _getEquipmentByIdDbQueryMock.Object,
            _dbContextMock.Object);
    }

    [Fact]
    public async Task HandleWhenEquipmentDoesNotExistShouldThrowEntityNotFoundException()
    {
        // Arrange
        var equipmentId = 1;
        var historyId = 1;

        var request = new DeleteEquipmentHistoryRequest.Command(equipmentId, historyId);

        _getEquipmentByIdDbQueryMock.Setup(
                x => x.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(null as EquipmentEntity);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task HandleWhenEquipmentDoesNotContainAssignsInvalidDataAppException()
    {
        // Arrange
        var equipmentId = 2;
        var historyId = 2;

        var equipment = new EquipmentEntity()
        {
            Id = equipmentId,
            Name = "TestName",
            TypeId = "Laptop",
            SerialNumber = "00000000001",
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            PurchasePrice = 1000,
            PurchasePriceUsd = 300,
            PurchaseDate = DateTime.Today.Date,
            PurchasePlace = "Minsk",
            GuaranteeDate = DateTime.Today.AddYears(1).Date,
            Characteristics = "Test characteristics",
            Location = EquipmentLocationType.Belarus,
            Comment = "Test comment",
            Assigns = []
        };

        _getEquipmentByIdDbQueryMock.Setup(
                x => x.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>{ equipment }.BuildMockDbSet().Object);

        var request = new DeleteEquipmentHistoryRequest.Command(equipmentId, historyId);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenEquipmentReturnDateIsNotNullInvalidDataAppException()
    {
        // Arrange
        var equipmentId = 5;
        var historyId = 6;

        var assign = new EquipmentAssignEntity
        {
            Id = 6,
            AssignedToUserId = 2,
            EquipmentId = equipmentId,
            IssueDate = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            ReturnDate = DateTime.MaxValue
        };

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            TypeId = "Laptop",
            Location = EquipmentLocationType.Belarus,
            Characteristics = "Has keys",
            Name = "Laptop",
            BrokenStatus = EquipmentBrokenStatus.None,
            PurchasePlace = "Market",
            SerialNumber = "0001",
            Assigns = [assign],
            Approver = new EquipmentUser
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@modsen-software.com"
            }
        };

        var equipmentToDelete = equipment;
        equipmentToDelete.Approver = null;
        equipmentToDelete.ApproverId = 1;

        _getEquipmentByIdDbQueryMock.Setup(
                x => x.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>{ equipment }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>{ assign }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>
            {
                new()
                {
                    BrokenStatus = EquipmentBrokenStatus.None,
                    Id = equipmentId,
                    Location = EquipmentLocationType.Belarus,
                    SerialNumber = "afsf",
                    Characteristics = "sfsd",
                    PurchasePlace = "sfdsf",
                    PurchaseCurrency = MoneyCurrencyType.Usd,
                    Name = "Name"
                }
            }.BuildMockDbSet().Object);

        var request = new DeleteEquipmentHistoryRequest.Command(equipmentId, historyId);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>();
    }

    [Fact]
    public async Task HandleWhenEquipmentReturnDateIsNullAndHasIssuedStatusShouldDeleteActiveAssign()
    {
        // Arrange
        var equipmentId = 5;
        var historyId = 6;

        var assign = new EquipmentAssignEntity
        {
            Id = 6,
            AssignedToUserId = 2,
            EquipmentId = equipmentId,
            IssueDate = new DateTime(2023, 7, 1, 0, 0, 0, DateTimeKind.Utc),
            ReturnDate = null,
            User = new EquipmentUser
            {
                UserId = 2,
                FirstName = "Hannah",
                LastName = "Doe",
                Email = "hannah.doe@modsen-software.com"
            },
        };

        var equipment = new EquipmentEntity
        {
            Id = equipmentId,
            TypeId = "Laptop",
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.None,
            Characteristics = "Has keys",
            Name = "Laptop",
            PurchasePlace = "Market",
            SerialNumber = "0001",
            Assigns = [assign],
            Approver = new EquipmentUser
            {
                UserId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@modsen-software.com"
            }
        };

        var expectedEquipmentHolderDto = new EquipmentHolderDto
        {
            Id = assign.AssignedToUserId,
            IssueDate = assign.IssueDate,
            FirstName = "Hannah",
            LastName = "Doe",
            EquipmentHistoryId = historyId,
        };

        var equipmentToDelete = equipment;
        equipmentToDelete.Approver = null;
        equipmentToDelete.ApproverId = 1;

        _getEquipmentByIdDbQueryMock.Setup(
                x => x.ExecuteAsync(
                    It.IsAny<int>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(equipment);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>{ equipment }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.EquipmentAssigns)
            .Returns(new List<EquipmentAssignEntity>{ assign }.BuildMockDbSet().Object);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>
            {
                new()
                {
                    BrokenStatus = EquipmentBrokenStatus.None,
                    Id = equipmentId,
                    Location = EquipmentLocationType.Belarus,
                    SerialNumber = "afsf",
                    Characteristics = "sfsd",
                    PurchasePlace = "sfdsf",
                    PurchaseCurrency = MoneyCurrencyType.Usd,
                    Name = "Name",
                    Assigns = [assign]
                }
            }.BuildMockDbSet().Object);

        var request = new DeleteEquipmentHistoryRequest.Command(equipmentId, historyId);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedEquipmentHolderDto);
    }
}
