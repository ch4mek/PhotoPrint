using System.IO.Compression;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.TimeTrack;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Contracts;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql;
using Modsen.Portal.TimeTrack.DataAccess.Npgsql.Extensions;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

internal sealed class GetPerUserTrackedTimeArchiveRequest : IQueryHandler<GetPerUserTrackedTimeArchiveRequest.Query, byte[]>
{
    private readonly ISender _sender;
    private readonly ITimeTrackDbContext _timeTrackDbContext;
    private readonly IReportNameComposer _reportNameComposer;
    private readonly IReportQuerySelector _reportQuerySelector;

    public GetPerUserTrackedTimeArchiveRequest(
        ISender sender,
        ITimeTrackDbContext timeTrackDbContext,
        IReportNameComposer reportNameComposer,
        IReportQuerySelector reportQuerySelector)
    {
        _sender = sender;
        _timeTrackDbContext = timeTrackDbContext;
        _reportNameComposer = reportNameComposer;
        _reportQuerySelector = reportQuerySelector;
    }

    public async Task<byte[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var timesheetsFilter = new TimesheetFilter
        {
            UserId = request.UserId,
            DepartmentId = request.DepartmentId,
            ResourceGroupId = request.ResourceGroupId,
            ClientId = request.ClientId,
            ProjectId = request.ProjectId,
            TrackedTimeType = request.TrackedTimeType
        };

        var timesheets = _timeTrackDbContext.Timesheets
            .AsNoTracking()
            .FilterByPeriod(request.Year, request.MonthOfYear, request.WeekOfYear)
            .ApplyTimesheetFilter(timesheetsFilter)
            .OrderBy(timesheet => timesheet.Id);

        var userIds = await timesheets.Select(t => t.UserId).Distinct().ToListAsync(cancellationToken);

        using var archiveStream = new MemoryStream();

        if (userIds.Count == 0)
        {
            return [];
        }

        using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
        {
            foreach (var userId in userIds)
            {
                var reportName = await _reportNameComposer.ComposeForSingleFileAsync(
                    userId,
                    request.ProjectId,
                    request.ClientId,
                    request.Year,
                    request.MonthOfYear,
                    request.Format,
                    cancellationToken);

                var queryModel = new GetTrackedTimeToExportQueryModel(
                    request.Year,
                    request.MonthOfYear,
                    request.WeekOfYear,
                    request.DepartmentId,
                    request.ClientId,
                    userId,
                    request.ProjectId,
                    request.ResourceGroupId,
                    reportName,
                    request.TrackedTimeType,
                    request.Format,
                    false);

                var reportQuery = _reportQuerySelector.GetQueryByParameters(queryModel);

                var reportContent = await _sender.Send(reportQuery, cancellationToken);

                var archiveReportEntry = archive.CreateEntry(reportName);

                await using var reportStream = archiveReportEntry.Open();
                using var reportFileStream = new MemoryStream(reportContent);

                await reportFileStream.CopyToAsync(reportStream, cancellationToken);
            }
        }

        return archiveStream.ToArray();
    }

    internal sealed record Query(
        int? Year,
        int[]? MonthOfYear,
        int? WeekOfYear,
        string? DepartmentId,
        int? ClientId,
        int? UserId,
        int? ResourceGroupId,
        int? ProjectId,
        TrackedTimeType TrackedTimeType,
        DocumentType Format)
        : IQuery<byte[]>, ITimeTrackRequest;
}
