using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcel;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByExcelAccount;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests.GetTrackedTimeByStimulsoft;
using Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

namespace Modsen.Portal.TimeTrack.UseCases.Services;

internal class ReportQuerySelector : IReportQuerySelector
{
    public IQuery<byte[]> GetQueryByParameters(GetTrackedTimeToExportQueryModel model)
    {
        if (model is { IsPerUserExport: true, UserId: null })
        {
            return new GetPerUserTrackedTimeArchiveRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                model.ClientId,
                model.UserId,
                model.ResourceGroupId,
                model.ProjectId,
                model.TrackedTimeType,
                model.Format);
        }

        var filterTuple = (model.UserId, model.ProjectId, model.ClientId, model.Format);

        IQuery<byte[]> query = filterTuple switch
        {
            { Format: DocumentType.Excel, ProjectId: null } => new GetTrackedTimeByClientInExcelRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                model.ClientId,
                model.UserId,
                model.ResourceGroupId,
                model.TrackedTimeType),
            { Format: DocumentType.Excel, ProjectId: not null } => new GetTrackedTimeByProjectInExcelRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                model.ClientId,
                model.UserId,
                model.ResourceGroupId,
                filterTuple.ProjectId.Value,
                model.TrackedTimeType),
            { Format: DocumentType.ExcelAccount, ProjectId: null } =>
                new GetTrackedTimeByClientInExcelAccountRequest.Query(
                    model.Year,
                    model.MonthOfYear,
                    model.WeekOfYear,
                    model.DepartmentId,
                    model.ClientId,
                    model.UserId,
                    model.ResourceGroupId,
                    model.TrackedTimeType),
            { Format: DocumentType.ExcelAccount, ProjectId: not null } =>
                new GetTrackedTimeByProjectInExcelAccountRequest.Query(
                    model.Year,
                    model.MonthOfYear,
                    model.WeekOfYear,
                    model.DepartmentId,
                    model.ClientId,
                    model.UserId,
                    model.ResourceGroupId,
                    filterTuple.ProjectId.Value,
                    model.TrackedTimeType),
            { UserId: not null } => new GetTrackedTimeByUserIdToExportRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                model.ProjectId,
                model.ClientId,
                filterTuple.UserId.Value,
                model.ResourceGroupId,
                model.Format,
                model.ReportName,
                model.TrackedTimeType),
            { ProjectId: not null } => new GetTrackedTimeByProjectIdToExportRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                filterTuple.ProjectId.Value,
                model.UserId,
                model.ResourceGroupId,
                model.Format,
                model.ReportName,
                model.TrackedTimeType),
            { ClientId: not null } => new GetTrackedTimeByClientIdToExportRequest.Query(
                model.Year,
                model.MonthOfYear,
                model.WeekOfYear,
                model.DepartmentId,
                filterTuple.ClientId.Value,
                model.UserId,
                model.ResourceGroupId,
                model.Format,
                model.ReportName,
                model.TrackedTimeType),
            _ => new GetTotalTrackedTimeToExportRequest.Query(
                    model.Year,
                    model.MonthOfYear,
                    model.WeekOfYear,
                    model.DepartmentId,
                    model.UserId,
                    model.ResourceGroupId,
                    model.Format,
                    model.ReportName,
                    model.TrackedTimeType)
        };

        return query;
    }
}
