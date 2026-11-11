using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Requests.ColorPicker.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Color Picker")]
[Route("api/color-picker")]
public class ColorPickerController : BaseApiController
{
    private readonly ISender _sender;

    public ColorPickerController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get user colors from color picker
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>list of colors</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<string>>> GetAll()
    {
        var colors = await _sender.Send(new GetUserColorsFromColorPickerRequest.Query(), HttpContext.RequestAborted);

        return Ok(colors);
    }

    /// <summary>
    /// Add color to color picker
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>New color</returns>
    [HttpPost]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<string>> AddColor([FromBody] ColorInput colorInput)
    {
        ArgumentNullException.ThrowIfNull(colorInput);
        var color = await _sender.Send(new AddColorToColorPickerRequest.Command(colorInput.Color), HttpContext.RequestAborted);

        return Ok(color);
    }

    /// <summary>
    /// Delete color from color picker
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="color">Color</param>
    /// <returns>Is color deleted</returns>
    [HttpDelete("{color}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<bool>> DeleteColor(string color)
    {
        var isDeleted = await _sender.Send(new DeleteColorFromColorPickerRequest.Command(color), HttpContext.RequestAborted);

        return Ok(isDeleted);
    }
}
