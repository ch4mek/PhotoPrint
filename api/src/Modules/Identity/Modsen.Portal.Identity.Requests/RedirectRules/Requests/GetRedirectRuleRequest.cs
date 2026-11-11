using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.RedirectRules.Requests;

internal sealed class GetRedirectRuleRequest : IQueryHandler<GetRedirectRuleRequest.Query, RedirectRuleDto>
{
    private readonly IRedirectRulesCache _redirectRulesCache;

    public GetRedirectRuleRequest(IRedirectRulesCache redirectRulesCache)
    {
        _redirectRulesCache = redirectRulesCache;
    }

    public async Task<RedirectRuleDto> Handle(Query request, CancellationToken cancellationToken)
    {
        var entity = await _redirectRulesCache.GetRedirectRuleByKey(request.RedirectKey, cancellationToken)
            ?? throw new EntityNotFoundException($"Redirect rule hasn't been found by {request.RedirectKey} key");
        
        return new RedirectRuleDto(entity.Id, entity.RedirectKey, entity.RedirectTo);
    }

    internal sealed record Query(string RedirectKey) : IQuery<RedirectRuleDto>;
}
