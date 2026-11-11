using System.Net;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Utilities.Controllers.Cache;

namespace Modsen.Portal.Utilities.Controllers.Configuration;

[Area("[Shared] Configuration")]
public class ConfigurationController : BaseApiController
{
    private readonly IConfigurationCache _configurationCache;

    public ConfigurationController(IConfigurationCache configurationCache)
    {
        _configurationCache = configurationCache;
    }

    /// <summary>
    /// Get all enums info ( used by frontend )
    /// Allowed role: Authorized
    /// </summary>
    /// <returns>Dictionary of EnumModels Lists</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ConfigurationOutput), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<ConfigurationOutput>> Get()
    {
        var configurationOutput = await _configurationCache.GetConfiguration();
        return Ok(configurationOutput);
    }
}
