using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Inquiries;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Agreements.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Agreements.Requests.RequestDtos;
using Modsen.Portal.Inquiries.Requests.Core.BusinessRules;

namespace Modsen.Portal.Inquiries.Requests.Agreements.Requests;

internal sealed class GetUserAgreementHistoryRequest
    : IQueryHandler<GetUserAgreementHistoryRequest.Query, PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>>
{
    private readonly IHasUserAccessBusinessRule _hasUserAccessBusinessRule;
    private readonly IInquiriesDbContext _inquiriesDbContext;

    public GetUserAgreementHistoryRequest(IHasUserAccessBusinessRule hasUserAccessBusinessRule,
        IInquiriesDbContext inquiriesDbContext)
    {
        _hasUserAccessBusinessRule = hasUserAccessBusinessRule;
        _inquiriesDbContext = inquiriesDbContext;
    }

    public async Task<PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>> Handle(
        Query request,
        CancellationToken cancellationToken)
    {
        var accessOptions = AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator;
        var hasUserAccess = await _hasUserAccessBusinessRule.IsCorrespondsToBusiness(null, request.UserId,
            accessOptions, cancellationToken);

        if (!hasUserAccess)
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var combinedRawQuery = GetCombinedRawQuery(request);

        var combinedQuery = combinedRawQuery
            .Select(historyDto => new UserAbsenceHistoryCoreDto
            {
                Id = null,
                InquiryId = historyDto.InquiryId,
                TransactionDate = historyDto.TransactionDate,
                TransactionType = historyDto.TransactionType ?? UserAgreementTransactionType.Using,
                UserAgreementId = historyDto.UserAgreementId,
                InquiryTemplateId = historyDto.InquiryTemplateId,
                InquiryTemplateName = historyDto.InquiryTemplateName,
                InquiryType = historyDto.InquiryType,
                InquiryTypeOrderWeight = historyDto.InquiryTypeOrderWeight,
                TrackingUnitType = historyDto.TrackingUnitType,
                AmountOfDays = historyDto.AmountOfDays,
                Accrued = historyDto.Accrued,
                Used = historyDto.Used,
                Remainder = null,
                Documents = null!,
                Description = historyDto.Description,
                IsCreatedManually = historyDto.IsCreatedManually,
            });

        if (request.Year.HasValue)
        {
            combinedQuery = combinedQuery.Where(historyDto => historyDto.TransactionDate.Year == request.Year.Value);
        }

        var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();

        var result = await request.QueryOptions
            .ApplyTo(combinedQuery)
            .Cast<UserAbsenceHistoryCoreDto>()
            .ToArrayAsync(cancellationToken);

        foreach (var dto in result)
        {
            dto.Documents = dto.TransactionType == UserAgreementTransactionType.Accrual
                ? []
                : (IReadOnlyCollection<InquiryFileCoreDto>)await _inquiriesDbContext.UserInquiryFiles
                    .Where(file => file.UserInquiryId == dto.InquiryId)
                    .Select(file => new InquiryFileCoreDto(file.FileId, file.FileName, file.FileSize))
                    .ToArrayAsync(cancellationToken);
        }

        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(combinedQuery)
            .OfType<UserAbsenceHistoryCoreDto>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>.Of(EmptyPageableListOfItems<UserAbsenceHistoryCoreDto>.Value);
        }

        foreach (var historyDto in result)
        {
            historyDto.Remainder = await combinedRawQuery
                .Where(x => x.InquiryTemplateId == historyDto.InquiryTemplateId)
                .Where(x => x.TransactionDate <= historyDto.TransactionDate)
                .SumAsync(x =>
                    UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(x.InquiryType)
                        ? x.Accrued ?? -x.Used ?? 0
                        : x.Used
                    , cancellationToken
                );
        }

        var pageableResult = PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>.Of(result.ToPageable(request.QueryOptions, totalCount));

        if (request.QueryOptions.Filter != null)
        {
            pageableResult.TotalUsed = await queryOptionsWithoutPagination
                .ApplyTo(combinedQuery)
                .OfType<UserAbsenceHistoryCoreDto>()
                .Where(x => x.InquiryId != null)
                .SumAsync(historyDto => historyDto.Used, cancellationToken);
        }

        return pageableResult;
    }

    private IQueryable<AbsenceHistoryQueryUnionCoreDto> GetCombinedRawQuery(Query request)
    {
        var transactionsRaw = _inquiriesDbContext.UserAgreementTransactions
            .Where(transaction => transaction.UserId == request.UserId)
            .Select(g => new AbsenceHistoryQueryUnionCoreDto
            {
                InquiryId = g.UserInquiryId,
                TransactionDate = g.TransactionDate,
                TransactionType = g.TransactionType,
                UserAgreementId = g.UserAgreementId,
                InquiryTemplateId = g.InquiryTemplateId,
                InquiryTemplateName = g.InquiryTemplate!.Name,
                InquiryType = g.InquiryTemplate.InquiryType,
                InquiryTypeOrderWeight = g.InquiryTemplate.InquiryTypeOrderWeight,
                TrackingUnitType = g.InquiryTemplate.TrackingUnitType,
                AmountOfDays = g.AmountOfDays,
                Accrued = g.AmountOfDays > 0
                    ? g.AmountOfDays
                    : null,
                Used = g.AmountOfDays < 0
                    ? -(g.AmountOfDays)
                    : null,
                Remainder = null,
                Description = g.Desctiption,
                IsCreatedManually = g.IsCreatedManually,
            })
            .AsQueryable();

        var inquiriesRaw = _inquiriesDbContext.UserInquiries
            .Where(inquiry => inquiry.UserId == request.UserId)
            .Where(inquiry => inquiry.AgreementTemplateId == null)
            .Where(inquiry => inquiry.Status == UserInquiryStatus.Approved)
            .Select(inquiry => new AbsenceHistoryQueryUnionCoreDto
            {
                InquiryId = inquiry.Id,
                TransactionDate = !inquiry.UserInquiryAccepts.Any()
                    ? inquiry.CreatedAtUtc
                    : inquiry.UserInquiryAccepts
                        .Select(accept => accept.AcceptedAtUtc)
                        .OrderByDescending(date => date!.Value)
                        .First()!.Value,
                TransactionType = null,
                UserAgreementId = null,
                InquiryTemplateId = inquiry.InquiryTemplateId,
                InquiryTemplateName = inquiry.InquiryTemplate!.Name,
                InquiryType = inquiry.InquiryTemplate.InquiryType,
                InquiryTypeOrderWeight = inquiry.InquiryTemplate.InquiryTypeOrderWeight,
                TrackingUnitType = inquiry.InquiryTemplate.TrackingUnitType,
                AmountOfDays = inquiry.TotalAccountantDays ?? inquiry.TotalHours!.Value,
                Accrued = null,
                Used = inquiry.TotalAccountantDays ?? inquiry.TotalHours!.Value,
                Remainder = null,
                Description = null,
                IsCreatedManually = false,
            })
            .AsQueryable();

        return transactionsRaw.Concat(inquiriesRaw)
            .AsSplitQuery()
            .AsNoTrackingWithIdentityResolution();
    }

    internal sealed record Query(
        int UserId,
        int? Year,
        ODataQueryOptions<UserAbsenceHistoryCoreDto> QueryOptions)
        : IQuery<PageableListOfItemsWithTotalUsed<UserAbsenceHistoryCoreDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
