using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Requests;

internal sealed class GetEmployeesPayloadTotals : IQueryHandler<GetEmployeesPayloadTotals.Query, PayloadTotalHoursCoreDto[]>
{
    private readonly IGetPayloadUsersQueryBuilder _getPayloadUsersQueryBuilder;
    private readonly IEmployeesPayloadComposer _employeesPayloadComposer;
    private readonly ITotalEmployeesPayloadCalculator _employeesPayloadCalculator;


    public GetEmployeesPayloadTotals(
        IGetPayloadUsersQueryBuilder getPayloadUsersQueryBuilder,
        IEmployeesPayloadComposer employeesPayloadComposer,
        ITotalEmployeesPayloadCalculator employeesPayloadCalculator)
    {
        _getPayloadUsersQueryBuilder = getPayloadUsersQueryBuilder;
        _employeesPayloadComposer = employeesPayloadComposer;
        _employeesPayloadCalculator = employeesPayloadCalculator;
    }

    public async Task<PayloadTotalHoursCoreDto[]> Handle(Query request, CancellationToken cancellationToken)
    {
        var userIds = await GetUserIds(request, cancellationToken);
        var payloadDto = await _employeesPayloadComposer.Compose(request.Year, userIds, userIds.Length, request.Filter, cancellationToken);
        var totals = await _employeesPayloadCalculator.Calculate(payloadDto, cancellationToken);

        return totals;
    }

    private Task<int[]> GetUserIds(Query request, CancellationToken token)
    {
        return _getPayloadUsersQueryBuilder
            .ForYear(request.Year)
            .WithFilter(request.Filter)
            .WithOrdering(request.Ordering)
            .BuildUserIds(token);
    }

    internal sealed class Query : IQuery<PayloadTotalHoursCoreDto[]>
    {
        public int Year { get; set; }
        public PayloadFilterCoreDto Filter { get; set; } = new();
        public PayloadOrderingCoreDto Ordering { get; set; } = new();
    }
}
