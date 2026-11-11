using MockQueryable.Moq;
using Modsen.Portal.Core.Merch;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Merch.DataAccess.Npgsql;
using Modsen.Portal.Merch.DataAccess.Npgsql.Entities;
using Modsen.Portal.Merch.Requests.Kits.Core.Calculators;
using Modsen.Portal.Merch.Requests.Kits.Core.CoreDto;
using Modsen.Portal.Merch.Requests.Kits.Core.Loaders;
using Modsen.Portal.Merch.Requests.Kits.Core.Resolvers;
using Modsen.Portal.Merch.Requests.Kits.Requests;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;
using Moq;

namespace Modsen.Portal.Merch.UnitTests.Kits.Requests;

public class GetPageableKitsRequestTests
{
    private readonly Mock<IKitPossibleAmountCalculator> _kitPossibleAmountCalculator = new();
    private readonly Mock<ICompoundSizesInStockResolver> _compoundSizesInStockLoader = new();
    private readonly Mock<IKitsWithCompoundsLoader> _kitsWithCompoundsLoader = new();
    private readonly Mock<IMerchDbContext> _dbContextMock = new();

    private readonly CancellationToken _token = CancellationToken.None;

    private readonly GetPageableKitsRequest _request;

    public GetPageableKitsRequestTests()
    {
        _request = new GetPageableKitsRequest(
            _compoundSizesInStockLoader.Object,
            _kitsWithCompoundsLoader.Object,
            _dbContextMock.Object,
            _kitPossibleAmountCalculator.Object);
    }

