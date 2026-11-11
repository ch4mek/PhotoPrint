using Modsen.Portal.Core;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Finance.Requests.Core.Builders;
using Modsen.Portal.Finance.Requests.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.CoreDto;
using Modsen.Portal.Finance.Requests.ProjectRates.Core.Encryptor;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Finance.Requests.ProjectRates.Requests;

internal class GetAllProjectRatesRequest : IQueryHandler<GetAllProjectRatesRequest.Query, PageableListOfItems<UserProjectRatesCoreDto>>
{
    private readonly IProjectRateEncryptor _numberEncryptor;
    private readonly IGetProjectRatesQueryBuilder _getRatesQueryBuilder;
    private readonly IIdentityContract _identityContract;
    private readonly ITeamBoardContract _teamBoardContract;

    public GetAllProjectRatesRequest(IGetProjectRatesQueryBuilder getRatesQueryBuilder, IProjectRateEncryptor numberEncryptor,
        IIdentityContract identityContract, ITeamBoardContract teamBoardContract)
    {
        _numberEncryptor = numberEncryptor;
        _getRatesQueryBuilder = getRatesQueryBuilder;
        _identityContract = identityContract;
        _teamBoardContract = teamBoardContract;
    }

    public async Task<PageableListOfItems<UserProjectRatesCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var getRatesQueryBuilderDto = await _getRatesQueryBuilder
            .ForYearAndMonth(request.Year, request.Month)
            .WithPagination(request.PageNumber * request.PageSize, request.PageSize)
            .WithFilter(request.Filter)
            .WithOrdering(request.Ordering)
            .BuildRates(cancellationToken);

        var userHistoricalDeps = await _identityContract.UsersHistoricalDepartment(
            request.Year, request.Month, getRatesQueryBuilderDto.UsersIds, cancellationToken);

        var accessOptions = AccessOptions.Manager | AccessOptions.Self;
        var visibility = await _teamBoardContract.CanUsersBeRetrievedByUser(getRatesQueryBuilderDto.UsersIds.ToArray(), accessOptions,
            cancellationToken);

        var usersWithProjectRates = new List<UserProjectRatesCoreDto>();

        foreach (var userId in getRatesQueryBuilderDto.UsersIds)
        {
            var userAssigns = getRatesQueryBuilderDto.ProjectUsers.FirstOrDefault(x => x.Key == userId)?.Select(g => g).ToList();
            var userRates = getRatesQueryBuilderDto.Rates.FirstOrDefault(x => x.Key == userId)?.Select(g => g);

            var userProjectRates = new List<ProjectRateGridItem>();

            foreach (var assign in userAssigns!)
            {
                var rate = userRates?.FirstOrDefault(r => r.ProjectId == assign.ProjectId);

                var projectRateCoreDto = new ProjectRateGridItem
                {
                    ProjectRateId = rate?.Id,
                    ProjectId = assign.ProjectId,
                    ProjectName = assign.Project!.ProjectName,
                    ProjectColor = assign.Project!.Color,
                    ClientId = assign.Project!.ClientId,
                    ClientName = assign.Project!.Client!.Name,
                    ClientLegalEntity = assign.Project!.Client.LegalEntity,
                    HourlyRate = rate == null
                        ? null
                        : _numberEncryptor.Decrypt(rate.HourlyRate),
                    CurrencyId = rate?.CurrencyId,
                    CanRateBeManagedByCurrentUser = getRatesQueryBuilderDto.ManagedProjectUsersIds.Contains(assign.Id)
                };
                userProjectRates.Add(projectRateCoreDto);
            }

            var userEntity = userAssigns[0].User!;

            var userWithProjectRates = new UserProjectRatesCoreDto
            {
                User = UserCoreDto.Of(userEntity, userHistoricalDeps.GetValueOrDefault(userEntity.Id)
                                                  ?? userEntity.DepartmentId), //use current user's department, if there is no data about user's department for specified month
                ProjectRates = userProjectRates,
                CanUserInfoBeRetrievedByCurrentUser = visibility.GetValueOrDefault(userEntity.Id)
            };

            var allCurrencies = userProjectRates.Select(x => x.CurrencyId).Distinct().ToList();

            if (allCurrencies.Count == 1 && !string.IsNullOrEmpty(allCurrencies.Single())) //all currencies are same
            {
                userWithProjectRates.TotalRate = userProjectRates.Sum(x => x.HourlyRate);
                userWithProjectRates.CurrencyId = allCurrencies!.Single();
            }

            usersWithProjectRates.Add(userWithProjectRates);
        }

        var result = PageableListOfItemsExtensions
                .ToPageable(usersWithProjectRates)
                .WithPaging(request.PageNumber, request.PageSize)
                .WithTotal(getRatesQueryBuilderDto.TotalUsersCount);

        return result;
    }

    internal sealed record Query : IQuery<PageableListOfItems<UserProjectRatesCoreDto>>
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public RatesFilterCoreDto Filter { get; set; } = new();
        public FinanceOrderingCoreDto Ordering { get; set; } = new();
    }
}
