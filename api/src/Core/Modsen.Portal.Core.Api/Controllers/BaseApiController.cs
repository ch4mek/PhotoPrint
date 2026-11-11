using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Modsen.Portal.Core.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public abstract class BaseApiController : ControllerBase;
