using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Inquiries.DataAccess.Npgsql;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.BusinessRules;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.CoreDto;
using Modsen.Portal.Inquiries.Requests.Inquiries.Core.Resolvers;

namespace Modsen.Portal.Inquiries.Requests.Inquiries.Request;

internal sealed class GetPageableInquiriesRequest : IQueryHandler<GetPageableInquiriesRequest.Query, PageableListOfItems<UserInquiryCoreDto>>
{
    private readonly IInquiriesDbContext _inquiriesDbContext;
    private readonly IHasUserAccessToInquiryBusinessRule _hasUserAccessToInquiryBusinessRule;
    private readonly ISetAvailabilityToApproveRejectResolver _setAvailabilityToApproveRejectResolver;

    public GetPageableInquiriesRequest(
        IInquiriesDbContext inquiriesDbContext,
        IHasUserAccessToInquiryBusinessRule hasUserAccessToInquiryBusinessRule,
        ISetAvailabilityToApproveRejectResolver setAvailabilityToApproveRejectResolver)
    {
        _inquiriesDbContext = inquiriesDbContext;
        _hasUserAccessToInquiryBusinessRule = hasUserAccessToInquiryBusinessRule;
        _setAvailabilityToApproveRejectResolver = setAvailabilityToApproveRejectResolver;
    }

    public async Task<PageableListOfItems<UserInquiryCoreDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        if (!await _hasUserAccessToInquiryBusinessRule.IsCorrespondsToBusiness(
                request.UserId, AccessOptions.Manager | AccessOptions.Hr | AccessOptions.Accountant | AccessOptions.Coordinator, cancellationToken))
        {
            throw new EntityNotFoundException(PortalMessages.UserNotFound);
        }

        var query = _inquiriesDbContext.UserInquiries
            .AsNoTracking()
            .Where(x => request.UserId == x.UserId)
            .Include(x => x.UserInquiryDates)
                .ThenInclude(d => d.CalendarExclusion)
            .Include(x => x.UserInquiryAccepts)
            .Include(x => x.InquiryTemplate)
            .Select(UserInquiryCoreDto.Map)
            .AsSplitQuery();

        var queryOptionsWithoutPagination = request.QueryOptions.RebuildWithoutPagination();

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<UserInquiryCoreDto>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptionsWithoutPagination
            .ApplyTo(query)
            .OfType<UserInquiryCoreDto>()
            .CountAsync(cancellationToken);

        result = await _setAvailabilityToApproveRejectResolver.ResolveAsync(result, null, cancellationToken);

        return result.ToPageable(request.QueryOptions, totalCount);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<UserInquiryCoreDto> QueryOptions) : IQuery<PageableListOfItems<UserInquiryCoreDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
