using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.BusinessRules;
using Modsen.Portal.TeamBoard.Requests.UserSalary.Core.Encryptor;

namespace Modsen.Portal.TeamBoard.Requests.UserSalary.Requests;

internal sealed class GetOdpUserSalaryDetailsByUserIdRequest
    : IQueryHandler<GetOdpUserSalaryDetailsByUserIdRequest.Query, PageableListOfItems<UserSalaryDetailsDto>>
{
    private readonly ICanUserAccessSalaryBusinessRule _canUserAccessSalaryBusinessRule;
    private readonly ITeamBoardDbContext _teamboardDbContext;
    private readonly ISalaryEncryptor _salaryEncryptor;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetOdpUserSalaryDetailsByUserIdRequest(ITeamBoardDbContext teamboardDbContext,
        ISalaryEncryptor salaryEncryptor,
        ICanUserAccessSalaryBusinessRule canUserAccessSalaryBusinessRule,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _teamboardDbContext = teamboardDbContext;
        _salaryEncryptor = salaryEncryptor;
        _canUserAccessSalaryBusinessRule = canUserAccessSalaryBusinessRule;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<UserSalaryDetailsDto>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var conversationId = request.ConversationId;
        var queryOptions = request.QueryOptions;

        var hasAccess = await _canUserAccessSalaryBusinessRule.IsCorrespondsToBusiness(request.UserId,
            cancellationToken);
        if (!hasAccess)
        {
            throw new ActionNotAllowedException();
        }

        var query = _teamboardDbContext.UserSalaries
            .AsNoTracking()
            .Where(salaryDetails => salaryDetails.UserId == request.UserId)
            .Select(OdpUserSalaryDetails.MapUserSalary);

        if (conversationId is not null)
        {
            query = query.Where(details => details.ConversationId == conversationId);
        }

        if (_authenticatedUserContext.GetRoles().IsSystemAdministrator() ||
            _authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsIntern())
        {
            query = query.Where(details => details.IsCurrent);
        }

        var salaries = (await query.ToListAsync(cancellationToken))
            .Select(s => new UserSalaryDetailsDto
            {
                Id = s.Id,
                UserId = s.UserId,
                SalaryFrom = s.SalaryFrom,
                SalaryUsd = _salaryEncryptor.Decrypt(s.SalaryUsd),
                Source = s.Source.ToString(),
                ConversationId = s.ConversationId,
                IsCurrent = s.IsCurrent
            })
            .AsQueryable();

        var result = queryOptions.ApplyTo(salaries)
            .OfType<UserSalaryDetailsDto>()
            .ToList();

        var total = queryOptions.RebuildWithoutPagination()
            .ApplyTo(salaries)
            .OfType<UserSalaryDetailsDto>()
            .Count();

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(int? ConversationId, int UserId,
        ODataQueryOptions<UserSalaryDetailsDto> QueryOptions) : IQuery<PageableListOfItems<UserSalaryDetailsDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
