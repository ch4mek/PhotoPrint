using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Articles.Requests.RequestDto;
using Modsen.Portal.StimulsoftReport.Contract;

namespace Modsen.Portal.KnowledgeBase.Requests.Articles.Requests;

internal sealed class ExportArticlesToPdfRequest : IQueryHandler<ExportArticlesToPdfRequest.Query, byte[]>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IReportExporter _reportExporter;

    public ExportArticlesToPdfRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IReportExporter reportExporter)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _reportExporter = reportExporter;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var articles = await _knowledgeBaseDbContext.Articles
            .Where(x => x.IsActive)
            .Include(x => x.Group)
            .Include(x => x.ArticleReferences)
            .Include(x => x.ArticleTags)
            .ThenInclude(t => t.Tag)
            .AsSplitQuery()
            .AsNoTrackingWithIdentityResolution()
            .ToListAsync(cancellationToken);

        var articlesToExport = articles.Select(x => new ArticleExportDto
        {
            ArticleGroupName = x.Group!.Name,
            Name = x.Name,
            Text = x.Text,
            Keywords = x.Keywords,
            ArticleTagNames = string.Join(", ", x.ArticleTags.Select(t => t.Tag!.Name).ToList()),
            InternalFileIds = string.Join(", ", x.ArticleReferences
                .Where(f => !f.InternalFileId.IsNullOrEmpty())
                .Select(f => f.InternalFileId).ToList()),
            ExternalUrls = string.Join(", ", x.ArticleReferences
                .Where(f => !f.ExternalUrl.IsNullOrEmpty())
                .Select(f => f.ExternalUrl).ToList())
        });

        var reportRequest = new ReportRequest
        {
            ReportName = PortalSettingNames.DefaultArticleReportName,
            TemplateType = TemplateType.PerArticle,
            ReportsAuthorName = PortalSettingNames.ReportsAuthorName,
            RootDataObject = articlesToExport
        };

        var articlesPdfFileInBytes = await _reportExporter.ExportPdfAsync(reportRequest, cancellationToken);

        return articlesPdfFileInBytes;
    }

    internal sealed record Query : IQuery<byte[]>;
}
