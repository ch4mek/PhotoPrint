using MockQueryable.Moq;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core.CoreDto;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;
using Modsen.Portal.StimulsoftReport.Contract;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Discounts;

public class ExportDiscountsToPdfRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContext = new();
    private readonly Mock<IDiscountLoader> _discountLoader = new();
    private readonly Mock<IReportExporter> _reportExporter = new();
    private readonly ExportDiscountsToPdfRequest _handler;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    public ExportDiscountsToPdfRequestTests()
    {
        _handler = new ExportDiscountsToPdfRequest(
            _dbContext.Object,
            _reportExporter.Object,
            _discountLoader.Object);
    }

    [Fact]
    public async Task HandleWhenActiveDiscountsExistShouldExportSuccessfully()
    {
        // Arrange
        var discountsFromDb = new List<DiscountEntity>
        {
            new() { Id = 1, IsActive = true, Name = null, Text = null },
            new() { Id = 2, IsActive = true, Name = null, Text = null }
        };

        MockDbContext(discountsFromDb);

        var loadedDiscounts = new List<DiscountCoreDto>
        {
            new()
            {
                Id = 1,
                Name = "Test Discount 1",
                DiscountCategoryName = "Category A",
                DiscountPercent = 10,
                Text = "Text",
                DiscountTags = new List<DiscountTagCoreDto> { new() { Name = "tag1" } },
                DiscountCities = new List<DiscountCityCoreDto> { new() { CityId = 1, Name = "City1" } },
                DiscountReferences = new List<DiscountReferenceCoreDto>()
            },
            new()
            {
                Id = 2,
                Name = "Test Discount 2",
                DiscountCategoryName = "Category B",
                DiscountPercent = 20,
                Text = "Text2",
                DiscountTags = new List<DiscountTagCoreDto>(),
                DiscountCities = new List<DiscountCityCoreDto>(),
                DiscountReferences = new List<DiscountReferenceCoreDto>()
            }
        };

        _discountLoader.Setup(x => x.Load(new[] { 1, 2 }, _cancellationToken))
            .ReturnsAsync(loadedDiscounts);

        var expectedBytes = new byte[] { 1, 2, 3 };
        _reportExporter.Setup(x => x.ExportPdfAsync(It.IsAny<ReportRequest>(), _cancellationToken))
            .ReturnsAsync(expectedBytes);

        var query = new ExportDiscountsToPdfRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEquivalentTo(expectedBytes);
        _dbContext.Verify(x => x.Discounts, Times.Once);
        _discountLoader.Verify(x => x.Load(new[] { 1, 2 }, _cancellationToken), Times.Once);
        _reportExporter.Verify(x => x.ExportPdfAsync(It.IsAny<ReportRequest>(), _cancellationToken), Times.Once);
    }

    [Fact]
    public async Task HandleWhenNoActiveDiscountsShouldReturnEmptyPdf()
    {
        // Arrange
        var discountsFromDb = new List<DiscountEntity>(); // no active discounts
        MockDbContext(discountsFromDb);

        _discountLoader.Setup(x => x.Load(Array.Empty<int>(), _cancellationToken))
            .ReturnsAsync([]);

        byte[] expectedBytes = [];
        _reportExporter.Setup(x => x.ExportPdfAsync(It.IsAny<ReportRequest>(), _cancellationToken))
            .ReturnsAsync(expectedBytes);

        var query = new ExportDiscountsToPdfRequest.Query();

        // Act
        var result = await _handler.Handle(query, _cancellationToken);

        // Assert
        result.Should().BeEmpty();
        _discountLoader.Verify(x => x.Load(Array.Empty<int>(), _cancellationToken), Times.Once);
        _reportExporter.Verify(x => x.ExportPdfAsync(It.IsAny<ReportRequest>(), _cancellationToken), Times.Once);
    }

    private void MockDbContext(IEnumerable<DiscountEntity> discounts)
    {
        _dbContext.Setup(x => x.Discounts)
            .Returns(discounts.ToArray().BuildMockDbSet().Object);
    }
}