    [Fact]
    public async Task HandleWhenLoaderReturnsEmptyKitsListReturnsEmptyPageableList()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto();
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>().BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        var expectedResult = EmptyPageableListOfItems<KitStorageDto>.Value;
        
        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenFilterOnIdReturnsValidId()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            Filters =
            [
                new QueryFilterDto { FilterColumn = "id", FilterValue = "1" }
            ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenFilterOnNameReturnsValidId()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            Filters =
            [
                new QueryFilterDto { FilterColumn = "name", FilterValue = "2" }
            ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 2 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenFilterOnDescriptionReturnsValidId()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            Filters =
            [
                new QueryFilterDto { FilterColumn = "description", FilterValue = "Modsen" }
            ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1",
                    Description = "modsen kit"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2",
                    Description = "Some kit"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenFilterOnCompoundReturnsValidId()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            Filters =
            [
                new QueryFilterDto { FilterColumn = "compound", FilterValue = "hood" }
            ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "test",
                    KitItems =
                    [
                        new() { Item = new ItemEntity() { Name = "Hoodie" } }
                    ]
                },
                new()
                {
                    Id = 2, Name = "test",
                    KitItems =
                    [
                        new() { Item = new ItemEntity() { Name = "Postcard" } }
                    ]
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenFilterOnUnknownColumnReturnsAllIds()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            Filters =
            [
                new QueryFilterDto { FilterColumn = "unknownColumn", FilterValue = "2" }
            ]
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1, 2 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenOrderingOnIdAscReturnsIdsOnAscendingOrder()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            SortColumn = "id",
            SortOrder = "asc"
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1, 2 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenOrderingOnIdDescReturnsIdsOnDescendingOrder()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            SortColumn = "id",
            SortOrder = "desc"
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "Kit 1"
                },
                new()
                {
                    Id = 2,
                    Name = "Kit 2"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 2, 1 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenOrderingOnNameAscReturnsIdsOnAscendingOrder()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            SortColumn = "name",
            SortOrder = "asc"
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "ZKit"
                },
                new()
                {
                    Id = 2,
                    Name = "AKit"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 2, 1 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task GetKitsIdsToLoadAsyncWhenOrderingOnNameDescReturnsIdsOnDescendingOrder()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto
        {
            SortColumn = "name",
            SortOrder = "desc"
        };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([]);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>
            {
                new()
                {
                    Id = 1,
                    Name = "ZKit"
                },
                new()
                {
                    Id = 2,
                    Name = "AKit"
                }
            }.BuildMockDbSet().Object);

        //Act
        await _request.Handle(request, _token);

        //Assert
        _kitsWithCompoundsLoader.Verify(
            loader => loader.LoadAsync(new [] { 1, 2 }, It.IsAny<int[]?>(), _token),
            Times.Once);
    }

    [Fact]
    public async Task HandleWhenKitDoesNotContainClothingReturnsValidResult()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto();
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test", } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = false,
                    AllStorages = ["Minsk", "Vitebsk", "Warsaw"],
                    Compounds =
                    [
                        new() { Id = 1, Name = "Postcard", AmountInKit = 1, IsClothing = false },
                        new() { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                },

                new()
                {
                    ItemId = 2,
                    Amount = 19,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 12,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))
            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 2,
                        StorageName = "Minsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Vitebsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 3,
                        StorageName = "Warsaw",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.Available
                    }
                ],
                 TotalPossibleKits = 10
            });

        var expectedResult = PageableListOfItemsExtensions.ToPageable([new KitStorageDto
            {
                Id = 1,
                Name = "Modsen newcomer kit",
                PhotoFileId = null,
                Description = "Modsen newcomer kit",
                ContainsClothing = false,
                AllStorages = [ "Minsk", "Vitebsk", "Warsaw" ],
                Compounds =
                [
                    new() { Name = "Postcard", AmountInKit = 1  },
                    new() { Name = "Cup", AmountInKit = 2 }
                ],
                Status = StorageAvailabilityStatus.RunningOut,
                TotalPossibleKits = 10,
                StorageAvailabilityStatuses =
                [
                    new() { StorageName = "Minsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Vitebsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Warsaw", Status = StorageAvailabilityStatus.Available}
                ],
                PossibleKits =
                [
                    new() { PossibleKitAmount = 2, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Minsk"},
                    new() { PossibleKitAmount = 5, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Vitebsk"},
                    new() { PossibleKitAmount = 3, SizeId = "One", Status = StorageAvailabilityStatus.Available, StorageName = "Warsaw"}
                ]
            }])
            .WithPaging(page: 1, pageSize: 10)
            .WithTotal(1);

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenFilterOnPossibleKitAmountAndTotalPossibleAmountEqualsToAmountMinReturnsKit()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto(){ AmountMin = 10 };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test" } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = false,
                    AllStorages = ["Minsk", "Vitebsk", "Warsaw"],
                    Compounds =
                    [
                        new() { Id = 1, Name = "Postcard", AmountInKit = 1, IsClothing = false },
                        new() { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                },

                new()
                {
                    ItemId = 2,
                    Amount = 19,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 12,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))
            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 2,
                        StorageName = "Minsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Vitebsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 3,
                        StorageName = "Warsaw",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.Available
                    }
                ],
                TotalPossibleKits = 10,
            });

        var expectedResult = PageableListOfItemsExtensions.ToPageable([new KitStorageDto
            {
                Id = 1,
                Name = "Modsen newcomer kit",
                PhotoFileId = null,
                Description = "Modsen newcomer kit",
                ContainsClothing = false,
                AllStorages = [ "Minsk", "Vitebsk", "Warsaw" ],
                Compounds =
                [
                    new() { Name = "Postcard", AmountInKit = 1  },
                    new() { Name = "Cup", AmountInKit = 2 }
                ],
                Status = StorageAvailabilityStatus.RunningOut,
                TotalPossibleKits = 10,
                StorageAvailabilityStatuses =
                [
                    new() { StorageName = "Minsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Vitebsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Warsaw", Status = StorageAvailabilityStatus.Available}
                ],
                PossibleKits =
                [
                    new() { PossibleKitAmount = 2, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Minsk"},
                    new() { PossibleKitAmount = 5, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Vitebsk"},
                    new() { PossibleKitAmount = 3, SizeId = "One", Status = StorageAvailabilityStatus.Available, StorageName = "Warsaw"}
                ]
            }])
            .WithPaging(page: 1, pageSize: 10)
            .WithTotal(1);

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenFilterOnPossibleKitAmountAndTotalPossibleAmountLessThanAmountMinReturnsEmptyList()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto(){ AmountMin = 11 };
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test" } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = false,
                    AllStorages = ["Minsk", "Vitebsk", "Warsaw"],
                    Compounds =
                    [
                        new() { Id = 1, Name = "Postcard", AmountInKit = 1, IsClothing = false },
                        new() { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                },

                new()
                {
                    ItemId = 2,
                    Amount = 19,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 12,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))
            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 2,
                        StorageName = "Minsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Vitebsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 3,
                        StorageName = "Warsaw",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.Available
                    }
                ]
            });

        var expectedResult = EmptyPageableListOfItems<KitStorageDto>.Value;

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenFilterOnStatusIdAndKitStatusDoesNotContainsReturnsEmptyList()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto(){ StatusIds = [ StorageAvailabilityStatus.Available ]};
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test" } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = false,
                    AllStorages = ["Minsk", "Vitebsk", "Warsaw"],
                    Compounds =
                    [
                        new() { Id = 1, Name = "Postcard", AmountInKit = 1, IsClothing = false },
                        new() { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                },

                new()
                {
                    ItemId = 2,
                    Amount = 19,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 12,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))
            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 2,
                        StorageName = "Minsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Vitebsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 3,
                        StorageName = "Warsaw",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.Available
                    }
                ]
            });

        var expectedResult = EmptyPageableListOfItems<KitStorageDto>.Value;

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenFilterOnStatusIdAndKitStatusContainsReturnsKit()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto(){ StatusIds = [StorageAvailabilityStatus.RunningOut]};
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test" } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = false,
                    AllStorages = ["Minsk", "Vitebsk", "Warsaw"],
                    Compounds =
                    [
                        new() { Id = 1, Name = "Postcard", AmountInKit = 1, IsClothing = false },
                        new() { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                },

                new()
                {
                    ItemId = 2,
                    Amount = 19,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 5,
                    SizeId = "One",
                    StorageName = "Vitebsk",
                    StorageId = 2,
                    SizeOrderWeight = 0
                },
                new()
                {
                    ItemId = 2,
                    Amount = 12,
                    SizeId = "One",
                    StorageName = "Warsaw",
                    StorageId = 3,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))
            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 2,
                        StorageName = "Minsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Vitebsk",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 3,
                        StorageName = "Warsaw",
                        SizeId = "One",
                        Status = StorageAvailabilityStatus.Available
                    }
                ],
                TotalPossibleKits = 10,
            });

        var expectedResult = PageableListOfItemsExtensions.ToPageable([new KitStorageDto
            {
                Id = 1,
                Name = "Modsen newcomer kit",
                PhotoFileId = null,
                Description = "Modsen newcomer kit",
                ContainsClothing = false,
                AllStorages = [ "Minsk", "Vitebsk", "Warsaw" ],
                Compounds =
                [
                    new() { Name = "Postcard", AmountInKit = 1  },
                    new() { Name = "Cup", AmountInKit = 2 }
                ],
                Status = StorageAvailabilityStatus.RunningOut,
                TotalPossibleKits = 10,
                StorageAvailabilityStatuses =
                [
                    new() { StorageName = "Minsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Vitebsk", Status = StorageAvailabilityStatus.RunningOut},
                    new() { StorageName = "Warsaw", Status = StorageAvailabilityStatus.Available}
                ],
                PossibleKits =
                [
                    new() { PossibleKitAmount = 2, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Minsk"},
                    new() { PossibleKitAmount = 5, SizeId = "One", Status = StorageAvailabilityStatus.RunningOut, StorageName = "Vitebsk"},
                    new() { PossibleKitAmount = 3, SizeId = "One", Status = StorageAvailabilityStatus.Available, StorageName = "Warsaw"}
                ]
            }])
            .WithPaging(page: 1, pageSize: 10)
            .WithTotal(1);

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Fact]
    public async Task HandleWhenKitContainsClothingReturnsValidResult()
    {
        //Arrange
        var queryOptions = new KitsQueryOptionsDto();
        var requestDto = new GetPageableKitsDto { Top = 10, Skip = 0, QueryOptions = queryOptions };
        var request = new GetPageableKitsRequest.Query(requestDto);

        _dbContextMock
            .Setup(context => context.Kits)
            .Returns(new List<KitEntity>{ new(){ Id = 1, Name = "test" } }.BuildMockDbSet().Object);

        _dbContextMock
            .Setup(context => context.KitMinAmounts)
            .Returns(new List<KitMinAmountEntity>
            {
                new() { KitId = 1, WarehouseId = 1, MinAmount = 5 },
                new() { KitId = 1, WarehouseId = 2, MinAmount = 10 },
                new() { KitId = 1, WarehouseId = 3, MinAmount = 1 }
            }.BuildMockDbSet().Object);

        _kitsWithCompoundsLoader
            .Setup(loader => loader.LoadAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    Id = 1,
                    Name = "Modsen newcomer kit",
                    PhotoFileId = null,
                    Description = "Modsen newcomer kit",
                    ContainsClothing = true,
                    AllStorages = ["Minsk"],
                    Compounds =
                    [
                        new KitCompoundCoreDto { Id = 1, Name = "Hoodie", AmountInKit = 1, IsClothing = true },
                        new KitCompoundCoreDto { Id = 2, Name = "Cup", AmountInKit = 2, IsClothing = false }
                    ]
                }
            ]);

        _compoundSizesInStockLoader
            .Setup(loader => loader.ResolveAsync(It.IsAny<int[]>(), It.IsAny<int[]?>(), _token))
            .ReturnsAsync([
                new()
                {
                    ItemId = 1,
                    Amount = 10,
                    SizeId = "XS",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 100
                },
                new()
                {
                    ItemId = 1,
                    Amount = 15,
                    SizeId = "S",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 200
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "M",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 300
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "L",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 400
                },
                new()
                {
                    ItemId = 1,
                    Amount = 5,
                    SizeId = "XL",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 500
                },
                new()
                {
                    ItemId = 1,
                    Amount = 1,
                    SizeId = "2XL",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 600
                },
                new()
                {
                    ItemId = 1,
                    Amount = 1,
                    SizeId = "3XL",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 700
                },
                new()
                {
                    ItemId = 1,
                    Amount = 1,
                    SizeId = "4XL",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 800
                },

                new()
                {
                    ItemId = 2,
                    Amount = 8,
                    SizeId = "One",
                    StorageName = "Minsk",
                    StorageId = 1,
                    SizeOrderWeight = 0
                }
            ]);

        _kitPossibleAmountCalculator
            .Setup(calculator => calculator.Calculate(
                It.IsAny<KitWithCompoundsCoreDto>(),
                It.IsAny<List<KitMinAmountEntity>>(),
                It.IsAny<List<CompoundSizeAmountInStockCoreDto>>()))

            .Returns(new KitTotalWithPossibleInStorageCoreDto
            {
                PossibleKits =
                [
                    new()
                    {
                        PossibleKitAmount = 10,
                        StorageName = "Minsk",
                        SizeId = "XS",
                        Status = StorageAvailabilityStatus.Available
                    },
                    new()
                    {
                        PossibleKitAmount = 15,
                        StorageName = "Minsk",
                        SizeId = "S",
                        Status = StorageAvailabilityStatus.Available
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Minsk",
                        SizeId = "M",
                        Status = StorageAvailabilityStatus.Available
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Minsk",
                        SizeId = "L",
                        Status = StorageAvailabilityStatus.Available
                    },
                    new()
                    {
                        PossibleKitAmount = 5,
                        StorageName = "Minsk",
                        SizeId = "XL",
                        Status = StorageAvailabilityStatus.Available
                    },
                    new()
                    {
                        PossibleKitAmount = 1,
                        StorageName = "Minsk",
                        SizeId = "2XL",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 1,
                        StorageName = "Minsk",
                        SizeId = "3XL",
                        Status = StorageAvailabilityStatus.RunningOut
                    },
                    new()
                    {
                        PossibleKitAmount = 1,
                        StorageName = "Minsk",
                        SizeId = "4XL",
                        Status = StorageAvailabilityStatus.RunningOut
                    }
                ],
                TotalPossibleKits = 10
            });

        var expectedResult = PageableListOfItemsExtensions.ToPageable([new KitStorageDto
            {
                Id = 1,
                Name = "Modsen newcomer kit",
                PhotoFileId = null,
                Description = "Modsen newcomer kit",
                ContainsClothing = true,
                AllStorages = [ "Minsk" ],
                Compounds =
                [
                    new() { Name = "Hoodie", AmountInKit = 1  },
                    new() { Name = "Cup", AmountInKit = 2 }
                ],
                Status = StorageAvailabilityStatus.RunningOut,
                TotalPossibleKits = 10,
                StorageAvailabilityStatuses =
                [
                    new() { StorageName = "Minsk", Status = StorageAvailabilityStatus.RunningOut}
                ],
                PossibleKits =
                [
                    new() { PossibleKitAmount = 10, StorageName = "Minsk", SizeId = "XS", Status = StorageAvailabilityStatus.Available },
                    new() { PossibleKitAmount = 15, StorageName = "Minsk", SizeId = "S", Status = StorageAvailabilityStatus.Available },
                    new() { PossibleKitAmount = 5, StorageName = "Minsk", SizeId = "M", Status = StorageAvailabilityStatus.Available },
                    new() { PossibleKitAmount = 5, StorageName = "Minsk", SizeId = "L", Status = StorageAvailabilityStatus.Available },
                    new() { PossibleKitAmount = 5, StorageName = "Minsk", SizeId = "XL", Status = StorageAvailabilityStatus.Available },
                    new() { PossibleKitAmount = 1, StorageName = "Minsk", SizeId = "2XL", Status = StorageAvailabilityStatus.RunningOut },
                    new() { PossibleKitAmount = 1, StorageName = "Minsk", SizeId = "3XL", Status = StorageAvailabilityStatus.RunningOut },
                    new() { PossibleKitAmount = 1, StorageName = "Minsk", SizeId = "4XL", Status = StorageAvailabilityStatus.RunningOut }
                ]
            }])
            .WithPaging(page: 1, pageSize: 10)
            .WithTotal(1);

        //Act
        var result = await _request.Handle(request, _token);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }
}
