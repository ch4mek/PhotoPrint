using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.RedirectRules.Requests;

internal sealed class GetRedirectRulesRequest : IQueryHandler<GetRedirectRulesRequest.Query, List<RedirectRuleDto>>
{
    private readonly IRedirectRulesCache _redirectRulesCache;

    public GetRedirectRulesRequest(IRedirectRulesCache redirectRulesCache)
    {
        _redirectRulesCache = redirectRulesCache;
    }

    public async Task<List<RedirectRuleDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var rules = await _redirectRulesCache.GetAllRedirectRules(cancellationToken);

        return rules.ConvertAll(redirectRule => new RedirectRuleDto(
            redirectRule.Id, redirectRule.RedirectKey, redirectRule.RedirectTo));
    }

    internal sealed record Query : IQuery<List<RedirectRuleDto>>;
}
