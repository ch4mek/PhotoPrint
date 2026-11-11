using System.Globalization;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Generators;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Commands;

public class CreateEquipmentCommandTests
{
    private readonly Mock<IEquipmentInventoryNumberGenerator> _equipmentInventoryNumberGeneratorMock = new();
    private readonly Mock<IIsSerialNumberExistsBusinessRule> _isSerialNumberExistingDbQueryMock = new();
    private readonly Mock<IIsEquipmentTypeExistsDbQuery> _isEquipmentTypeExistDbQueryMock = new();
    private readonly Mock<IEquipmentDbContext> _equipmentDbContextMock = new();
    private readonly Mock<IIdentityContract> _identityContract = new();

    private readonly CancellationToken _cancellationToken;
    private readonly CreateEquipmentRequest _handler;

    public CreateEquipmentCommandTests()
    {
        _cancellationToken = CancellationToken.None;
        _handler = new CreateEquipmentRequest(
            _equipmentInventoryNumberGeneratorMock.Object,
            _isEquipmentTypeExistDbQueryMock.Object,
            _isSerialNumberExistingDbQueryMock.Object,
            _equipmentDbContextMock.Object,
            _identityContract.Object);
    }

    [Fact]
    public async Task HandleApproverDoesNotHavePermissionShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int approverId = 1;

        var request = new CreateEquipmentRequest.Command(new CreateEquipmentRequestDto()
        {
            Name = "Name",
            EquipmentTypeId = "Headset",
            BrokenStatus = EquipmentBrokenStatus.None,
            Location = EquipmentLocationType.Belarus,
            SerialNumber = "ABC-CBA",
            PurchasePrice = 3999.99m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 1999.99m,
            PurchaseDate = DateTime.UtcNow.Date,
            PurchasePlace = "PurchasePlace",
            GuaranteeDate = DateTime.UtcNow.Date.AddYears(2),
            Characteristics = "Characteristics",
            Comment = "Comment",
            InvoiceNumber = "DWEVDVQ3214EDA",
            Storage = LocationType.Vitebsk,
            RepairPlace = null,
            ApproverId = approverId,
            JiraTask = "qwe"
        });

