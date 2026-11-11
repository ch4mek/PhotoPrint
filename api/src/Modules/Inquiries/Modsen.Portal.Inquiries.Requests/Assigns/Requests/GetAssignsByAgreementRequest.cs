using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Assigns.Core.CoreDto;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class GetAssignsByAgreementRequest
    : IQueryHandler<GetAssignsByAgreementRequest.Query, PageableListOfItems<UserAgreementAssignCoreDto>>
{
    private readonly IInquiriesDbContext _dbContext;
    private readonly IIdentityContract _identityContract;
    private readonly IClockService _clockService;

    public GetAssignsByAgreementRequest(
        IInquiriesDbContext dbContext,
        IIdentityContract identityContract,
        IClockService clockService)
    {
        _dbContext = dbContext;
        _identityContract = identityContract;
        _clockService = clockService;
    }

    public async Task<PageableListOfItems<UserAgreementAssignCoreDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        _ = await _dbContext.InquiryTemplateAgreements
            .FirstOrDefaultAsync(x => x.Id == request.AgreementId, cancellationToken)
            ?? throw new EntityNotFoundException(PortalMessages.AgreementTemplateNotFound);

        var today = _clockService.TodayUtc();

        var earliestAgreements = _dbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .Where(a => a.InquiryAgreementTemplateId == request.AgreementId)
            .Where(a => a.User!.IsActive)
            .Where(a => a.DateTo == null || a.DateTo >= today)
            .GroupBy(x => x.UserId)
            .Select(g => new
            {
                UserId = g.Key,
                EarliestDateFrom = g.Min(x => x.DateFrom)
            });

        var query = _dbContext.UserAgreements
            .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .Where(a => a.InquiryAgreementTemplateId == request.AgreementId)
            .Where(a => a.DateTo == null || a.DateTo >= today)
            .Join(
                earliestAgreements,
                ua => new { ua.UserId, ua.DateFrom },
                ea => new { ea.UserId, DateFrom = ea.EarliestDateFrom },
                (ua, ea) => ua)
            .Select(UserAgreementAssignCoreDto.MapAssign);

        var totalCount = await request.QueryOptions
            .RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<UserAgreementAssignCoreDto>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<UserAgreementAssignCoreDto>.Value;
        }

        var userAgreements = await request.QueryOptions
            .ApplyTo(query)
            .Cast<UserAgreementAssignCoreDto>()
            .ToListAsync(cancellationToken);

        var departments =
            (await _identityContract.GetDepartmentsClassificator(cancellationToken))
            .ToDictionary(keySelector: dept => dept.Id);

        foreach (var userAgreement in userAgreements)
        {
            userAgreement.Department = userAgreement.DepartmentId == null
                ? null
                : departments.GetValueOrDefault(userAgreement.DepartmentId)?.FullName;
        }

        return userAgreements.ToPageable(request.QueryOptions, totalCount);
    }

    internal sealed record Query(int AgreementId, ODataQueryOptions<UserAgreementAssignCoreDto> QueryOptions)
        : IQuery<PageableListOfItems<UserAgreementAssignCoreDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
