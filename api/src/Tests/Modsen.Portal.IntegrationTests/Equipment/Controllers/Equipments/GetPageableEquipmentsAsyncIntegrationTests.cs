using System.Globalization;
using System.Net;
using System.Text.Json;
using Bogus;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Equipment;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.Equipment.Controllers.Contracts.Input;
using Modsen.Portal.Equipment.Controllers.Contracts.Output;
using Modsen.Portal.Equipment.Requests.Equipment.Requests.RequestDto;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Identity;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
namespace Modsen.Portal.IntegrationTests.Equipment.Controllers.Equipments;

[Collection("GeneralCollection")]
public class GetPageableEquipmentsAsyncIntegrationTests
{
    private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    private readonly Faker<CreateEquipmentRequestDto> _createEquipmentInputFaker;

    public GetPageableEquipmentsAsyncIntegrationTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _jsonOptions = factory.JsonOptions;

        _createEquipmentInputFaker = new Faker<CreateEquipmentRequestDto>()
            .CustomInstantiator(_ => new CreateEquipmentRequestDto
            {
                SerialNumber = Guid.CreateVersion7().ToAlphabetic(),
                BrokenStatus = EquipmentBrokenStatus.None,
                InvoiceNumber = Guid.CreateVersion7().ToAlphabetic(),
                Name = Guid.CreateVersion7().ToAlphabetic(),
                Characteristics = Guid.CreateVersion7().ToAlphabetic(),
                ApproverId = _factory.CeoUserId!.Value,
                PurchasePlace = "Minsk",
                PurchaseCurrency = MoneyCurrencyType.Byn,
                PurchasePrice = 100,
                PurchasePriceUsd = 33.33m,
                PurchaseDate = DateTime.UtcNow.Date,
                Comment = Guid.CreateVersion7().ToAlphabetic(),
                GuaranteeDate = DateTime.UtcNow.Date.AddYears(1),
                Location = EquipmentLocationType.Belarus,
                EquipmentTypeId = "PcAccessoriesHdd",
                Storage = LocationType.Vitebsk,
                JiraTask = "qwe"
            });
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenRequestIsValidResponseWithExpectedEquipmentCountResult()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        const string searchPattern = "SearchPatternSerial";

