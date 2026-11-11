using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record DevelopmentPlanOutput(int Id,
    DateOnly StartDate,
    DateOnly? DeadlineDate,
    DateOnly? EndDate,
    string Summary,
    string? Notes,
    int UserId)
{
    public static DevelopmentPlanOutput Of(DevelopmentPlanDto source)
    {
        return new DevelopmentPlanOutput(source.Id, source.StartDate, source.DeadlineDate, source.EndDate,
            source.Summary, source.Notes, source.UserId);
    }
};
