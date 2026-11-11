using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Requests.Messages.Core.CoreDto;
using Modsen.Portal.Identity.Requests.Messages.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers
{
    [Area("[Identity] DigitalHr")]
    [ApiController]
    [Route("api/[controller]")]
    internal class DigitalHrController : BaseApiController
    {

        private readonly ISender _sender;

        public DigitalHrController(ISender sender)
        {
            _sender = sender;
        }

        /// <summary>
        /// Analyze question
        /// Allowed roles: Anonymous
        /// </summary>
        /// <param name="input">Request</param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpPost("analyze-question")]
        public async Task<IActionResult> AnalyzeQuestion([FromBody] DigitalHrQuestionInput input)
        {
            var result = await _sender.Send(new CreateChatMessageRequest.Command(input.Question),
                HttpContext.RequestAborted);

            return Ok(result);
        }

        /// <summary>
        /// Analyze question
        /// Allowed roles: Anonymous
        /// </summary>
        /// <param name="queryOptions">Request</param>
        /// <returns></returns>
        /// <exception cref="AppException"></exception>
        [HttpPost("chat-history")]
        public async Task<IActionResult> ChatHistory([FromBody] ODataQueryOptions<MessageCoreDto> queryOptions)
        {
            var result = await _sender.Send(new GetChatHistoryRequest.Command(queryOptions), HttpContext.RequestAborted);
            return Ok(result);
        }
    }
}
