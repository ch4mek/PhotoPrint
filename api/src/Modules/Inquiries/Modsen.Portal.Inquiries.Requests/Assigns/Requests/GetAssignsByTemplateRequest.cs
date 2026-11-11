using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Contract;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.DataAccess.Npgsql.Entities;
using Modsen.Portal.Inquiries.Requests.Assigns.Requests.RequestDtos;

namespace Modsen.Portal.Inquiries.Requests.Assigns.Requests;

internal sealed class GetAssignsByTemplateRequest
    : IQueryHandler<GetAssignsByTemplateRequest.Query, PageableListOfItems<UserAssignDto>>
{
    private readonly IInquiriesDbContext _dbContext;
    private readonly IIdentityContract _identityContract;

    public GetAssignsByTemplateRequest(IInquiriesDbContext dbContext, IIdentityContract identityContract)
    {
        _dbContext = dbContext;
        _identityContract = identityContract;
    }

    public async Task<PageableListOfItems<UserAssignDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var template = await _dbContext.InquiryTemplates.FirstOrDefaultAsync(x=> x.Id == request.TemplateId, cancellationToken);

        if(template is null)
        {
            throw new EntityNotFoundException("template not found");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        IQueryable<AssignedUserDto> baseQuery;

        if (UserInquiryEntity.InquiryTypesThatCanAccrue.Contains(template.InquiryType))
        {
            var earliestAgreements = _dbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Where(a => a.InquiryTemplateAgreement!.InquiryTemplateId == request.TemplateId)
                .Where(a => a.User!.IsActive)
                .Where(a => a.DateTo == null || a.DateTo >= today)
                .GroupBy(x => x.UserId)
                .Select(g => new
                {
                    UserId = g.Key,
                    EarliestDateFrom = g.Min(x => x.DateFrom)
                });

            baseQuery = _dbContext.UserAgreements
                .Where(x => !x.IsDeleted)
                .AsNoTracking()
                .Where(a => a.InquiryTemplateAgreement!.InquiryTemplateId == request.TemplateId)
                .Where(a => a.DateTo == null || a.DateTo >= today)
                .Join(
                    earliestAgreements,
                    ua => new { ua.UserId, ua.DateFrom },
                    ea => new { ea.UserId, DateFrom = ea.EarliestDateFrom },
                    (ua, ea) => ua)
                .Select(AssignedUserDto.MapAgreement);
        }
        else
        {
            var earliestTemplateAssigns = _dbContext.UserInquiryTemplates
                .AsNoTracking()
                .Where(a => a.InquiryTemplateId == request.TemplateId)
                .Where(a => a.AssignedToUser!.IsActive)
                .Where(a => a.DateTo == null || a.DateTo >= today)
                .GroupBy(x => x.AssignedToUserId)
                .Select(g => new
                {
                    AssignedToUserId = g.Key,
                    EarliestDateFrom = g.Min(x => x.DateFrom)
                });

            baseQuery = _dbContext.UserInquiryTemplates
                .AsNoTracking()
                .Where(a => a.InquiryTemplateId == request.TemplateId)
                .Where(a => a.DateTo == null || a.DateTo >= today)
                .Join(
                    earliestTemplateAssigns,
                    uit => new { uit.AssignedToUserId, uit.DateFrom },
                    eta => new { eta.AssignedToUserId, DateFrom = eta.EarliestDateFrom },
                    (ua, ea) => ua)
                .Select(AssignedUserDto.MapTemplate);
        }

        var appliedQuery = (IQueryable<AssignedUserDto>)queryOptions.ApplyTo(baseQuery);
        var totalCount = await queryOptions
            .RebuildWithoutPagination()
            .ApplyTo(baseQuery)
            .Cast<AssignedUserDto>()
            .CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return EmptyPageableListOfItems<UserAssignDto>.Value;
        }

        var agreements = await appliedQuery.ToListAsync(cancellationToken);

        var departments = (await _identityContract.GetDepartmentsClassificator(cancellationToken))
            .ToDictionary(keySelector: d => d.Id);

        var result = agreements
            .Select(a => new UserAssignDto(
                a.UserId,
                a.UserFullName,
                a.DepartmentId is not null
                    ? departments.GetValueOrDefault(a.DepartmentId)?.FullName
                    : null,
                a.Position,
                a.AvatarFileId,
                a.UserFirstName,
                a.UserLastName,
                a.DateFrom,
                a.DateTo))
            .ToList();

        return result.ToPageable(queryOptions, totalCount);
    }

    internal sealed record Query(int TemplateId, ODataQueryOptions<AssignedUserDto> QueryOptions)
        : IQuery<PageableListOfItems<UserAssignDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
