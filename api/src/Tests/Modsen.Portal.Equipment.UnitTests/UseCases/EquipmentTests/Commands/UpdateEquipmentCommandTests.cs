using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.Cache;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.Requests.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Dto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.DbQueries;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.Identity.Contract;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Commands;

public class UpdateEquipmentCommandTests
{
    private readonly Mock<IGetEquipmentByIdDbQuery> _getEquipmentByIdDbQueryMock = new();
    private readonly Mock<IIsEquipmentTypeExistsDbQuery> _isEquipmentTypeExistDbQueryMock = new();
    private readonly Mock<IIsSerialNumberExistsBusinessRule> _isSerialNumberExistingDbQueryMock = new();
    private readonly Mock<IIsInventoryNumberExistBusinessRule> _isInventoryNumberExistDbQueryMock = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();
    private readonly Mock<IEquipmentTypesCache> _typesCacheMock = new();

    private readonly Mock<IIdentityContract> _identityContract = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly UpdateEquipmentRequest _handler;

    public UpdateEquipmentCommandTests()
    {
        _handler = new UpdateEquipmentRequest(
            _isEquipmentTypeExistDbQueryMock.Object,
            _isSerialNumberExistingDbQueryMock.Object,
            _isInventoryNumberExistDbQueryMock.Object,
            _getEquipmentByIdDbQueryMock.Object,
            _typesCacheMock.Object,
            _dbContextMock.Object,
            _identityContract.Object
        );
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenApproverDoesNotExistsShouldThrowEntityNotFoundException()
    {
        // Arrange
        const int approverId = 1;

        var request = new UpdateEquipmentRequest.Command(new UpdateEquipmentRequestDto
        {
            Id = 1,
            Name = "Name",
            EquipmentTypeId = "Headset",
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.None,
            SerialNumber = "ABC-CBA",
            InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.ApproverDoesNotExist);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenApproverDoesNotHavePermissionShouldThrowActionNotAllowedException()
    {
        // Arrange
        const int approverId = 1;

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = 1,
                Name = "Name",
                EquipmentTypeId = "Headset",
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.None,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);


        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<ActionNotAllowedException>()
            .WithMessage(PortalMessages.ApproverDoesNotHavePermission);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenEquipmentTypeWasNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        const int approverId = 1;

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = 2,
                Name = "Name",
                EquipmentTypeId = "Headset",
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.Writtenoff,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock
            .Setup(x => x.ExecuteAsync(request.RequestDto.EquipmentTypeId, _cancellationToken))
            .ReturnsAsync(false);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.EquipmentTypeNotFound);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenEquipmentWasNotFoundThrowEntityNotFoundException()
    {
        // Arrange
        const int approverId = 1;

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = 2,
                Name = "Name",
                EquipmentTypeId = "Headset",
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.None,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock
            .Setup(x => x.ExecuteAsync(request.RequestDto.EquipmentTypeId, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>().BuildMockDbSet().Object);

        // Act
        var result = () => _handler.Handle(
            request,
            _cancellationToken);

        // Assert
        await result.Should()
            .ThrowAsync<EntityNotFoundException>()
            .WithMessage(PortalMessages.EquipmentNotFound);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenUpdateIsSuccessfulShouldReturnUpdatedEquipmentDto()
    {
        // Arrange
        const int approverId = 1;
        const int equipmentId = 2;
        const string equipmentTypeId = "Headset";

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = equipmentId,
                Name = "Name",
                EquipmentTypeId = equipmentTypeId,
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.None,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        var equipment = new EquipmentEntity
        {
            Id = request.RequestDto.Id,
            Name = "Name",
            TypeId = equipmentTypeId,
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.Broken,
            SerialNumber = "QWE-EWQ",
            InventoryNumber = "QWE-EWQ",
            PurchasePrice = 40.0m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 12.3m,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePlace = "old purchase place",
            GuaranteeDate = DateTime.UtcNow.AddYears(1),
            Characteristics = "KASJDLKdasdf",
            Comment = "old comment",
            InvoiceNumber = "1213213QWERQRQWE",
            Storage = LocationType.Minsk,
            RepairPlace = null,
            ApproverId = approverId,
            JiraTask = "qwe"
        };

        var expectedUpdatedEquipmentDto = new UpdatedEquipmentDto
        (
            Id: equipmentId,
            Name: request.RequestDto.Name,
            TypeId: request.RequestDto.EquipmentTypeId,
            Location: request.RequestDto.Location,
            BrokenStatus: request.RequestDto.BrokenStatus,
            SerialNumber: request.RequestDto.SerialNumber,
            InventoryNumber: request.RequestDto.InventoryNumber,
            PurchasePrice: request.RequestDto.PurchasePrice,
            PurchaseCurrency: request.RequestDto.PurchaseCurrency,
            PurchasePriceUsd: request.RequestDto.PurchasePriceUsd,
            PurchaseDate: request.RequestDto.PurchaseDate,
            PurchasePlace: request.RequestDto.PurchasePlace,
            GuaranteeDate: request.RequestDto.GuaranteeDate,
            Characteristics: request.RequestDto.Characteristics,
            Comment: request.RequestDto.Comment,
            ApproverId: approverId,
            Storage: LocationType.Vitebsk,
            RepairPlace: null,
            InvoiceNumber: request.RequestDto.InvoiceNumber,
            JiraTask: "qwe"
        );

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _typesCacheMock.Setup(x => x.GetEquipmentTypeById(It.IsAny<string>(), _cancellationToken))
            .ReturnsAsync(new EquipmentTypeRecord{Id = equipmentTypeId, FullName = "Headset", ShortName = "Headset"});

        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock
            .Setup(x => x.ExecuteAsync(request.RequestDto.EquipmentTypeId, _cancellationToken))
            .ReturnsAsync(true);

        _dbContextMock.Setup(x => x.Equipments)
            .Returns(new List<EquipmentEntity>()
            {
                equipment
            }.BuildMockDbSet().Object);

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(It.IsAny<int>(), _cancellationToken))
            .ReturnsAsync(equipment);

        _dbContextMock.Setup(x => x.EquipmentTypes)
            .Returns(
                new List<EquipmentTypeEntity>
                {
                    new() { Id = equipmentTypeId, FullName = "Headset", ShortName = "Headset", OrderWeight = 0}
                }.BuildMockDbSet().Object);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedUpdatedEquipmentDto);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenBrokenStatusIsRenovationAndRepairPlaceIsNullShouldThrowInvalidDataAppException()
    {
        // Arrange
        const int approverId = 1;
        const int equipmentId = 2;
        const string equipmentTypeId = "Headset";

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = equipmentId,
                Name = "Name",
                EquipmentTypeId = equipmentTypeId,
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.Renovation,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);


        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.MandatoryRepairPlace);
    }

    [Fact]
    public async Task UpdateEquipmentAsyncWhenBrokenStatusIsWrittenOffAndEquipmentIsAssignedShouldThrowInvalidDataAppException()
    {
        // Arrange
        const int approverId = 1;
        const int equipmentId = 2;
        const string equipmentTypeId = "Headset";

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = equipmentId,
                Name = "Name",
                EquipmentTypeId = equipmentTypeId,
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.Writtenoff,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock.Setup(x => x.ExecuteAsync(equipmentTypeId, _cancellationToken))
            .ReturnsAsync(true);

        var equipment = new EquipmentEntity()
        {
            Id = request.RequestDto.Id,
            Name = "Name",
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.Broken,
            SerialNumber = "QWE-EWQ",
            InventoryNumber = "QWE-EWQ",
            PurchasePrice = 40.0m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 12.3m,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePlace = "old purchase place",
            GuaranteeDate = DateTime.UtcNow.AddYears(1),
            Characteristics = "KASJDLKdasdf",
            Comment = "old comment",
            InvoiceNumber = "1213213QWERQRQWE",
            Storage = LocationType.Minsk,
            RepairPlace = null,
            ApproverId = approverId,
            Assigns =
            [
                new EquipmentAssignEntity()
                {
                    Id = 1,
                    IssueDate = DateTime.UtcNow,
                    ReturnDate = null,
                    IsDeleted = false,
                    AssignedToUserId = 25,
                    ApproverId = approverId
                }
            ]
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(request.RequestDto.Id, _cancellationToken))
            .ReturnsAsync(equipment);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.BrokenStatusConflict);
    }

