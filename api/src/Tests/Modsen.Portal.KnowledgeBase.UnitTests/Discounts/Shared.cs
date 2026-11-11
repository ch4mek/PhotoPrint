using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

internal static class Shared
{
    private static DiscountEntity Fit(
        int id,
        int categoryId,
        DiscountCategoryEntity category,
        string? keywords = null,
        List<DiscountTagEntity>? tags = null,
        List<DiscountCityEntity>? cities = null
    )
    {
        return new DiscountEntity
        {
            Id = id,
            DiscountCategoryId = categoryId,
            DiscountCategory = category,
            Name = "Name",
            Text = "Text",
            DiscountCities = cities ?? [],
            DiscountTags = tags ?? [],
        };
    }

    internal static void MockDbContext(Mock<IKnowledgeBaseDbContext> dbMock)
    {
        var tags = new List<TagEntity> { new() { Id = 1, Name = "Tag-1", }, new() { Id = 2, Name = "Tag-2", } };

        var discountTags = new List<DiscountTagEntity>
        {
            new() { Id = 1, TagId = 1, DiscountId = 1, Tag = tags[0] },
            new() { Id = 2, TagId = 1, DiscountId = 2, Tag = tags[0] },
            new() { Id = 3, TagId = 1, DiscountId = 3, Tag = tags[0] },
            new() { Id = 4, TagId = 2, DiscountId = 1, Tag = tags[1] },
        };

        var cities = new List<CityEntity> { new() { Id = 1, Name = "City", } };

        var discountCities = new List<DiscountCityEntity>
        {
            new() { Id = 1, CityId = 1, DiscountId = 1, City = cities[0] },
        };

        var discountCategories = new List<DiscountCategoryEntity>
        {
            new() { Id = 1, Name = "First", },
            new() { Id = 2, Name = "Second", },
            new() { Id = 3, Name = "Third", },
        };

        var discounts = new List<DiscountEntity>
        {
            Fit(id: 1, categoryId: 1, discountCategories[0],
                tags: [discountTags[0], discountTags[3]]),
            Fit(id: 2, categoryId: 2, discountCategories[1],
                tags: [discountTags[0]],
                cities: [discountCities[0]]),
            Fit(id: 3, categoryId: 2, discountCategories[1],
                keywords: "Sport Health"),
            Fit(id: 4, categoryId: 3, discountCategories[2]),
        };

        dbMock.Setup(db => db.Tags)
            .Returns(tags.BuildMockDbSet().Object);
        dbMock.Setup(db => db.DiscountTags)
            .Returns(discountTags.BuildMockDbSet().Object);

        dbMock.Setup(db => db.Cities)
            .Returns(cities.BuildMockDbSet().Object);
        dbMock.Setup(db => db.DiscountCities)
            .Returns(discountCities.BuildMockDbSet().Object);

        dbMock.Setup(db => db.DiscountsReferences)
            .Returns(Array.Empty<DiscountReferenceEntity>().BuildMockDbSet().Object);
        dbMock.Setup(db => db.DiscountCategories)
            .Returns(discountCategories.BuildMockDbSet().Object);
        dbMock.Setup(db => db.Discounts)
            .Returns(discounts.BuildMockDbSet().Object);
    }
}
