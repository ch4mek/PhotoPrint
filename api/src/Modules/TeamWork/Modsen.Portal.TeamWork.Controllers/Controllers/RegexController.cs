using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Authentication.RegexApiKey;
using Modsen.Portal.TeamWork.UseCases;

namespace Modsen.Portal.TeamWork.Controllers.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Area("[TeamWork] Team Work")]
    [ServiceFilter(typeof(RegexApiKeyHeaderAttribute))]
    public class RegexController : ControllerBase
    {
        private readonly IMediator _sender;

        public RegexController(IMediator sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Get users from json
        /// Allowed Roles : Auth
        /// </summary>
        /// <returns>A list of emails</returns>
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost("extract-users")]
        public async Task<List<string>> GetUsersFromJson([FromBody] ExtractUsersRequestModel? request)
        {
            var response = await _sender.Send(new GetDeserializedUsersUseCase.Query(request?.Text ?? string.Empty));
            return response.Users;
        }
    }

    public class ExtractUsersRequestModel
    {
        public string? Text { get; set; }
    }
}
