using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Modsen.Portal.Core.Authentication.AgentKey;
using Modsen.Portal.TeamBoard.Contract;
using Modsen.Portal.TeamBoard.Contract.Contract.Output;

namespace Modsen.Portal.Utilities.Controllers.Agent
{
    [AllowAnonymous]
    [Area("[Agent] Agent")]
    [Route("api/agent-service")]
    [ServiceFilter(typeof(AgentApiKeyHeaderAttribute))]
    internal sealed class AgentController : Controller
    {
        private readonly ITeamBoardContract _teamBoardContract;

        public AgentController(ITeamBoardContract teamBoardContract)
        {
            _teamBoardContract = teamBoardContract;
        }

        /// <summary>
        /// Permanent info about user
        /// Access via api key
        /// </summary>
        /// <param name="input">User email</param>
        /// <returns></returns>
        [HttpPost("user-info/permanent")]
        public async Task<ActionResult<PermanentUserInfoOutput>> GetPermanentUserInfo(
            [FromBody] UserInfoInput input)
        {
            var cancellationToken = HttpContext.RequestAborted;
            var userInfo = await _teamBoardContract.GetPermanentUserInfo(input.Email, cancellationToken);

            return Ok(userInfo);
        }

        /// <summary>
        /// Impermanent info about user
        /// Access via api key
        /// </summary>
        /// <param name="input">User email</param>
        /// <returns></returns>
        [HttpPost("user-info/impermanent")]
        public async Task<ActionResult<ImpermanentUserInfoOutput>> GetImpermanentUserInfo(
            [FromBody] UserInfoInput input)
        {
            var cancellationToken = HttpContext.RequestAborted;
            var userInfo = await _teamBoardContract.GetImpermanentUserInfo(input.Email, cancellationToken);

            return Ok(userInfo);
        }
    }
}
