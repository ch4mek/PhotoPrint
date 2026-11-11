using Modsen.Portal.TeamBoard.Requests.Reviews.DevelopmentPlans.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Controllers.Contracts.Output;

internal record UserPerformanceReviewOutput(int Id, string UserFullName, IReadOnlyCollection<PerformanceReviewDateDto>? Dates);
