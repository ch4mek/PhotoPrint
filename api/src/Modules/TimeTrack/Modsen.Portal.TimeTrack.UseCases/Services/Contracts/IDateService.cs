using Modsen.Portal.TimeTrack.UseCases.Dtos.Reports;
using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

internal interface IDateService
{
    DateSpanDto GetDateSpan(IPeriodRequest request);
}
