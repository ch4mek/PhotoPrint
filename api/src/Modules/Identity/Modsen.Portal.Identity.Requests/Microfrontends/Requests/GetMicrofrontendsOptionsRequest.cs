using System.Text.Json;
using System.Text.Json.Nodes;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Identity.DataAccess.Npgsql.Cache;

namespace Modsen.Portal.Identity.Requests.Microfrontends.Requests;

internal sealed class GetMicrofrontendsOptionsRequest : IQueryHandler<GetMicrofrontendsOptionsRequest.Query, string>
{
    private readonly IAuthenticatedUserContext _authContext;
    private readonly IMicrofrontendsCache _microfrontendsCache;

    public GetMicrofrontendsOptionsRequest(
        IAuthenticatedUserContext authContext,
        IMicrofrontendsCache microfrontendsCache)
    {
        _authContext = authContext;
        _microfrontendsCache = microfrontendsCache;
    }

    public async Task<string> Handle(Query request, CancellationToken cancellationToken)
    {
        var userClaims = _authContext.GetUserClaimsNames();

        var microfrontendsConfig = await _microfrontendsCache.GetMicrofrontendsConfig(cancellationToken);

        var outputJson = FilterByUserClaims(microfrontendsConfig.JsonData, userClaims);

        return outputJson;
    }

    private static string FilterByUserClaims(string? json, IReadOnlyCollection<string> userClaims)
    {
        var root = JsonNode.Parse(json ?? string.Empty)?.AsObject();

        if (root?["microfrontends"] is not JsonArray microfrontends)
        {
            return json ?? string.Empty;
        }

        var filteredArray = new JsonArray();

        foreach (var item in microfrontends)
        {
            if (item?["name"] is null)
            {
                continue;
            }

            if (userClaims.Contains(MicrofrontendsClaimTypeNames.ManageMfe.Name))
            {
                filteredArray.Add(item.DeepClone());
                continue;
            }

            var claimsNode = item["claims"];
            if (claimsNode is null)
            {
                continue;
            }

            if (HasClaim(claimsNode, userClaims))
            {
                filteredArray.Add(item.DeepClone());
            }
        }

        root["microfrontends"] = filteredArray;

        var options = new JsonSerializerOptions { WriteIndented = true };
        return root.ToJsonString(options);
    }

    private static bool HasClaim(JsonNode claimsNode, IReadOnlyCollection<string> userClaims)
    {
        return claimsNode switch
        {
            JsonValue claimsValue when claimsValue.TryGetValue(out string? claim) => userClaims.Contains(claim),
            JsonArray claimsArray => claimsArray.Count == 0 || claimsArray
                .Select(c => c?.ToString())
                .Any(c => c is not null && userClaims.Contains(c)),
            _ => false
        };
    }

    internal sealed record Query : IQuery<string>;
}
