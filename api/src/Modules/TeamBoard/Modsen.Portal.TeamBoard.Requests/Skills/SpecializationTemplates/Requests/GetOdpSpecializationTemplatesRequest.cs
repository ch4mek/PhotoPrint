using FluentValidation;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.OData;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.UseCases.ValidationRules;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql;
using Modsen.Portal.TeamBoard.DataAccess.Npgsql.OData;
using Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Core.BusinessRules;

namespace Modsen.Portal.TeamBoard.Requests.Skills.SpecializationTemplates.Requests;

internal sealed class GetOdpSpecializationTemplatesRequest :
    IQueryHandler<GetOdpSpecializationTemplatesRequest.Query, PageableListOfItems<OdpSpecializationTemplate>>
{
    private readonly ICanUserUpdateSpecializationTemplateBusinessRule _canUserUpdateTemplateBusinessRule;
    private readonly ITeamBoardDbContext _context;

    public GetOdpSpecializationTemplatesRequest(
        ICanUserUpdateSpecializationTemplateBusinessRule canUserUpdateTemplateBusinessRule, ITeamBoardDbContext context)
    {
        _canUserUpdateTemplateBusinessRule = canUserUpdateTemplateBusinessRule;
        _context = context;
    }

    public async Task<PageableListOfItems<OdpSpecializationTemplate>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        var pageableSpecializationTemplates = await GetTemplatesAsync(request, cancellationToken);

        foreach (var specializationTemplate in pageableSpecializationTemplates.Items)
        {
            var canBeUpdated = await _canUserUpdateTemplateBusinessRule.IsCorrespondsToBusiness(
                specializationTemplate.CreatedById,
                specializationTemplate.Id,
                cancellationToken);

            specializationTemplate.CanBeUpdated = canBeUpdated;
        }

        return pageableSpecializationTemplates;
    }

    private async Task<PageableListOfItems<OdpSpecializationTemplate>> GetTemplatesAsync(Query request,
        CancellationToken cancellationToken)
    {
        var baseQuery = _context.SpecializationTemplates
            .Where(x => x.IsActive)
            .AsQueryable();
        if (request.DepartmentId is not null)
        {
            baseQuery = baseQuery.Where(st =>
                st.SpecializationTemplateDepartments.Any(d => d.DepartmentId == request.DepartmentId));
        }

        var query = baseQuery.Select(OdpSpecializationTemplate.Map);
        var total = await request.QueryOptions.RebuildWithoutPagination()
            .ApplyTo(query)
            .Cast<OdpSpecializationTemplate>()
            .CountAsync(cancellationToken);

        var result = await request.QueryOptions
            .ApplyTo(query)
            .Cast<OdpSpecializationTemplate>()
            .ToListAsync(cancellationToken);

        return result.ToPageable(request.QueryOptions, total);
    }

    internal sealed record Query(ODataQueryOptions<OdpSpecializationTemplate> QueryOptions, string? DepartmentId)
        : IQuery<PageableListOfItems<OdpSpecializationTemplate>>;

    internal sealed class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.QueryOptions).SetValidator(new ODataQueryOptionsValidator());
            RuleFor(x => x.QueryOptions.RawValues).DivisiblePaginationRule();
        }
    }
}
