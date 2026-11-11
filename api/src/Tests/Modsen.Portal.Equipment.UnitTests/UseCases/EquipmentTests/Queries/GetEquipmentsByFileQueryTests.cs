using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Equipment.DataAccess.Npgsql;
using Modsen.Portal.Equipment.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Equipment.DataAccess.Npgsql.OData;
using Modsen.Portal.Equipment.Requests.Core.BusinessRules;
using Modsen.Portal.Equipment.Requests.Equipment.Core.CoreDto;
using Modsen.Portal.Equipment.Requests.Equipment.Core.Exporters;
using Modsen.Portal.Equipment.Requests.Equipment.Requests;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings;
using Modsen.Portal.Equipment.Requests.EquipmentPortalSettings.Dto;

namespace Modsen.Portal.Equipment.UnitTests.UseCases.EquipmentTests.Queries;

public class GetEquipmentsByFileQueryTests
{
    private readonly Mock<ICanAccessUnassignedEquipmentBusinessRule> _isAvailableEquipmentsWithoutAssignersMock = new();
    private readonly Mock<IEquipmentPortalSettingsAccessor> _settingsAccessorMock = new();
    private readonly Mock<IAuthenticatedUserContext> _authenticatedUserContextMock = new();
    private readonly Mock<IEquipmentToExcelExporter> _equipmentToExcelExporter = new();
    private readonly Mock<IEquipmentDbContext> _dbContextMock = new();

    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly GetEquipmentsByFileRequest _handler;

    public GetEquipmentsByFileQueryTests()
    {
        _handler = new GetEquipmentsByFileRequest(
            _isAvailableEquipmentsWithoutAssignersMock.Object,
            _settingsAccessorMock.Object,
            _authenticatedUserContextMock.Object,
            _equipmentToExcelExporter.Object,
            _dbContextMock.Object);

        _settingsAccessorMock.Setup(x => x.GetSettings(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new EquipmentPortalSettingsDto
            {
                SummaryReportWorksheet = "Summary Report",
                BaseFontSize = 10,
                EquipmentsFileName = "Equipment",
                DefaultColumnWidthInSymbols = 30,
                DefaultDateFormat = "dd/MM/yyyy",
                FileExportDateFormat = "dd.MM.yyyy"
            });
    }

    [Fact]
    public async Task GetEquipmentsByFileWhenTryToGetExcelFileReturnStreamWithFileOfExcelFormat()
    {
        // Arrange
        var equipment = new EquipmentEntity
        {
            Id = 1,
            Name = "Name",
            Characteristics = "Characteristics",
            PurchasePlace = "PurchasePlace",
            SerialNumber = "SerialNumber",
            Location = EquipmentLocationType.Belarus,
            BrokenStatus = EquipmentBrokenStatus.Renovation,
            PurchaseCurrency = MoneyCurrencyType.Usd,
            Comment = "comment",
            TypeId = "type",
            Type = new EquipmentTypeEntity
            {
                Id = "type",
                FullName = "Type",
                ShortName = "T",
                OrderWeight = 0
            },
            ApproverId = 123,
            Approver = new EquipmentUser
            {
                UserId = 123,
                FirstName = "Test",
                LastName = "Head of Department",
                Email = "email"
            },
            Assigns = new List<EquipmentAssignEntity>
            {
                new EquipmentAssignEntity
                {
                    AssignedToUserId = 99,
                    IssueDate = new DateTime(2023, 04, 10, 0, 0, 0, DateTimeKind.Utc),
                    ReturnDate = null,
                    User = new EquipmentUser
                    {
                        FirstName = "First",
                        LastName = "Last",
                        DepartmentId = "Net",
                        Email = "email 2"
                    }
                }
            }
        };

        var streamWithExcelFile = Array.Empty<byte>();

        var equipments = new List<EquipmentEntity> { equipment };

        var queryParameters = new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "50",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        };

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpEquipment>(queryParameters);

        _dbContextMock.Setup(mock => mock.EquipmentDepartments)
            .Returns(new List<EquipmentDepartmentEntity>().BuildMockDbSet().Object);

        _dbContextMock.Setup(mock => mock.Equipments)
            .Returns(equipments.BuildMockDbSet().Object);

        var settings = await _settingsAccessorMock.Object.GetSettings(_cancellationToken);

        var exportModel = new EquipmentsToExcelExportCoreDto
        {
            Equipments = It.IsAny<IEnumerable<EquipmentToExcelExportCoreDto>>(),
            ShouldPrintPurchaseDetails = true,
            DefaultDateFormat = settings.DefaultDateFormat
        };

        _equipmentToExcelExporter
            .Setup(x => x.Export(exportModel, _cancellationToken))
            .ReturnsAsync(streamWithExcelFile);

        _authenticatedUserContextMock
            .Setup(x => x.GetRoles())
            .Returns([PortalRoleNames.SystemAdministrator]);

        _authenticatedUserContextMock
            .Setup(x => x.DepartmentId)
            .Returns("Net");

        _isAvailableEquipmentsWithoutAssignersMock
            .Setup(a => a.IsCorrespondsToBusiness(_authenticatedUserContextMock.Object))
            .Returns(true);

        var request = new GetEquipmentsByFileRequest.Query(queryOptions, DocumentType.Excel);

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
