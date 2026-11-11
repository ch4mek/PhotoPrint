using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.Requests.Payslips.Requests.RequestDto;

namespace Modsen.Portal.TeamBoard.Requests.Payslips.Requests;

internal sealed class GetPayslipsRequest : IQueryHandler<GetPayslipsRequest.Query, PageableListOfItems<OdpPayslip>>
{
    private readonly ITeamBoardDbContext _teamBoardDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetPayslipsRequest(ITeamBoardDbContext teamBoardDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamBoardDbContext = teamBoardDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpPayslip>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var hasAccess = _authenticatedUserContext.GetRequiredUserId() == request.UserId;

        if (!hasAccess)
        {
            throw new ActionNotAllowedException("You don\'t have access to another user\'s payslips");
        }

        var query = _teamBoardDbContext.Payslips
            .Where(payslip => payslip.UserId == request.UserId)
            .Include(payslip => payslip.User)
            .AsNoTracking()
            .Select(p =>
                new OdpPayslip
                {
                    FileId = p.FileId,
                    FileName = p.FileName,
                    ReportDate = p.ReportDate,
                    UserId = p.UserId,
                    CreatedAtUtc = p.CreatedAtUtc,
                });

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpPayslip>()
            .ToListAsync(cancellationToken);

        var totalItems = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpPayslip>()
            .CountAsync(cancellationToken);

        if (totalItems is 0)
        {
            return EmptyPageableListOfItems<OdpPayslip>.Value;
        }

        var page = request.QueryOptions.Skip.Value / request.QueryOptions.Top.Value + 1;
        var pageSize = request.QueryOptions.Top.Value;

        return result.ToPageable()
            .WithPaging(page, pageSize)
            .WithTotal(totalItems);
    }

    internal sealed record Query(int UserId, ODataQueryOptions<OdpPayslip> QueryOptions)
        : IQuery<PageableListOfItems<OdpPayslip>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.UserId)
                .NotNull();

            RuleFor(x => x.QueryOptions)
                .SetValidator(new ODataQueryOptionsValidator());

            RuleFor(x => x.QueryOptions.RawValues)
                .DivisiblePaginationRule();
        }
    }
}
