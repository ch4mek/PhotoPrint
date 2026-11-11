using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Core.Builders;

namespace Modsen.Portal.TeamBoard.Requests.Skills.HardSkills.Requests;

internal sealed class GetOdpHardSkillsRequest
    : IQueryHandler<GetOdpHardSkillsRequest.Query, PageableListOfItems<OdpHardSkill>>
{
    private readonly IHardSkillQueryBuilder _queryBuilder;

    public GetOdpHardSkillsRequest(IHardSkillQueryBuilder queryBuilder)
    {
        _queryBuilder = queryBuilder;
    }

    public async Task<PageableListOfItems<OdpHardSkill>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var query = _queryBuilder
            .WithDepartment(request.DepartmentId)
            .WithSpecializationTemplate(request.SpecializationId)
            .Build()
            .Select(OdpHardSkill.Map);

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpHardSkill>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpHardSkill>.Value;
        }

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpHardSkill>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpHardSkill> QueryOptions,
        string? DepartmentId,
        int? SpecializationId) : IQuery<PageableListOfItems<OdpHardSkill>>;


    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
