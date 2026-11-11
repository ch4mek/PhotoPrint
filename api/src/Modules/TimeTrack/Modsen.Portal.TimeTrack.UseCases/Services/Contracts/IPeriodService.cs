using Modsen.Portal.TimeTrack.UseCases.Reports.Requests;

namespace Modsen.Portal.TimeTrack.UseCases.Services.Contracts;

internal interface IPeriodService
{
    bool IsFilterForEntireYear(IPeriodRequest period);

    bool IsFilterForSelectedQuarter(IPeriodRequest period);

    bool IsFilterForOneSelectedMonth(IPeriodRequest period);
}
