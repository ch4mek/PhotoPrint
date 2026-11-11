using Modsen.Portal.Core;
using Modsen.Portal.Core.TimeTrack;

namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

public record GetTrackedTimeToExportQueryModel(
    int? Year,
    int[]? MonthOfYear,
    int? WeekOfYear,
    string? DepartmentId,
    int? ClientId,
    int? UserId,
    int? ProjectId,
    int? ResourceGroupId,
    string ReportName,
    TrackedTimeType TrackedTimeType,
    DocumentType Format,
    bool IsPerUserExport);
