using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.BusinessRules;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class CreateDiscountRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();
    private readonly Mock<IDiscountLoader> _discountLoaderMock = new();
    private readonly Mock<IHasDiscountCategoryAllDiscountTagsBusinessRule> _hasDiscountCategoryAllDiscountTagsBusinessRuleMock = new();
    private readonly Mock<IImageFileIdValidator> _imageFileIdValidatorMock = new();

    private readonly CreateDiscountRequest _handler;

    public CreateDiscountRequestTests()
    {
        _handler = new CreateDiscountRequest(_dbContextMock.Object,
            _discountLoaderMock.Object,
            _hasDiscountCategoryAllDiscountTagsBusinessRuleMock.Object,
            _imageFileIdValidatorMock.Object
            );
    }

    // TODO: Is there any way to avoid this?
    private static InternalEntityEntry FakeInternalEntityEntry(DiscountEntity entity)
    {
        return new InternalEntityEntry(
            new Mock<IStateManager>().Object,
            new RuntimeEntityType("T", typeof(int), false, null, null, null, ChangeTrackingStrategy.Snapshot, null,
                false, null, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0),
            entity);
    }

    private void MockTestContext()
    {
        var discountCategories = new List<DiscountCategoryEntity>
        {
            new() { Id = 1, Name = "First", }, new() { Id = 2, Name = "Second", },
        };

        var cities = new List<CityEntity>
        {
            new() {Id = 1, Name = "Minsk"}
        };

        var discount = new DiscountEntity { Id = 1, Name = "TEST", Text = "123"};

        var entityEntry = new Mock<EntityEntry<DiscountEntity>>(FakeInternalEntityEntry(discount));

        entityEntry.Setup(e => e.Entity)
            .Returns(discount);

        _hasDiscountCategoryAllDiscountTagsBusinessRuleMock.Setup(e => e.IsCorrespondsToBusiness(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _dbContextMock.Setup(db => db.Discounts)
            .Returns(Array.Empty<DiscountEntity>().BuildMockDbSet().Object);

        _dbContextMock.Setup(db => db.Discounts.Add(It.IsAny<DiscountEntity>()))
            .Returns(entityEntry.Object);

        _dbContextMock.Setup(db => db.Cities)
            .Returns(cities.BuildMockDbSet().Object);

        _dbContextMock.Setup(db => db.DiscountCategories)
            .Returns(discountCategories.BuildMockDbSet().Object);

        _dbContextMock.Setup(db => db.DiscountCategories.FindAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((object[] category) =>
                new ValueTask<DiscountCategoryEntity?>(discountCategories.Find(it => it.Id == (int)category[0])));
    }

    [Fact]
    public async Task HandleWhenRequestShouldCreate()
    {
        // Arrange
        MockTestContext();

        // Act
        var discount =
            new CreateDiscountRequest.Command { Name = "Name", Text = "Text", DiscountPercent = 10, IsActive = true, DiscountCityIds = [1], DiscountTagIds = [] };

        _discountLoaderMock.Setup(x => x.Load(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            new DiscountCoreDto()
            {
                DiscountCategoryName = "First",
                Name = "Name",
                Text = "Text",
                DiscountPercent = 10,
                IsActive = true,
                DiscountCities = [],
                DiscountTags = []
            });

        var a = await _handler.Handle(
            discount with { DiscountCategoryId = 1 },
            It.IsAny<CancellationToken>());

        _discountLoaderMock.Setup(x => x.Load(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(
            new DiscountCoreDto()
            {
                DiscountCategoryName = "Second",
                Name = "Name",
                Text = "Text",
                DiscountPercent = 10,
                IsActive = true,
                DiscountCities = [],
                DiscountTags = []
            });

        var b = await _handler.Handle(
            discount with { DiscountCategoryId = 2 },
            It.IsAny<CancellationToken>());

        // Assert
        a.Text.Should().Be("Text");
        a.Name.Should().Be("Name");

        a.DiscountCategoryName.Should().Be("First");
        b.DiscountCategoryName.Should().Be("Second");
    }
}