        _equipmentDbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com"
                }
            }.BuildMockDbSet().Object);

        _identityContract.Setup(x => x.IsUserManager(approverId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.ApproverDoesNotHavePermission);
    }

    [Fact]
    public async Task HandleValidRequestShouldCreateEquipment()
    {
        // Arrange
        const int userId = 1;

        var request = new CreateEquipmentRequest.Command(new CreateEquipmentRequestDto()
        {
            Name = "Name",
            EquipmentTypeId = "Headset",
            BrokenStatus = EquipmentBrokenStatus.Broken,
            Location = EquipmentLocationType.Belarus,
            SerialNumber = "ABC-CBA",
            PurchasePrice = 3999.99m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 1999.99m,
            PurchaseDate = DateTime.UtcNow.Date,
            PurchasePlace = "PurchasePlace",
            GuaranteeDate = DateTime.UtcNow.Date.AddYears(2),
            Characteristics = "Characteristics",
            Comment = "Comment",
            InvoiceNumber = "DWEVDVQ3214EDA",
            Storage = LocationType.Vitebsk,
            RepairPlace = null,
            ApproverId = userId,
            JiraTask = "qwe"
        });

        var expectedEquipmentUser = new EquipmentUser
        {
            UserId = request.RequestDto.ApproverId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@modsen-software.com"
        };

        var invNumberPart = request.RequestDto.PurchaseDate.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        var expectedEquipmentDto = new EquipmentCoreDto
        {
            Id = 0,
            Characteristics = request.RequestDto.Characteristics,
            GuaranteeDate = request.RequestDto.GuaranteeDate,
            Location = request.RequestDto.Location,
            Name = request.RequestDto.Name,
            BrokenStatus = EquipmentBrokenStatus.Broken,
            PurchaseDate = request.RequestDto.PurchaseDate,
            PurchasePlace = request.RequestDto.PurchasePlace,
            PurchasePrice = request.RequestDto.PurchasePrice,
            PurchasePriceUsd = request.RequestDto.PurchasePriceUsd,
            SerialNumber = request.RequestDto.SerialNumber,
            InventoryNumber = $"{invNumberPart}1D",
            TypeId = request.RequestDto.EquipmentTypeId,
            JiraTask = "qwe",
            Approver = new ApproverDto
            {
                Id = request.RequestDto.ApproverId,
                FirstName = expectedEquipmentUser.FirstName,
                LastName = expectedEquipmentUser.LastName
            },
            Type = new EquipmentTypeDto(request.RequestDto.EquipmentTypeId, "Headset", "Headset"),
            Comment = request.RequestDto.Comment,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            InvoiceNumber = request.RequestDto.InvoiceNumber,
            Storage = LocationType.Vitebsk
        };

        _identityContract.Setup(x => x.IsUserManager(
                It.IsAny<int>(),
                _cancellationToken))
            .ReturnsAsync(true);

        _equipmentDbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>()
            {
                new()
                {
                    Id = 0,
                    Name = "Laptop",
                    PurchasePlace = "Amazon",
                    Characteristics = "16GB RAM",
                    SerialNumber = "S1",
                    Approver = new EquipmentUser
                    {
                        UserId = request.RequestDto.ApproverId,
                        FirstName = expectedEquipmentUser.FirstName,
                        LastName = expectedEquipmentUser.LastName,
                        Email = expectedEquipmentUser.Email
                    },
                    Type = new EquipmentTypeEntity
                    {
                        Id = request.RequestDto.EquipmentTypeId,
                        FullName = "Headset",
                        ShortName = "Headset",
                        OrderWeight = 0
                    },
                    BrokenStatus = EquipmentBrokenStatus.None
                }
            }.BuildMockDbSet().Object);

        _equipmentDbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>
            {
                new()
                {
                    UserId = request.RequestDto.ApproverId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com"
                }
            }.BuildMockDbSet().Object);

        _isEquipmentTypeExistDbQueryMock.Setup(x => x.ExecuteAsync(
                request.RequestDto.EquipmentTypeId,
                _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = await _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(
            expectedEquipmentDto,
            config => config.Excluding(x => x.InventoryNumber));

        _identityContract.Verify(
            x => x.IsUserManager(
                userId,
                _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task HandleApproverDoesNotExistsShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int approverId = 1;

        var request = new CreateEquipmentRequest.Command(new CreateEquipmentRequestDto()
        {
            Name = "Name",
            EquipmentTypeId = "Headset",
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            Location = EquipmentLocationType.Belarus,
            SerialNumber = "ABC-CBA",
            PurchasePrice = 3999.99m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 1999.99m,
            PurchaseDate = DateTime.UtcNow.Date,
            PurchasePlace = "PurchasePlace",
            GuaranteeDate = DateTime.UtcNow.Date.AddYears(2),
            Characteristics = "Characteristics",
            Comment = "Comment",
            InvoiceNumber = "DWEVDVQ3214EDA",
            Storage = LocationType.Vitebsk,
            RepairPlace = null,
            ApproverId = approverId,
            JiraTask = "qwe"
        });

        _equipmentDbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<EntityNotFoundException>().WithMessage(PortalMessages.ApproverDoesNotExist);
    }

    [Fact]
    public async Task HandlerWhenBrokenStatusIsRenovationAndRepairPlaceIsNullShouldThrowInvalidDataAppException()
    {
        // Arrange
        const int approverId = 1;

        var request = new CreateEquipmentRequest.Command(new CreateEquipmentRequestDto()
        {
            Name = "Name",
            EquipmentTypeId = "Headset",
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            Location = EquipmentLocationType.Belarus,
            SerialNumber = "ABC-CBA",
            PurchasePrice = 3999.99m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 1999.99m,
            PurchaseDate = DateTime.UtcNow.Date,
            PurchasePlace = "PurchasePlace",
            GuaranteeDate = DateTime.UtcNow.Date.AddYears(2),
            Characteristics = "Characteristics",
            Comment = "Comment",
            InvoiceNumber = "DWEVDVQ3214EDA",
            Storage = LocationType.Vitebsk,
            RepairPlace = null,
            ApproverId = approverId,
            JiraTask = "qwe"
        });

        _equipmentDbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = request.RequestDto.ApproverId,
                    FirstName = "test",
                    LastName = "test",
                    Email = "test@modsen-software.com"
                }
            }.BuildMockDbSet().Object);

        _identityContract.Setup(x => x.IsUserManager(
                It.IsAny<int>(),
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock.Setup(x => x.ExecuteAsync(request.RequestDto.EquipmentTypeId, _cancellationToken))
            .ReturnsAsync(true);

        _isSerialNumberExistingDbQueryMock.Setup(x => x.IsCorrespondsToBusiness(request.RequestDto.SerialNumber, null,
                _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>().WithMessage(PortalMessages.MandatoryRepairPlace);
    }
}
