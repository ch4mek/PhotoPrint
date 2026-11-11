using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamWork.UseCases.Models;
using Modsen.Portal.TimeTrack.Contract;

namespace Modsen.Portal.TeamWork.UseCases;

internal sealed class GetUsersWithInactiveTimeTrackingUseCase    : IQueryHandler<GetUsersWithInactiveTimeTrackingUseCase.Query, GetUsersWithInactiveTimeTrackingUseCase.Response>
{
    private const int s_daysRangeBeforeCurrentDate = 4;

    private readonly ITimeTrackContract _timeTrackContract;

    public GetUsersWithInactiveTimeTrackingUseCase(ITimeTrackContract timeTrackContract)
    {
        _timeTrackContract = timeTrackContract;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var users = await _timeTrackContract.GetUsersWithInactiveTimeTracking(s_daysRangeBeforeCurrentDate,
            cancellationToken);

        return new Response(users.ConvertAll(user => new UserEmail(user.Email, user.Name)));
    }

    internal sealed record Query : IQuery<Response>;
    internal record Response(IReadOnlyCollection<UserEmail> Users);
}
