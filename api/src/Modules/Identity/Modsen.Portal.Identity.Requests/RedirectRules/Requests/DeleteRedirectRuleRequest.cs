using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.Requests.RedirectRules.Requests.RequestDto;

namespace Modsen.Portal.Identity.Requests.RedirectRules.Requests;

internal sealed class DeleteRedirectRuleRequest : ICommandHandler<DeleteRedirectRuleRequest.Command,RedirectRuleDto>
{
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IRedirectRulesCache _redirectRulesCache;

    public DeleteRedirectRuleRequest(IIdentityDbContext identityDbContext, IRedirectRulesCache redirectRulesCache)
    {
        _identityDbContext = identityDbContext;
        _redirectRulesCache = redirectRulesCache;
    }

    public async Task<RedirectRuleDto> Handle(Command request, CancellationToken cancellationToken)
    {
        var redirectRule = await _identityDbContext.RedirectRules
            .FirstOrDefaultAsync(redirectRule => redirectRule.Id == request.Id, cancellationToken)
            ?? throw new EntityNotFoundException($"Redirect rule hasn't been found by {request.Id} id");

        _identityDbContext.RedirectRules.Remove(redirectRule);
        await _identityDbContext.SaveChanges(cancellationToken);
        _redirectRulesCache.Clear();

        return new RedirectRuleDto(redirectRule.Id, redirectRule.RedirectKey, redirectRule.RedirectTo);
    }

    internal sealed record Command(Guid Id) : ICommand<RedirectRuleDto>, ITransactionalRequest;
}
