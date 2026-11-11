using Microsoft.EntityFrameworkCore;
using Modsen.Portal.Core.Services;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;
using Modsen.Portal.Identity.DataAccess.Npgsql.DatabaseEntities;

namespace Modsen.Portal.Identity.Requests.Microfrontends.Requests;

internal sealed class UpdateMicrofrontendOptionsRequest
    : ICommandHandler<UpdateMicrofrontendOptionsRequest.Command, string>
{
    private readonly IClockService _clockService;
    private readonly IIdentityDbContext _identityDbContext;
    private readonly IMicrofrontendsCache _microfrontendsCache;

    public UpdateMicrofrontendOptionsRequest(
        IClockService clockService,
        IIdentityDbContext identityDbContext,
        IMicrofrontendsCache microfrontendsCache)
    {
        _clockService = clockService;
        _identityDbContext = identityDbContext;
        _microfrontendsCache = microfrontendsCache;
    }

    public async Task<string> Handle(Command request, CancellationToken cancellationToken)
    {
        var config = await _identityDbContext.MicrofrontendsConfigs
            .OrderBy(config => config.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (config is null)
        {
            config = new MicrofrontendsConfigEntity();
            _identityDbContext.MicrofrontendsConfigs.Add(config);
        }

        config.JsonData = request.Config;
        config.LastUpdate = _clockService.UtcNow;

        await _identityDbContext.SaveChanges(cancellationToken);

        _microfrontendsCache.Clear();

        return config.JsonData;
    }

    internal sealed record Command(string Config) : ICommand<string>, ITransactionalRequest;
}