        [Fact]
    public async Task UpdateEquipmentAsyncWhenExistingInventoryNumberAssignedShouldThrowInvalidDataAppException()
    {
        // Arrange
        const int approverId = 1;
        const int equipmentId = 2;
        const string equipmentTypeId = "Headset";

        var request = new UpdateEquipmentRequest.Command(
            new UpdateEquipmentRequestDto
            {
                Id = equipmentId,
                Name = "Name",
                EquipmentTypeId = equipmentTypeId,
                Location = EquipmentLocationType.Belarus,
                BrokenStatus = EquipmentBrokenStatus.Writtenoff,
                SerialNumber = "ABC-CBA",
                InventoryNumber = "ABC-CBA",
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

        _dbContextMock.Setup(x => x.EquipmentUsers)
            .Returns(new List<EquipmentUser>()
            {
                new()
                {
                    UserId = approverId,
                    FirstName = "CEO",
                    LastName = "TEST",
                    Email = "test@modsen-software.com",
                    IsActive = true
                }
            }.BuildMockDbSet().Object);

        _identityContract
            .Setup(x => x.IsUserManager(
                request.RequestDto.ApproverId,
                _cancellationToken))
            .ReturnsAsync(true);

        _isEquipmentTypeExistDbQueryMock.Setup(x => x.ExecuteAsync(equipmentTypeId, _cancellationToken))
            .ReturnsAsync(true);

        var equipment = new EquipmentEntity()
        {
            Id = request.RequestDto.Id,
            Name = "Name",
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.Broken,
            SerialNumber = "QWE-EWQ",
            InventoryNumber = "ABC-BCA",
            PurchasePrice = 40.0m,
            PurchaseCurrency = MoneyCurrencyType.Byn,
            PurchasePriceUsd = 12.3m,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePlace = "old purchase place",
            GuaranteeDate = DateTime.UtcNow.AddYears(1),
            Characteristics = "KASJDLKdasdf",
            Comment = "old comment",
            InvoiceNumber = "1213213QWERQRQWE",
            Storage = LocationType.Minsk,
            RepairPlace = null,
            ApproverId = approverId,
            Assigns =
            [
                new EquipmentAssignEntity()
                {
                    Id = 1,
                    IssueDate = DateTime.UtcNow,
                    ReturnDate = null,
                    IsDeleted = false,
                    AssignedToUserId = 25,
                    ApproverId = approverId
                }
            ]
        };

        _getEquipmentByIdDbQueryMock.Setup(x => x.ExecuteAsync(request.RequestDto.Id, _cancellationToken))
            .ReturnsAsync(equipment);

        // Act
        var result = () => _handler.Handle(request, _cancellationToken);

        // Assert
        await result.Should().ThrowAsync<InvalidDataAppException>()
            .WithMessage(PortalMessages.BrokenStatusConflict);
    }
}
