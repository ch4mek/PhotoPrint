using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Input;
using Modsen.Portal.TimeTrack.Controllers.Contracts.Output;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.OData;
using Modsen.Portal.TimeTrack.UseCases.Clients.Requests;

namespace Modsen.Portal.TimeTrack.Controllers.Controllers;

[Area("[TimeTrack] Clients")]
internal class ClientsController : BaseApiController
{
    private readonly ISender _sender;

    public ClientsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Get Client by id
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <returns>Client with specified id</returns>
    [ProducesResponseType(typeof(ClientOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<ClientOutput>> GetByIdAsync([FromRoute] int id)
    {
        var client = await _sender.Send(new GetClientByIdRequest.Query(id), HttpContext.RequestAborted);

        return Ok(ClientOutput.Of(client));
    }

    /// <summary>
    /// Get Clients with odata pagination, filtering and sorting
    /// Allowed Roles: CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="queryOptions">Odata options</param>
    /// <returns>Collections of clients, it's count and total count of items by provided filtering</returns>
    [ProducesResponseType(typeof(ClientOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("search")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<ClientOutput>>> GetClientsAsync(
        ODataQueryOptions<OdpClient> queryOptions)
    {
        var clientsDtoPage = await _sender.Send(new GetClientsRequest.Query(queryOptions), HttpContext.RequestAborted);

        var clients = clientsDtoPage.Items.Select(ClientOutput.Of)
            .ToPageable()
            .WithPaging(clientsDtoPage.Page, clientsDtoPage.PageSize)
            .WithTotal(clientsDtoPage.Total);

        return Ok(clients);
    }

    /// <summary>
    /// Create Client
    /// Allowed Roles: CEO, CTO
    /// </summary>
    /// <param name="input">Input with client properties</param>
    /// <returns>Created Client</returns>
    [ProducesResponseType(typeof(CreateClientOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [HttpPost]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<CreateClientOutput>> CreateClientAsync([FromBody] CreateClientInput input)
    {
        var command = new CreateClientRequest.Command(
            input.Name,
            input.LegalEntityName,
            input.Comments,
            input.Color,
            input.RegisteredLocation,
            input.SalesManagerId);

        var createdClient = await _sender.Send(
            command,
            HttpContext.RequestAborted);

        return Ok(CreateClientOutput.Of(createdClient));
    }

    /// <summary>
    /// Update existing client
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="input">Input with updated client properties</param>
    /// <returns>Updated Client</returns>
    [ProducesResponseType(typeof(ClientOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpPut]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<ClientOutput>> UpdateClientAsync([FromBody] UpdateClientInput input)
    {
        var request = new UpdateClientRequest.Command(
            input.Id,
            input.Name,
            input.LegalEntityName,
            input.Color,
            input.Comments,
            RegisteredLocation: input.RegisteredLocation,
            SalesManagerId: input.SalesManagerId);

        var updatedClient = await _sender.Send(
            request,
            HttpContext.RequestAborted);

        return Ok(ClientOutput.Of(updatedClient));
    }

    /// <summary>
    /// Remove client
    /// Allowed Roles : CEO, CTO
    /// </summary>
    /// <param name="id">Client ID</param>
    /// <param name="input">Input with isActive property</param>
    /// <returns>An indicator whether operation succeeded or do not.</returns>
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [HttpPatch("{id:int}/IsActive")]
    [Authorize(Policy = ChiefOnlyPolicyName.Name)]
    public async Task<ActionResult<ClientStatusOutput>> UpdateClientStatusAsync(
        [FromRoute] int id,
        [FromBody] ClientStatusInput input)
    {
        var clientStatus = await _sender.Send(
            new UpdateClientStatusRequest.Command(id, input.IsActive),
            HttpContext.RequestAborted);

        return Ok(ClientStatusOutput.Of(clientStatus));
    }

    /// <summary>
    /// Get projects of specific client
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="clientId">Client ID</param>
    /// <param name="onlyManagedProjects">Flag for only managed projects</param>
    /// <returns>List of client projects</returns>
    [HttpGet("{clientId:int}/projects")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    [ProducesResponseType(typeof(List<ClientProjectOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ClientProjectOutput>>> GetProjectsByClientIdAsync(
        [FromRoute] int clientId,
        [FromQuery] bool? onlyManagedProjects)
    {
        var projects = await _sender.Send(
            new GetClientProjectsRequest.Query(clientId, onlyManagedProjects),
            HttpContext.RequestAborted);

        return Ok(projects);
    }

    /// <summary>
    /// Get Clients by keyword
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="keyWord">substring that should client name contains</param>
    /// <param name="isActive">flag,indicating should we return deactivated or active clients. If not provided - return all clients</param>
    /// <param name="onlyManagedClients">Flag for only managed projects</param>
    /// <returns>List of clients</returns>
    [ProducesResponseType(typeof(List<ClientLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [HttpGet("lookup")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<List<ClientLookupOutput>>> ClientsLookupAsync(
        [FromQuery] string? keyWord,
        [FromQuery] bool? isActive,
        [FromQuery] bool? onlyManagedClients)
    {
        var clients = await _sender.Send(
            new GetClientsLookupRequest.Query(keyWord, isActive, onlyManagedClients),
            HttpContext.RequestAborted);

        return Ok(
            clients.Select(x => new ClientLookupOutput
            {
                Id = x.Id,
                Name = x.Name
            }).ToList()
        );
    }
}
