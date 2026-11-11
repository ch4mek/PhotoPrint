using MockQueryable.Moq;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql.Entities;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;
using Modsen.Portal.StimulsoftReport.Contract;
using Moq;

namespace Modsen.Portal.KnowledgeBase.UnitTests.Articles.Requests;

public class ExportArticlesToPdfRequestTests
{
    private readonly Mock<IKnowledgeBaseDbContext> _dbContextMock = new();
    private readonly Mock<IReportExporter> _reportExporterMock = new();
    private readonly ExportArticlesToPdfRequest _handler;
    private readonly CancellationToken _ct = CancellationToken.None;

    public ExportArticlesToPdfRequestTests()
    {
        _handler = new ExportArticlesToPdfRequest(_dbContextMock.Object, _reportExporterMock.Object);
    }

    [Fact]
    public async Task HandleWhenActiveArticlesExistShouldExportPdfSuccessfully()
    {
        // Arrange
        var activeArticle = CreateArticleEntity(
            id: 1,
            groupName: "Group A",
            name: "Article 1",
            text: "Text content",
            keywords: "cloud;data",
            isActive: true,
            tagNames: ["tag1", "tag2"],
            internalFileIds: ["file123"],
            externalUrls: ["http://link.com"]);

        var inactiveArticle = CreateArticleEntity(
            id: 2,
            groupName: "Group B",
            name: "Article 2",
            text: "Some text",
            keywords: "other",
            isActive: false,
            tagNames: [],
            internalFileIds: [],
            externalUrls: []);

        var articles = new List<ArticleEntity> { activeArticle, inactiveArticle };
        _dbContextMock.Setup(x => x.Articles).Returns(articles.BuildMockDbSet().Object);

        var expectedPdfBytes = new byte[] { 1, 2, 3 };
        _reportExporterMock
            .Setup(x => x.ExportPdfAsync(It.IsAny<ReportRequest>(), _ct))
            .ReturnsAsync(expectedPdfBytes);

        var query = new ExportArticlesToPdfRequest.Query();

        // Act
        var result = await _handler.Handle(query, _ct);

        // Assert
        _reportExporterMock.Verify(x =>
                x.ExportPdfAsync(It.Is<ReportRequest>(r =>
                        r.ReportName == PortalSettingNames.DefaultArticleReportName &&
                        r.TemplateType == TemplateType.PerArticle &&
                        r.RootDataObject != null),
                    _ct),
            Times.Once);

        result.Should().BeEquivalentTo(expectedPdfBytes);
    }

    private ArticleEntity CreateArticleEntity(
        int id,
        string groupName,
        string name,
        string text,
        string keywords,
        bool isActive,
        IEnumerable<string> tagNames,
        IEnumerable<string> internalFileIds,
        IEnumerable<string> externalUrls)
    {
        var group = new GroupEntity { Id = 100, Name = groupName };

        var tags = tagNames.Select(t => new ArticleTagEntity { Tag = new TagEntity { Name = t } }).ToList();

        var references = new List<ArticleReferenceEntity>();
        references.AddRange(internalFileIds.Select(f => new ArticleReferenceEntity { InternalFileId = f }));
        references.AddRange(externalUrls.Select(u => new ArticleReferenceEntity { ExternalUrl = u }));

        return new ArticleEntity
        {
            Id = id,
            Group = group,
            GroupId = group.Id,
            Name = name,
            Text = text,
            Keywords = keywords,
            IsActive = isActive,
            ArticleTags = tags,
            ArticleReferences = references
        };
    }
}
