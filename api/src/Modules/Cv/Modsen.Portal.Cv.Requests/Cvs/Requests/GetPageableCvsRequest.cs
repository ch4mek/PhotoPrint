using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Cv.DataAccess.Npgsql;
using Modsen.Portal.Cv.Requests.Cvs.Core.CoreDto;

namespace Modsen.Portal.Cv.Requests.Cvs.Requests;

internal sealed class GetPageableCvsRequest
    : IQueryHandler<GetPageableCvsRequest.Query, PageableListOfItems<OdpCvDto>>
{
    private readonly ICvDbContext _cvDbContext;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public GetPageableCvsRequest(
        ICvDbContext cvDbContext,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _cvDbContext = cvDbContext;
        _authenticatedUserContext = authenticatedUserContext;
    }

    public async Task<PageableListOfItems<OdpCvDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var query = _cvDbContext.Cvs
            .AsNoTracking()
            .Select(OdpCvDto.MapCv);

        query = GetFilteredQuery(query);

        var queryResult = await queryOptions
            .ApplyTo(query)
            .Cast<OdpCvDto>()
            .ToListAsync(cancellationToken);

        var totalCount = await queryOptions
            .RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpCvDto>()
            .CountAsync(cancellationToken);

        return queryResult.ToPageable(queryOptions, totalCount);
    }

    private IQueryable<OdpCvDto> GetFilteredQuery(IQueryable<OdpCvDto> query)
    {
        if (_authenticatedUserContext.GetRoles().IsHeadOfDepartment())
        {
            query = query.Where(x => x.UserDepartmentId == _authenticatedUserContext.DepartmentId);
        }

        if (_authenticatedUserContext.GetRoles().IsGroupManager())
        {
            query = query.Where(x => x.UserResourceGroupId == _authenticatedUserContext.ResourceGroupId);
        }

        if (_authenticatedUserContext.GetRoles().IsEmployee() ||
            _authenticatedUserContext.GetRoles().IsSystemAdministrator())
        {
            query = query.Where(x => x.UserId == _authenticatedUserContext.UserId);
        }

        return query;
    }

    internal sealed record Query(ODataQueryOptions<OdpCvDto> QueryOptions)
        : IQuery<PageableListOfItems<OdpCvDto>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