        var createPatternLikeEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.SerialNumber, _ => searchPattern)
            .Generate();
        var createNotPatternLikeEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.SerialNumber, _ => Guid.CreateVersion7().ToAlphabetic())
            .Generate();

        for (var i = 0; i < 5; i++)
        {
            createPatternLikeEquipmentInput.SerialNumber = searchPattern + Guid.CreateVersion7().ToAlphabetic();

            await client.CreateEquipmentAsync(createPatternLikeEquipmentInput);
        }

        await client.CreateEquipmentAsync(createNotPatternLikeEquipmentInput);

        // Act
        var response = await client.GetPageableEquipmentAsync(10, 0, searchPattern);

        // Assert
        response.Should().NotBeNull();
        response.Page.Should().Be(1);
        response.Items.Count.Should().Be(5);
        response.Items.Should()
            .OnlyContain(equipment =>
                equipment.Id.ToString(CultureInfo.InvariantCulture).StartsWith(searchPattern) ||
                equipment.Name.StartsWith(searchPattern) ||
                equipment.EquipmentTypeId.StartsWith(searchPattern) ||
                equipment.SerialNumber.StartsWith(searchPattern) ||
                equipment.PurchasePlace.StartsWith(searchPattern) ||
                equipment.Location.StartsWith(searchPattern) ||
                equipment.Approver.FullName.StartsWith(searchPattern) ||
                (equipment.ActiveHolder != null &&
                    equipment.ActiveHolder.FullName.StartsWith(searchPattern)) ||
                (equipment.ActiveHolder != null &&
                    equipment.ActiveHolder.DepartmentId != null &&
                    equipment.ActiveHolder.DepartmentId.StartsWith(searchPattern)));
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenAuthedAsHeadOfDepartmentShouldReturnEquipmentAssignedToHeadOrUsersFromHeadDepartment()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var cto = await client.GetCurrentUser();

        var ceoId = _factory.CeoUserId!.Value;
        var netHeadOfDepartmentId = _factory.HeadOfDepartmentUserId!.Value;

        string[] serialNumbers = ["11", "22"];

        var equipments = new List<EquipmentOutput>();
        foreach (var serialNumber in serialNumbers)
        {
            var createEquipmentInput = _createEquipmentInputFaker.Clone()
                .RuleFor(x => x.SerialNumber, _ => serialNumber)
                .Generate();

            var createdEquipment = await client.CreateEquipmentAsync(createEquipmentInput);

            equipments.Add(createdEquipment!);
        }

        await client.CreateEquipmentHistoryAsync(
            equipments[0].Id,
            new AssignEquipmentInput
            {
                UserId = netHeadOfDepartmentId,
                ApproverId = cto.Id,
                IssueDate = DateTime.UtcNow.Date.AddDays(-1)
            });

        await client.CreateEquipmentHistoryAsync(
            equipments[1].Id,
            new AssignEquipmentInput
            {
                UserId = ceoId,
                ApproverId = cto.Id,
                IssueDate = DateTime.UtcNow.Date.AddDays(-1)
            });

        var expectedSerialNumbers = new List<string> { serialNumbers[0] };

        var filter = $"serialNumber in ({string.Join(",", serialNumbers.Select(n => $"'{n}'"))})";

        // Act
        await client.AuthAsHeadOfDepartment();

        var response = await client.GetPageableEquipmentWithFilterAsync(10, 0, filter);

        // Assert
        response!.Items
            .Select(x => x.SerialNumber)
            .Should()
            .BeEquivalentTo(expectedSerialNumbers);
    }

    [Theory]
    [InlineData("")]
    [InlineData("?")]
    [InlineData("?top=50")]
    [InlineData("?skip=0")]
    [InlineData("?top=0&skip=50")]
    [InlineData("?top=50&skip=-1")]
    [InlineData("?top=50&skip=15")]
    [InlineData("?top=26&skip=50")]
    [InlineData("?top=50&skip=100&select=id,firstName")]
    [InlineData("?top=50&skip=0&apply=groupby((position), aggregate(id with count as Count))")]
    public async Task GetPageableEquipmentsAsyncWhenRequestIsInvalidResponseWithBadRequestResult(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetPageableEquipmentWithQueryAsResponseMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("?top=10&skip=0")]
    [InlineData("?top=10&skip=50")]
    [InlineData("?top=50&skip=50&filter=contains(name,'LG110')")]
    [InlineData("?top=50&skip=50&orderby=name")]
    [InlineData("?top=50&skip=50&filter=contains(name,'LG110')&orderby=name")]
    [InlineData("?top=50&skip=50&filter=Name eq 'LG110'")]
    [InlineData("?$filter=contains(tolower(Name), tolower(''))&$orderby=Name asc&$top=50&$skip=0")]
    public async Task GetPageableEquipmentsAsyncWhenRequestIsValidResponseWithOk(string query)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        // Act
        var response = await client.GetPageableEquipmentWithQueryAsResponseMessageAsync(query);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("name", true)]
    [InlineData("EquipmentTypeFullName", false)]
    [InlineData("EquipmentTypeFullName", true)]
    [InlineData("purchasePriceUsd", false)]
    [InlineData("purchasePriceUsd", true)]
    [InlineData("purchaseDate", false)]
    [InlineData("purchaseDate", true)]
    public async Task GetPageableEquipmentsAsyncWithOrderByResponseEquipmentsInRightOrder(
        string orderBy,
        bool orderByDesc)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCto();

        var order = orderByDesc
            ? "desc"
            : "asc";

        var guid = Guid.CreateVersion7().ToAlphabetic();
        var createFirstEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.Characteristics, _ => guid)
            .RuleFor(x => x.Name, _ => "AAAA")
            .RuleFor(x => x.BrokenStatus, _ => EquipmentBrokenStatus.None)
            .RuleFor(x => x.PurchasePlace, _ => "AAAAA")
            .RuleFor(x => x.EquipmentTypeId, _ => "AccessPoint")
            .RuleFor(x => x.PurchasePriceUsd, _ => 100)
            .RuleFor(x => x.PurchaseDate, _ => DateTime.UtcNow.Date.AddDays(-3))
            .Generate();
        var createSecondEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.Characteristics, _ => guid)
            .RuleFor(x => x.Name, _ => "NNNN")
            .RuleFor(x => x.BrokenStatus, _ => EquipmentBrokenStatus.None)
            .RuleFor(x => x.PurchasePlace, _ => "NNNNNN")
            .RuleFor(x => x.EquipmentTypeId, _ => "Monitor")
            .RuleFor(x => x.PurchasePriceUsd, _ => 110)
            .RuleFor(x => x.PurchaseDate, _ => DateTime.UtcNow.Date.AddDays(-2))
            .Generate();
        var createThirdEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.Characteristics, _ => guid)
            .RuleFor(x => x.Name, _ => "ZZZZ")
            .RuleFor(x => x.BrokenStatus, _ => EquipmentBrokenStatus.None)
            .RuleFor(x => x.PurchasePlace, _ => "ZZZZZ")
            .RuleFor(x => x.EquipmentTypeId, _ => "WebCamera")
            .RuleFor(x => x.PurchasePriceUsd, _ => 120)
            .RuleFor(x => x.PurchaseDate, _ => DateTime.UtcNow.Date.AddDays(-1))
            .Generate();

        var firstEquipment = await client.CreateEquipmentAsync(createFirstEquipmentInput);

        var secondEquipment = await client.CreateEquipmentAsync(createSecondEquipmentInput);

        var thirdEquipment = await client.CreateEquipmentAsync(createThirdEquipmentInput);

        var equipmentOutputs = new List<EquipmentOutput> { firstEquipment!, secondEquipment!, thirdEquipment! };

        if (orderByDesc)
        {
            equipmentOutputs.Reverse();
        }

        await client.AuthAsCto();

        var query = $"?top=3&skip=0&filter=Characteristics eq '{guid}'&orderby={orderBy} {order}";

        // Act
        var response = await client.GetPageableEquipmentWithQueryAsResponseMessageAsync(query);

        // Assert
        response.Should().NotBeNull();
        //response!.Items.Count.Should().Be(3);
        //response.Items.Should().BeEquivalentTo(equipmentOutputs, config =>
        //{
        //    config.Excluding(x => x.Id);
        //    return config.WithStrictOrdering();
        //});
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncShouldReturnFilterOptionsWithActiveUsersAsActiveHolders()
    {
        //Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstUser = await client.CreateEmployee();

        var secondUser = await client.CreateEmployee();

        var thirdUser = await client.CreateEmployee();

        var expectedIds = new List<int> { firstUser!.Id, secondUser!.Id };

        await client.DeleteUserAsync(thirdUser!.Id);

        //Act
        var response = await client.GetPageableEquipmentAsync(10, 0);

        //Assert
        response!.Items.Should().NotBeNull();
        response.Items.Should().Contain(equipment =>
            equipment.ActiveHolder == null || expectedIds.Contains(equipment.ActiveHolder.Id));
    }

    [Theory]
    [InlineData("aaa")]
    [InlineData("12")]
    public async Task GetPageableEquipmentsAsyncWhenRequestContainsFilterWithStartWithResponseWithExpectedCEquipment(
            string searchPattern)
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var stringFieldsOfSearch = new List<string>
            {
                "name",
                "cast(id, Edm.String)",
                "EquipmentTypeId",
                "serialNumber",
                "purchasePlace",
                "ApproverFullName",
                "ActiveHolderFullName",
                "ActiveHolderDepartmentId"
            };

        var filterClause =
            $"({string.Join(" or ",
                stringFieldsOfSearch.Select(field => $"startswith(tolower({field}),tolower('{searchPattern}'))"))
            })";

        var equipment = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.SerialNumber, _ => searchPattern)
            .Generate();

        await client.CreateEquipmentAsync(equipment);

        // Act
        var response = await client.GetPageableEquipmentWithFilterAsync(10, 0, filterClause);

        // Assert
        response!.Items.Should().NotBeNull();

        response.Items.Should()
            .OnlyContain(e =>
                e.Id.ToString(CultureInfo.InvariantCulture).StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.Name.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.EquipmentTypeId.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.SerialNumber.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.PurchasePlace.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.Location.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                e.Approver.FullName.StartsWith(searchPattern, StringComparison.InvariantCultureIgnoreCase) ||
                (e.ActiveHolder != null &&
                    e.ActiveHolder.FullName.StartsWith(searchPattern,
                        StringComparison.InvariantCultureIgnoreCase)) ||
                (e.ActiveHolder != null &&
                    e.ActiveHolder.DepartmentId != null &&
                    e.ActiveHolder.DepartmentId.StartsWith(searchPattern,
                        StringComparison.InvariantCultureIgnoreCase)));
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenRequestContainsFilterInClauseShouldReturnValid()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var filterClause = "location in ('Poland')";

        var equipment = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.Location, _ => EquipmentLocationType.Poland);

        await client.CreateEquipmentAsync(equipment);

        // Act
        var response = await client.GetPageableEquipmentWithFilterAsync(10, 0, filterClause);

        // Assert
        response!.Items.Should().NotBeNull();

        response.Items.Should()
            .OnlyContain(e =>
                e.Location == EquipmentLocationType.Poland.GetDescription());
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenActiveAssignReturnDateIsTodayAndEquipmentNotIssuedOnThisReturnDateShouldReturnEquipmentWithoutAssign()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var issueDate = DateTime.UtcNow.Date.AddDays(-1);
        var returnDate = DateTime.UtcNow.Date;

        var equipment = await client.CreateEquipmentAsync(_createEquipmentInputFaker.Generate());

        await client.AuthAsEmployee();

        var employee = await client.GetCurrentUser();

        await client.AuthAsCeo();

        await client.CreateEquipmentAssignAsync(
            equipment!.Id,
            new AssignEquipmentInput
            {
                UserId = employee.Id,
                IssueDate = issueDate,
                ApproverId = _factory.CeoUserId!.Value
            });

        await client.CreateEquipmentHistoryWithReturnAsync(equipment.Id, new ReturnEquipmentInput { ReturnDate = returnDate });

        // Act
        var response = await client.GetPageableEquipmentWithEqSerialNumberAsync(1, 0, equipment.SerialNumber);

        //Assert
        response!.Items.Should().HaveCount(1);

        var equipmentOutput = response.Items.First();
        equipmentOutput.ActiveHolder.Should().BeNull();
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenThereIsAssignWithReturnDateIsTodayAndEquipmentHasSecondAssignIssuedOnThisReturnDateShouldReturnEquipmentWithSecondAssign()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var firstUserIssueDate = DateTime.UtcNow.Date.AddDays(-1);
        var firstUserReturnDate = DateTime.UtcNow.Date;

        var secondUserIssueDate = firstUserReturnDate;

        var equipment = await client.CreateEquipmentAsync(_createEquipmentInputFaker.Generate());

        await client.AuthAsEmployee();
        var firstUser = await client.GetCurrentUser();

        await client.AuthAsHeadOfDepartment();
        var secondUser = await client.GetCurrentUser();

        await client.AuthAsCeo();
        var ceo = await client.GetCurrentUser();

        await client.CreateEquipmentHistoryAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = firstUser.Id,
            IssueDate = firstUserIssueDate,
            ApproverId = ceo.Id
        });

        await client.CreateEquipmentHistoryWithReturnAsync(
            equipment.Id,
            new ReturnEquipmentInput { ReturnDate = firstUserReturnDate });

        await client.CreateEquipmentHistoryAsync(equipment.Id, new AssignEquipmentInput
        {
            UserId = secondUser.Id,
            IssueDate = secondUserIssueDate,
            ApproverId = _factory.CeoUserId!.Value
        });

        var expectedAssign = new EquipmentHolderOutput
        {
            Id = secondUser.Id,
            FullName = $"{secondUser.LastName} {secondUser.FirstName}",
            DepartmentId = secondUser.DepartmentId,
            IssueDate = secondUserIssueDate,
            ReturnDate = null,
            ApproverFullName = $"{ceo.LastName } {ceo.FirstName }",
            ApproverId = ceo.Id
        };

        // Act
        await client.AuthAsCeo();
        var response = await client.GetPageableEquipmentWithEqSerialNumberAsync(1, 0, equipment.SerialNumber);

        //Assert
        response!.Items.Should().HaveCount(1);

        var equipmentOutput = response.Items.First();
        equipmentOutput.ActiveHolder.Should().BeEquivalentTo(expectedAssign);
    }

    [Fact]
    public async Task GetPageableEquipmentsAsyncWhenThereIsOnlyAssignInFutureShouldReturnEquipmentWithThisAssign()
    {
        // Arrange
        using var client = _factory.CreateClient();
        await client.AuthAsCeo();

        var issueDate = DateTime.UtcNow.Date.AddDays(1);

        var serialNumber = Guid.CreateVersion7().ToAlphabetic();
        var createEquipmentInput = _createEquipmentInputFaker.Clone()
            .RuleFor(x => x.SerialNumber, _ => serialNumber)
            .Generate();

        var equipment = await client.CreateEquipmentAsync(createEquipmentInput);

        await client.AuthAsEmployee();
        var employee = await client.GetCurrentUser();

        await client.AuthAsCeo();
        var ceo = await client.GetCurrentUser();

        await client.CreateEquipmentHistoryAsync(equipment!.Id, new AssignEquipmentInput
        {
            UserId = employee.Id,
            IssueDate = issueDate,
            ApproverId = ceo.Id
        });

        var expectedAssign = new EquipmentHolderOutput
        {
            Id = employee.Id,
            FullName = $"{employee.LastName} {employee.FirstName}",
            DepartmentId = employee.DepartmentId,
            IssueDate = issueDate,
            ReturnDate = null,
            ApproverId = ceo.Id,
            ApproverFullName = $"{ceo.LastName} {ceo.FirstName}"
        };

        // Act
        var response = await client.GetPageableEquipmentWithEqSerialNumberAsync(1, 0, serialNumber);

        //Assert
        response!.Items.Should().HaveCount(1);
        var item = response.Items.First();
        item.ActiveHolder.Should().BeEquivalentTo(expectedAssign);
    }
}
