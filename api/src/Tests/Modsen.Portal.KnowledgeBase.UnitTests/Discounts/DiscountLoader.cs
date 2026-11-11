using System.Reflection;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Moq;
using Xunit.Sdk;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class DiscountLoaderTests
{
    private sealed class InterfacesInline : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            var dbMock = new Mock<IKnowledgeBaseDbContext>();

            Shared.MockDbContext(dbMock);

            return [[new DiscountLoader(dbMock.Object)]];
        }
    }

    [Theory]
    [InterfacesInline]
    internal async Task LoadDiscounts(IDiscountLoader loader)
    {
        // Arrange
        var expect = new List<DiscountCoreDto>()
        {
            new()
            {
                Id = 1,
                DiscountCategoryId = 1,
                DiscountCategoryName = "First",
                DiscountTags =
                [
                    new() { DiscountId = 1, Id = 1, Name = "Tag-1", TagId = 1 },
                    new() { DiscountId = 1, Id = 4, Name = "Tag-2", TagId = 2 }
                ],
                DiscountCities = [],
                DiscountReferences = [],
                Name = "Name",
                Text = "Text"
            },
            new()
            {
                Id = 2,
                DiscountCategoryId = 2,
                DiscountCategoryName = "Second",
                DiscountCities =
                [
                    new() { CityId = 1, Name = "City" }
                ],
                DiscountTags =
                [
                    new() { DiscountId = 1, Id = 1, Name = "Tag-1", TagId = 1 }
                ],
                DiscountReferences = [],
                Name = "Name",
                Text = "Text"
            },
            new()
            {
                Id = 3,
                DiscountCategoryId = 2,
                DiscountCategoryName = "Second",
                DiscountTags = [],
                DiscountCities = [],
                DiscountReferences = [],
                Name = "Name",
                OrderWeight = 0,
                Text = "Text"
            },
            new()
            {
                Id = 4,
                DiscountCategoryId = 3,
                DiscountCategoryName = "Third",
                DiscountTags = [],
                DiscountCities = [],
                DiscountReferences = [],
                Name = "Name",
                OrderWeight = 0,
                Text = "Text"
            }
        };

        // Act
        var discounts = await loader.Load([1, 2, 3, 4], It.IsAny<CancellationToken>());

        // Assert
        discounts.Should().BeEquivalentTo<DiscountCoreDto>(expect);
    }
}
