using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.UseCases.Core.Calculators;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.TeamBoard.Contract;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Core.Resolvers;

internal interface IAssignDefaultInquiryTemplatesResolver
{
    Task ComposeAsync(int userId, CancellationToken cancellationToken);

    Task ComposeWithAccrualsAsync(int userId, CancellationToken cancellationToken);

    Task ComposeWithAccrualsAsync(
        int userId,
        DateOnly contractDateFrom,
        DateOnly? contractDateTo,
        CancellationToken cancellationToken);
}

internal class AssignDefaultInquiryTemplatesResolver : IAssignDefaultInquiryTemplatesResolver
{
    private readonly IClockService _clockService;
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IUserAgreementTransactionComposer _userAgreementTransactionComposer;
    private readonly ITeamBoardContract _teamBoardContract;
    private readonly IDateCalculator _dateCalculator;

    public AssignDefaultInquiryTemplatesResolver(
        IClockService clockService,
        IInquiriesDbContext inquiriesDbContext,
        IUserAgreementTransactionComposer userAgreementTransactionComposer,
        ITeamBoardContract teamBoardContract,
        IDateCalculator dateCalculator)
    {
        _clockService = clockService;
        _inquiriesDbContext = inquiriesDbContext;
        _userAgreementTransactionComposer = userAgreementTransactionComposer;
        _teamBoardContract = teamBoardContract;
        _dateCalculator = dateCalculator;
    }

    public async Task ComposeAsync(int userId, CancellationToken cancellationToken)
    {
        await AssignDefaultInquiryTemplates(userId, cancellationToken);
        await AssignDefaultAgreements(userId, cancellationToken);

        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ComposeWithAccrualsAsync(
        int userId,
        CancellationToken cancellationToken)
    {
        await ComposeAsync(userId, cancellationToken);

        var result = await GetContractDates(userId, cancellationToken);
        if (result is not null)
        {
            var (contractDateFrom, contractDateTo) = result.Value;
            await CreateAccrualTransactions(userId, contractDateFrom, contractDateTo, cancellationToken);
        }
    }

    public async Task ComposeWithAccrualsAsync(
        int userId,
        DateOnly contractDateFrom,
        DateOnly? contractDateTo,
        CancellationToken cancellationToken)
    {
        await ComposeAsync(userId, cancellationToken);
        ValidateContractDates(contractDateFrom, contractDateTo);

        if (!contractDateTo.HasValue || contractDateTo.Value >= _clockService.TodayUtc())
        {
            await CreateAccrualTransactions(userId, contractDateFrom, contractDateTo, cancellationToken);
        }  
    }

    private async Task AssignDefaultInquiryTemplates(int userId, CancellationToken cancellationToken)
    {
        var defaultInquiryTemplateIds = await _inquiriesDbContext.InquiryTemplates
            .Where(x => x.IsActive)
            .Where(x => x.IsDefault)
            .Where(x => !UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.InquiryType))
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var inquiryTemplateId in defaultInquiryTemplateIds)
        {
            var assignment = new UserInquiryTemplateEntity
            {
                AssignedToUserId = userId,
                InquiryTemplateId = inquiryTemplateId,
                DateFrom = _clockService.TodayUtc(),
                DateTo = null,
            };

            _inquiriesDbContext.UserInquiryTemplates.Add(assignment);
        }
    }

    private async Task AssignDefaultAgreements(int userId, CancellationToken cancellationToken)
    {
        var userAgreements = new List<UserAgreementEntity>();

        var defaultAgreementIds = await _inquiriesDbContext.InquiryTemplateAgreements
            .Where(x => !x.IsDeleted)
            .Where(x => x.IsDefault)
            .Where(x =>
                x.InquiryTemplate != null &&
                UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.InquiryTemplate.InquiryType)
            )
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var defaultAgreementId in defaultAgreementIds)
        {
            var agreementAssign = new UserAgreementEntity
            {
                UserId = userId,
                InquiryAgreementTemplateId = defaultAgreementId,
                DateFrom = _clockService.TodayUtc(),
                DateTo = null,
            };

            userAgreements.Add(agreementAssign);
        }

        _inquiriesDbContext.UserAgreements.AddRange(userAgreements);
    }

    private async Task<(DateOnly contractDateFrom, DateOnly? contractDateTo)?> GetContractDates(int userId, CancellationToken cancellationToken)
    {
        var today = _clockService.TodayUtc();

        var contracts = await _teamBoardContract.GetUserContractsDates(
            userId,
            cancellationToken,
            onlyActiveUsers: false);

        var lastActiveContract = contracts
            .Where(x => x.ContractStartDate <= today)
            .OrderBy(x => x.ContractStartDate)
            .LastOrDefault();

        if (lastActiveContract == null || !lastActiveContract.ContractStartDate.HasValue)
        {
            return null;
        }

        var contractDateFrom = lastActiveContract.ContractStartDate.Value;
        var contractDateTo = lastActiveContract.FireDate ?? lastActiveContract.ContractEndDate;

        return (contractDateFrom, contractDateTo);
    }

    private static void ValidateContractDates(DateOnly contractDateFrom, DateOnly? contractDateTo)
    {
        if (contractDateTo.HasValue &&
            contractDateFrom >= contractDateTo.Value)
        {
            throw new InvalidDataAppException("ContractDateTo must be ghreater than ContractDateFrom");
        }
    }

    private async Task CreateAccrualTransactions(
        int userId,
        DateOnly contractDateFrom,
        DateOnly? contractDateTo,
        CancellationToken cancellationToken)
    {
        var now = _clockService.UtcNow;
        var today = _clockService.TodayUtc();

        var transactionInfo = new UserAgreementTransactionInfoCoreDto
        {
            UserId = userId,
            ContractStartDate = contractDateFrom,
            ContractEndDate = contractDateTo,
            TransactionDate = now,
            IsTransactionForMonth = _dateCalculator.IsWholeMonthsPassed(contractDateFrom, today),
            IsTransactionForYear = _dateCalculator.IsWholeYearsPassed(contractDateFrom, today),
            IsTransactionForIncompleteYear = _dateCalculator
                .IsWholeYearsPassed(contractDateFrom.AddDays(-1), today)
        };

        var transactions = await _userAgreementTransactionComposer.Compose(transactionInfo, cancellationToken);

        _inquiriesDbContext.UserAgreementTransactions.AddRange(transactions);
        await _inquiriesDbContext.SaveChangesAsync(cancellationToken);
    }
}
