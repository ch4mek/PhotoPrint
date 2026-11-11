using Microsoft.AspNetCore.OData.Query;
using MockQueryable.Moq;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Items.Requests;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Items.Requests;

public class GetPageableMerchItemsRequestTests
{
    private readonly GetPageableMerchItemsRequest _handler;
    private readonly Mock<IMerchDbContext> _dbContext = new();
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    private static readonly List<ItemEntity> OdpItemSet =
    [
        new ItemEntity
        {
            Id = 1,
            Name = "Item 1",
            Supplier = "Supplier 1",
            Status = MerchItemStatus.Available,
            IsClothing = false,
            Description = "description",
            PhotoFileId = "photofileid 1",
            ItemSizeMinAmounts =
            [
                new ()
                {
                    ItemId = 1,
                    SizeId = "1",
                    MerchSize = new MerchSizeEntity()
                    {
                        Id = "1",
                        OrderWeight = 100,
                        Name = "name",
                    },
                    WarehouseId = 1,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 1,
                        Location = "Warsaw",
                    },
                    MinAmount = 111
                },
                new ()
                {
                    ItemId = 1,
                    SizeId = "1",
                    MerchSize = new MerchSizeEntity()
                    {
                        Id = "1",
                        OrderWeight = 100,
                        Name = "name",
                    },
                    WarehouseId = 2,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 2,
                        Location = "New York",
                    },
                    MinAmount = 112
                },
            ]
        },
        new ItemEntity
        {
            Id = 2,
            Name = "Item 2",
            Supplier = "Supplier 2",
            Status = MerchItemStatus.Available,
            IsClothing = false,
            Description = "description",
            PhotoFileId = "photofileid 2",
            ItemSizeMinAmounts =
            [
                new ItemSizeMinAmountEntity
                {
                    ItemId = 2,
                    SizeId = "2",
                    MerchSize = new MerchSizeEntity()
                    {
                        Id = "2",
                        OrderWeight = 200,
                        Name = "name2",
                    },
                    WarehouseId = 1,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 1,
                        Location = "Warsaw",
                    },
                    MinAmount = 161
                },
                new ItemSizeMinAmountEntity
                {
                    ItemId = 2,
                    SizeId = "2",
                    MerchSize = new MerchSizeEntity() { Id = "2", OrderWeight = 200, Name = "name2" },
                    WarehouseId = 2,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 2,
                        Location = "New York",
                    },
                    MinAmount = 162
                },
            ]
        },
        new ItemEntity
        {
            Id = 3,
            Name = "Item 3",
            Supplier = "Supplier 3",
            Status = MerchItemStatus.Available,
            IsClothing = true,
            Description = "description",
            PhotoFileId = "photofileid 3",
            ItemSizeMinAmounts =
            [
                new ItemSizeMinAmountEntity
                {
                    ItemId = 3,
                    SizeId = "3",
                    MerchSize = new MerchSizeEntity()
                    {
                        Id = "3",
                        OrderWeight = 300,
                        Name = "name",
                    },
                    WarehouseId = 1,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 1,
                        Location = "Warsaw",
                    },
                    MinAmount = 1000
                },
                new ItemSizeMinAmountEntity
                {
                    ItemId = 3,
                    SizeId = "3",
                    MerchSize = new MerchSizeEntity()
                    {
                        Id = "3",
                        OrderWeight = 300,
                        Name = "name",
                    },
                    WarehouseId = 2,
                    Warehouse = new WarehouseEntity()
                    {
                        Id = 2,
                        Location = "New York",
                    },
                    MinAmount = 3020
                },
            ]
        },
    ];

    private static readonly List<OdpItemEntityDto> MappedOdpItems =
    [
        new OdpItemEntityDto
        {
            Id = 1,
            Name = "Item 1",
            Supplier = "Supplier 1",
            Status = MerchItemStatus.Available.ToString(),
            IsClothing = false,
            Description = "description",
            PhotoFileId = "photofileid 1",
            OdpItemSizeMinAmounts =
            [
                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "1",
                    SizeOrderWeight = 100,
                    WarehouseId = 1,
                    WarehouseLocation = "Warsaw",
                    MinAmount = 111
                },

                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "1",
                    SizeOrderWeight = 100,
                    WarehouseId = 2,
                    WarehouseLocation = "New York",
                    MinAmount = 112
                }

            ]
        },
        new OdpItemEntityDto
        {
            Id = 2,
            Name = "Item 2",
            Supplier = "Supplier 2",
            Status = MerchItemStatus.Available.ToString(),
            IsClothing = false,
            Description = "description",
            PhotoFileId = "photofileid 2",
            OdpItemSizeMinAmounts =
            [
                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "2",
                    SizeOrderWeight = 200,
                    WarehouseId = 1,
                    WarehouseLocation = "Warsaw",
                    MinAmount = 161
                },

                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "2",
                    SizeOrderWeight = 200,
                    WarehouseId = 2,
                    WarehouseLocation = "New York",
                    MinAmount = 162
                }

            ]
        },
        new OdpItemEntityDto
        {
            Id = 3,
            Name = "Item 3",
            Supplier = "Supplier 3",
            Status = MerchItemStatus.Available.ToString(),
            IsClothing = true,
            Description = "description",
            PhotoFileId = "photofileid 3",
            OdpItemSizeMinAmounts =
            [
                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "3",
                    SizeOrderWeight = 300,
                    WarehouseId = 1,
                    WarehouseLocation = "Warsaw",
                    MinAmount = 1000
                },

                new OdpItemSizeMinAmountEntityDto
                {
                    SizeId = "3",
                    SizeOrderWeight = 300,
                    WarehouseId = 2,
                    WarehouseLocation = "New York",
                    MinAmount = 3020
                }

            ]
        },
    ];

    public GetPageableMerchItemsRequestTests()
    {         
        _handler = new GetPageableMerchItemsRequest(_dbContext.Object);
    }

    [Fact]
    public async Task GetPageableMerchItemsWhenQueryParamsNullShouldReturnMerchItems()
    {
        // Arrange
        var odpItemsDbSet = OdpItemSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(odpItemsDbSet.Object);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpItemEntityDto>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "90",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        });      

        var request = new GetPageableMerchItemsRequest.Query(queryOptions,
            new GetPageableMerchItemsRequest.ExtendedQueryParams { StoragesIds = null, MinAmountSortAscending = null, StorageSortAscending = null });      

        var expected = new PageableListOfItems<OdpItemEntityDto>
        {
            Items = MappedOdpItems,
            Page = 1,
            PageSize = 90,
            Total = MappedOdpItems.Count
        };

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);        
    }

    [Fact]
    public async Task GetPageableMerchItemsWhenStorageIdsSpecifiedShouldReturnMerchItems()
    {
        // Arrange
        var itemsDbSet = OdpItemSet.BuildMockDbSet();

        _dbContext
            .Setup(x => x.Items)
            .Returns(itemsDbSet.Object);

        var queryOptions = ODataQueryOptionsMock.BuildMock<OdpItemEntityDto>(new Dictionary<string, string?>
        {
            [nameof(ODataRawQueryOptions.Top)] = "90",
            [nameof(ODataRawQueryOptions.Skip)] = "0"
        });

        var storageId = 1;

        var request = new GetPageableMerchItemsRequest.Query(queryOptions,
            new GetPageableMerchItemsRequest.ExtendedQueryParams { StoragesIds = [storageId], MinAmountSortAscending = null, StorageSortAscending = null });

        var odpItemDtos = new OdpItemEntityDto[MappedOdpItems.Count];
        MappedOdpItems.CopyTo(odpItemDtos);

        foreach (var item in odpItemDtos)
        {
            item.OdpItemSizeMinAmounts = item.OdpItemSizeMinAmounts.Where(i => i.WarehouseId == storageId).ToList();
        }

        var expected = new PageableListOfItems<OdpItemEntityDto>
        {
            Items = MappedOdpItems,
            Page = 1,
            PageSize = 90,
            Total = MappedOdpItems.Count
        };

        // Act
        var result = await _handler.Handle(request, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
}
