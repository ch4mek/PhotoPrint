using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.SettingNames;
using Modsen.Portal.Core.Utils.Extensions;
using Modsen.Portal.KnowledgeBase.DataAccess.Npgsql;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Core;
using Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests.RequestDto;
using Modsen.Portal.StimulsoftReport.Contract;

namespace Modsen.Portal.KnowledgeBase.Requests.Discounts.Requests;

internal sealed class ExportDiscountsToPdfRequest : IQueryHandler<ExportDiscountsToPdfRequest.Query, byte[]>
{
    private readonly IKnowledgeBaseDbContext _knowledgeBaseDbContext;
    private readonly IDiscountLoader _discountLoader;
    private readonly IReportExporter _reportExporter;

    public ExportDiscountsToPdfRequest(
        IKnowledgeBaseDbContext knowledgeBaseDbContext,
        IReportExporter reportExporter,
        IDiscountLoader discountLoader)
    {
        _knowledgeBaseDbContext = knowledgeBaseDbContext;
        _discountLoader = discountLoader;
        _reportExporter = reportExporter;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var discountIds = await _knowledgeBaseDbContext.Discounts
            .Where(x => x.IsActive)
            .Select(x => x.Id)
            .ToArrayAsync(cancellationToken);

        var discounts = await _discountLoader.Load(discountIds, cancellationToken);

        var discountsToExport = discounts.Select(x =>
            new DiscountExportDto
            {
                DiscountCategoryName = x.DiscountCategoryName,
                Name = x.Name,
                DiscountPercent = x.DiscountPercent,
                Text = x.Text,
                DiscountTagNames = string.Join(", ", x.DiscountTags.Select(t => t.Name).ToList()),
                DiscountCityNames = string.Join(", ", x.DiscountCities.Select(c => c.Name).ToList()),
                ExternalUrls = string.Join(", ", x.DiscountReferences
                .Where(f => !f.SocialMediaUrl.IsNullOrEmpty())
                .Select(f => f.SocialMediaUrl).ToList()),
            });


        var reportRequest = new ReportRequest
        {
            ReportName = PortalSettingNames.DefaultDiscountReportName,
            TemplateType = TemplateType.PerDiscount,
            ReportsAuthorName = PortalSettingNames.ReportsAuthorName,
            RootDataObject = discountsToExport
        };

        var discountsPdfFileInBytes = await _reportExporter.ExportPdfAsync(reportRequest, cancellationToken);

        return discountsPdfFileInBytes;
    }

    internal sealed record Query : IQuery<byte[]>;
}
