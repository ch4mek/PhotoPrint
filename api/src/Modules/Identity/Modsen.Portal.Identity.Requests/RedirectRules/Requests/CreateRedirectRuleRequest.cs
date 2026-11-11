using Modsen.Portal.Core;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.RedirectRules.Requests;

internal sealed class CreateRedirectRuleRequest : ICommandHandler<CreateRedirectRuleRequest.Command, RedirectRuleDto>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IRedirectRulesCache _redirectRulesCache;

    public CreateRedirectRuleRequest(
        IIdentityDbContext identityDbContext,
        IGuidGenerator guidGenerator,
        IRedirectRulesCache redirectRulesCache)
    {
        _identityDbContext = identityDbContext;
        _guidGenerator = guidGenerator;
        _redirectRulesCache = redirectRulesCache;
    }

    public async Task<RedirectRuleDto> Handle(Command request, CancellationToken cancellationToken)
    {
        if (!Uri.IsWellFormedUriString(request.RedirectTo, UriKind.Absolute))
        {
            throw new ArgumentException(PortalMessages.RedirectToFieldNotProperlyFormed);
        }

        var key = request.RedirectKey ?? _guidGenerator.Create().ToString();
        var rule = await CreateRedirectRuleDbQuery(_guidGenerator.Create(), key, request.RedirectTo, cancellationToken);
        _redirectRulesCache.Clear();

        return rule;
    }

    public sealed record Command(string? RedirectKey, string RedirectTo)
        : ICommand<RedirectRuleDto>, ITransactionalRequest;

    private async Task<RedirectRuleDto> CreateRedirectRuleDbQuery(Guid id, string redirectKey, string redirectTo,
    CancellationToken cancellationToken)
    {
        var entry = await _identityDbContext.RedirectRules.AddAsync(new RedirectRule
        {
            Id = id,
            RedirectKey = redirectKey,
            RedirectTo = redirectTo
        }, cancellationToken);

        await _identityDbContext.SaveChanges(cancellationToken);

        return new RedirectRuleDto(entry.Entity.Id, entry.Entity.RedirectKey, entry.Entity.RedirectTo);
    }
}
