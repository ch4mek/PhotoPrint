using Modsen.Portal.Core.Services.Cqrs;

namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

internal interface IReportQuerySelector
{
    IQuery<byte[]> GetQueryByParameters(GetTrackedTimeToExportQueryModel model);
}
