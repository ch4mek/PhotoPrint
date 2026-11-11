using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.TeamBoard.Requests.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.Payload.Core;
using Modsen.Portal.TeamBoard.Requests.Payload.Core.CoreDto;

namespace Modsen.Portal.TeamBoard.Requests.Payload.Requests;

internal sealed class GetEmployeesPayload : IQueryHandler<GetEmployeesPayload.Query, PayloadCoreDto>
{
    private readonly IGetPayloadUsersQueryBuilder _getPayloadUsersQueryBuilder;
    private readonly IEmployeesPayloadComposer _employeesPayloadComposer;
    private readonly ICanTeamBoardUserBeRetrievedBusinessRule _canTeamBoardUserBeRetrievedBusinessRule;


    public GetEmployeesPayload(
        IGetPayloadUsersQueryBuilder getPayloadUsersQueryBuilder,
        IEmployeesPayloadComposer employeesPayloadComposer,
        ICanTeamBoardUserBeRetrievedBusinessRule canTeamBoardUserBeRetrievedBusinessRule)
    {
        _getPayloadUsersQueryBuilder = getPayloadUsersQueryBuilder;
        _employeesPayloadComposer = employeesPayloadComposer;
        _canTeamBoardUserBeRetrievedBusinessRule = canTeamBoardUserBeRetrievedBusinessRule;
    }

    public async Task<PayloadCoreDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var (userIds, totalUsers) = await GetUserIds(request, cancellationToken);
        var payloadDto = await _employeesPayloadComposer.Compose(request.Year, userIds, totalUsers, request.Filter, cancellationToken);

        await AddUserInfoVisibility(userIds, payloadDto.Users, cancellationToken);

        return payloadDto;
    }

    private async Task<(int[], int)> GetUserIds(Query request, CancellationToken token)
    {
        var userIds = await _getPayloadUsersQueryBuilder
            .ForYear(request.Year)
            .WithFilter(request.Filter)
            .WithOrdering(request.Ordering)
            .WithCareerStartDate()
            .BuildUserIds(token);

        return (userIds
            .Skip(request.PageNumber * request.PageSize)
            .Take(request.PageSize)
            .ToArray(), userIds.Length);
    }

    private async Task AddUserInfoVisibility(int[] userIds, IReadOnlyCollection<PayloadUserCoreDto> users, CancellationToken token)
    {
        var accessOptions = AccessOptions.Manager;
        var visibility = await _canTeamBoardUserBeRetrievedBusinessRule.IsCorrespondsToBusinessBatch(userIds, accessOptions, token);

        foreach (var user in users)
        {
            if (visibility.TryGetValue(user.UserId, out var canRetrieve))
            {
                user.CanUserInfoBeRetrievedByCurrentUser = canRetrieve;
            }
        }
    }

    internal sealed class Query : IQuery<PayloadCoreDto>
    {
        public int Year { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public PayloadFilterCoreDto Filter { get; set; } = new();
        public PayloadOrderingCoreDto Ordering { get; set; } = new();
    }
}
