using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Services.PortalSettings;

namespace Modsen.Portal.Identity.Requests.PortalSettings;

internal sealed class GetPortalSettingsRequest : IQueryHandler<GetPortalSettingsRequest.Query, List<PortalSettingDto>>
{
    private readonly IPortalSettingsAccessor _portalSettingCache;

    public GetPortalSettingsRequest(IPortalSettingsAccessor portalSettingCache)
    {
        _portalSettingCache = portalSettingCache;
    }

    public async Task<List<PortalSettingDto>> Handle(Query request, CancellationToken cancellationToken)
    {
        var portalSettings = await _portalSettingCache.GetAll(cancellationToken);

        return portalSettings.Select(x => new PortalSettingDto
        {
            Name = x.Name,
            Value = x.Value ?? string.Empty,
            ValueType = x.ValueType
        }).ToList();
    }
    internal sealed record Query : IQuery<List<PortalSettingDto>>, IIdentityRequest;
}
