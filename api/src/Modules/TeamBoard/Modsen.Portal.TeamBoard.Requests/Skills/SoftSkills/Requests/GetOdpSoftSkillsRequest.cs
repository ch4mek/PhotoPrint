using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SoftSkills.Requests;

internal sealed class GetOdpSoftSkillsRequest
    : IQueryHandler<GetOdpSoftSkillsRequest.Query, PageableListOfItems<OdpSoftSkill>>
{
    private readonly ITeamBoardDbContext _dbContext;

    public GetOdpSoftSkillsRequest(ITeamBoardDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PageableListOfItems<OdpSoftSkill>> Handle(Query request, CancellationToken cancellationToken)
    {
        var queryOptions = request.QueryOptions;

        var query = _dbContext.SoftSkills
            .AsNoTracking()
            .Where(skill => skill.IsActive)
            .Select(OdpSoftSkill.Map);

        var total = await queryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .OfType<OdpSoftSkill>()
            .CountAsync(cancellationToken);

        if (total == 0)
        {
            return EmptyPageableListOfItems<OdpSoftSkill>.Value;
        }

        var result = await queryOptions
            .ApplyTo(query)
            .Cast<OdpSoftSkill>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(queryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpSoftSkill> QueryOptions)
        : IQuery<PageableListOfItems<OdpSoftSkill>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
