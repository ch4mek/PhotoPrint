using System.Net;
using System.Text.Json;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Merch;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.Shared;
using Modsen.Portal.IntegrationTests.TestFramework.HttpClientExtensions.TeamboardHttpClientExtensions;
using Modsen.Portal.Merch.Controllers.Contracts.Input;
using Modsen.Portal.Merch.Requests.Items.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Kits.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Storage.Requests.RequestDto;
using Modsen.Portal.Merch.Requests.Transactions.Requests.RequestDto;
namespace Modsen.Portal.IntegrationTests.Merch.Transactions
{
    [Collection("MerchCollection")]
    public class TransferTransactionsTests
    {
        private readonly IntegrationTestFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

        public TransferTransactionsTests(IntegrationTestFactory factory)
        {
            _factory = factory;
        _jsonOptions = factory.JsonOptions;
        }

        [Fact]
        public async Task CreateItemTransactionsAsyncWithoutClothReturnOk()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var storageMinskId = 1;
            var storageVitebskId = 2;

            var createdItem = await client.CreateMerchItemAsHttpMessage(
                isClothing: false,
                Guid.CreateVersion7().ToAlphabetic()
            ).WithResponseType<ItemDto>();

            // Act
            await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = storageMinskId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "One",
                        Count = 10
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Receipt
            }).WithResponseType<int>();

            await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = storageMinskId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "One",
                        Count = 3
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseRecipientId = storageVitebskId
            }).WithResponseType<int>();

            var transaction = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = createdItem.Id,
                IdTo = createdItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var storageMinsk = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [storageMinskId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = createdItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            var storageVitebsk = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [storageVitebskId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = createdItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            // Assert
            transaction.Items.First().Transactions.Should().NotBeEmpty();
            storageMinsk.Items.First().TotalAmount.Should().Be(7);
            storageVitebsk.Items.First().TotalAmount.Should().Be(3);
        }

        [Fact]
        public async Task CreateItemTransactionsAsyncWithClothReturnOk()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var storageMinskId = 1;
            var storageVitebskId = 2;

            var createdItem = await client.CreateMerchItemAsHttpMessage(
                isClothing: true,
                Guid.CreateVersion7().ToAlphabetic()
            ).WithResponseType<ItemDto>();

            // Act
            await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = storageMinskId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 3
                    },
                    new()
                    {
                        SizeId = "XL",
                        Count = 5
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Receipt
            }).WithResponseType<int>();

            await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = storageMinskId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 2
                    },
                    new()
                    {
                        SizeId = "XL",
                        Count = 2
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseRecipientId = storageVitebskId,
            }).WithResponseType<int>();

            var transaction = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = createdItem.Id,
                IdTo = createdItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var minskStorage = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [storageMinskId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = createdItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            var vitebskStorage = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [storageVitebskId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = createdItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            // Assert
            transaction.Items.First().Transactions.Should().NotBeEmpty();
            minskStorage.Items.First().TotalAmount.Should().Be(4);
            vitebskStorage.Items.First().TotalAmount.Should().Be(4);
            minskStorage.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "L",
                Amount = 1
            });

            minskStorage.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "XL",
                Amount = 3
            });

            vitebskStorage.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "L",
                Amount = 2
            });

            vitebskStorage.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "XL",
                Amount = 2
            });
        }

        [Fact]
        public async Task CreateItemTransactionsAsyncWithIncorrectAmountReturnBadRequest()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var minskStorageId = 1;
            var vitebskStorageId = 2;

            var createdItem = await client.CreateMerchItemAsHttpMessage(
                isClothing: true,
                Guid.CreateVersion7().ToAlphabetic()
            ).WithResponseType<ItemDto>();

            // Act
            await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = minskStorageId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 3
                    },
                    new()
                    {
                        SizeId = "M",
                        Count = 5
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Receipt
            });

            var response = await client.CreateItemTransaction(new AddItemTransactionInput
            {
                ItemId = createdItem.Id,
                WarehouseId = minskStorageId,
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "XL",
                        Count = 3
                    }
                },
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseRecipientId = vitebskStorageId,
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateKitTransactionsAsyncWithoutClothReturnOk()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var minskStorageId = 1;
            var vitebskStorageId = 2;

            var firstItem = await client.CreateMerchItem(isClothing: false);
            var secondItem = await client.CreateMerchItem(isClothing: false);
            var minAmountsList = await client.GetKitsStoragesMinAmounts();

            var kitInput = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

            //Act
            var createdKit = await client.CreateMerchKit(kitInput);

            // Act
            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 3,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Receipt,
                WarehouseId = minskStorageId,
            }).WithResponseType<int>();

            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 1,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseId = minskStorageId,
                WarehouseRecipientId = vitebskStorageId
            }).WithResponseType<int>();

            var transactionfirst = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = firstItem.Id,
                IdTo = firstItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var minskStoragefirst = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [minskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = firstItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            var transactionSecnod = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = secondItem.Id,
                IdTo = secondItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var minskStorageSecond = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [minskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = secondItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();


            var vitebskStoragefirst = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [vitebskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = firstItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

           var vitebskStorageSecond = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
           new StorageQueryOptionsDto
           {
               StorageIds = [vitebskStorageId],
               Filters =
               [
                   new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = secondItem.Id.ToString(),
                    }
               ]
           }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            // Assert
            transactionfirst.Items.First().Transactions.Should().NotBeEmpty();
            minskStoragefirst.Items.First().TotalAmount.Should().Be((3 - 1) * kitInput.Compounds.First().AmountInKit);

            transactionSecnod.Items.First().Transactions.Should().NotBeEmpty();
            minskStorageSecond.Items.First().TotalAmount.Should().Be((3 - 1) * kitInput.Compounds.Skip(1).First().AmountInKit);

            vitebskStoragefirst.Items.First().TotalAmount.Should().Be(1 * kitInput.Compounds.First().AmountInKit);
            vitebskStorageSecond.Items.First().TotalAmount.Should().Be(1 * kitInput.Compounds.Skip(1).First().AmountInKit);
        }

        [Fact]
        public async Task CreateKitTransactionsAsyncWithClothReturnOk()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var minskStorageId = 1;
            var vitebskStorageId = 2;
            var count = 3;

            var firstItem = await client.CreateMerchItem(isClothing: false);
            var secondItem = await client.CreateMerchItem(isClothing: true);
            var minAmountsList = await client.GetKitsStoragesMinAmounts();

            var kitInput = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

            //Act
            var createdKit = await client.CreateMerchKit(kitInput);

            // Act
            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = count,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 1
                    },
                    new()
                    {
                        SizeId = "XL",
                        Count = 1
                    },
                    new()
                    {
                        SizeId = "M",
                        Count = 1
                    }
                },
                TransactionType = Core.Merch.MerchTransactionType.Receipt,
                WarehouseId = minskStorageId,
            }).WithResponseType<int>();

            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 1,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 1
                    }
                },
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseId = minskStorageId,
                WarehouseRecipientId = vitebskStorageId
            }).WithResponseType<int>();

            var transactionfirst = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = firstItem.Id,
                IdTo = firstItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var storagefirst = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [minskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = firstItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            var transactionSecnod = await client.GetTransactions(Idfilters: new TransactionFilters
            {
                IdFrom = secondItem.Id,
                IdTo = secondItem.Id
            })
            .WithResponseType<PageableListOfItems<TransationOdpDto>>();

            var storageSecond = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [minskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = secondItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();


            var vitebskstoragefirst = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
                new StorageQueryOptionsDto
                {
                    StorageIds = [vitebskStorageId],
                    Filters =
                    [
                        new QueryFilterDto
                         {
                             FilterColumn = "id",
                             FilterValue = firstItem.Id.ToString(),
                         }
                    ]
                }).WithResponseType<PageableListOfItems<StorageItemDto>>();
            var vitebskstorageSecond = await client.GetMerchStorageAsHttpMessage(top: 25, skip: 0,
            new StorageQueryOptionsDto
            {
                StorageIds = [vitebskStorageId],
                Filters =
                [
                    new QueryFilterDto
                    {
                        FilterColumn = "id",
                        FilterValue = secondItem.Id.ToString(),
                    }
                ]
            }).WithResponseType<PageableListOfItems<StorageItemDto>>();

            // Assert
            transactionfirst.Items.First().Transactions.Should().NotBeEmpty();
            storagefirst.Items.First().TotalAmount.Should().Be((count - 1) * kitInput.Compounds.First().AmountInKit);

            transactionSecnod.Items.First().Transactions.Should().NotBeEmpty();
            storageSecond.Items.First().TotalAmount.Should().Be((count - 1) * kitInput.Compounds.Skip(1).First().AmountInKit);

            vitebskstoragefirst.Items.First().TotalAmount.Should().Be(1 * kitInput.Compounds.First().AmountInKit);
            vitebskstorageSecond.Items.First().TotalAmount.Should().Be(1 * kitInput.Compounds.Skip(1).First().AmountInKit);

            storageSecond.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "L",
                Amount = 0
            });

            storageSecond.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "XL",
                Amount = 1
            });

            storageSecond.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "M",
                Amount = 1
            });

            vitebskstorageSecond.Items.First().SizeAmounts.Should().ContainEquivalentOf(new
            {
                SizeId = "L",
                Amount = 1
            });
        }

        [Fact]
        public async Task CreateKitTransactionsAsyncWithIncorrectAmountReturnBadRequest()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var storageId = 1;
            var vitebskStorageId = 2;

            var firstItem = await client.CreateMerchItem(isClothing: false);
            var secondItem = await client.CreateMerchItem(isClothing: false);
            var minAmountsList = await client.GetKitsStoragesMinAmounts();

            var kitInput = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

            //Act
            var createdKit = await client.CreateMerchKit(kitInput);

            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 3,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Receipt,
                WarehouseId = storageId,
            }).WithResponseType<int>();

            var transfertransactionsCount = await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 4,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseId = storageId,
                WarehouseRecipientId = vitebskStorageId
            });

            // Assert
            transfertransactionsCount.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateKitTransactionsAsyncWithIncorrectClothAmountReturnBadRequest()
        {
            // Arrange
            using var client = _factory.CreateClient();
            await client.AuthAsCto();

            var storageId = 1;
            var vitebskStorageId = 2;
            var count = 3;

            var firstItem = await client.CreateMerchItem(isClothing: false);
            var secondItem = await client.CreateMerchItem(isClothing: true);
            var minAmountsList = await client.GetKitsStoragesMinAmounts();

            var kitInput = new CreateKitDto(
            Name: Guid.CreateVersion7().ToAlphabetic(),
            PhotoFileId: "FileId",
            Description: "Description",
            Compounds: new List<AddKitCompoundDto>
            {
                new(firstItem.Id, 2),
                new(secondItem.Id, 1)
            },
            MinAmounts: minAmountsList);

            //Act
            var createdKit = await client.CreateMerchKit(kitInput);

            // Act
            await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = count,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 1
                    },
                    new()
                    {
                        SizeId = "XL",
                        Count = 1
                    },
                    new()
                    {
                        SizeId = "M",
                        Count = 1
                    }
                },
                TransactionType = Core.Merch.MerchTransactionType.Receipt,
                WarehouseId = storageId,
            }).WithResponseType<int>();

            var transferTransactionsCount = await client.CreateKitTransaction(new AddKitTransactionInput
            {
                Amount = 1,
                KitId = createdKit.Id,
                Notes = Guid.CreateVersion7().ToAlphabetic(),
                SizeCounts = new List<Portal.Merch.Requests.Core.Dto.SizeCountDto>
                {
                    new()
                    {
                        SizeId = "L",
                        Count = 10
                    }
                },
                TransactionType = Core.Merch.MerchTransactionType.Transfer,
                WarehouseId = storageId,
                WarehouseRecipientId = vitebskStorageId
            });

            transferTransactionsCount.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}
