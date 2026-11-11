using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.Dtos;

namespace Modsen.Portal.TimeTrack.UseCases.Queries;

internal sealed class GetUsersWithInactiveTimeTrackingUseCase : IQueryHandler<GetUsersWithInactiveTimeTrackingUseCase.Query, GetUsersWithInactiveTimeTrackingUseCase.Response>
{
    private readonly IGetUsersWithInactiveTimeTrackingDbQuery _dbQuery;
    private readonly IClockService _clockService;

    public GetUsersWithInactiveTimeTrackingUseCase(IGetUsersWithInactiveTimeTrackingDbQuery dbQuery,
        IClockService clockService)
    {
        _dbQuery = dbQuery;
        _clockService = clockService;
    }

    public async Task<Response> Handle(Query request, CancellationToken cancellationToken)
    {
        var currentDateTime = _clockService.UtcNow;
        var startDate = currentDateTime.AddDays(-request.DaysRangeBeforeCurrentDate);

        var dbRequest = new GetUsersWithInactiveTimeTrackingDbRequest(
            DateOnly.FromDateTime(startDate), DateOnly.FromDateTime(currentDateTime));

        var response = await _dbQuery.ExecuteAsync(dbRequest, cancellationToken);

        return new Response(response.ConvertAll(user => new UserEmailDto(user.Email, user.Name)));
    }

    internal sealed record Query(int DaysRangeBeforeCurrentDate) : IQuery<Response>;

    internal record Response(List<UserEmailDto> Users);
}
