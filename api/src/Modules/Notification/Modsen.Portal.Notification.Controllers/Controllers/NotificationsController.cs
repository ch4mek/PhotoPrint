using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Notification;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Notification.Controllers.Contracts.Output;
using Modsen.Portal.Notification.DataAccess.Npgsql.Entities;
using Modsen.Portal.Notification.UseCases.Requests;

namespace Modsen.Portal.Notification.Controllers.Controllers;

[Area("[Notification] Notifications")]
internal class NotificationsController : BaseApiController
{
    private readonly ISender _sender;

    public NotificationsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Returns user's notifications
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="queryOptions">Parameters for searching, ordering and paging</param>
    /// <returns>Pageable list of notifications</returns>
    [HttpGet]
    [Route("~/api/users/{id:int}/notifications/search")]
    [ProducesResponseType(typeof(PageableListOfItems<OdpNotification>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PageableListOfItems<OdpNotification>>> GetOdpNotificationsByUserIdAsync(
        [FromRoute] int id, ODataQueryOptions<OdpNotification> queryOptions)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(new GetOdpNotificationsRequest.Query(id, queryOptions),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Returns boolean value
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="notificationTypes">Types of notifications</param>
    /// <returns>True if there is unread notification otherwise returns false</returns>
    [HttpGet]
    [Route("~/api/users/{id:int}/notifications/unread/preview")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsAnyUnreadNotificationExisting([FromRoute] int id,
        [FromQuery] List<NotificationTypes>? notificationTypes)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(new IsAnyUnreadNotificationExistingRequest.Query(id, notificationTypes),
            cancellationToken);

        return Ok(result.IsAnyUnreadNotificationExisting);
    }

    /// <summary>
    /// Reads user's notifications
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="notificationIds">Notification ids</param>
    /// <returns>Updated rows count</returns>
    [HttpPatch("read")]
    [ProducesResponseType(typeof(ReadNotificationsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ReadNotificationsOutput>> ReadNotificationAsync(
        [FromBody] List<int> notificationIds)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(new ReadNotificationsRequest.Command(notificationIds), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Deletes notification
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="id">Notification id</param>
    /// <returns>Deleted row id</returns>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeleteNotificationOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeleteNotificationOutput>> DeleteNotificationAsync([FromRoute] int id)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(new DeleteNotificationRequest.Command(id), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Deletes user's notifications
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="id">User id</param>
    /// <param name="notificationTypes">Types of notifications</param>
    /// <returns>Deleted rows count</returns>
    [HttpDelete]
    [Route("~/api/users/{id:int}/notifications")]
    [ProducesResponseType(typeof(DeleteNotificationsOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeleteNotificationsOutput>> DeleteNotificationsByUserIdAsync([FromRoute] int id,
        [FromQuery] List<NotificationTypes>? notificationTypes)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var result = await _sender.Send(new DeleteNotificationsByUserIdRequest.Command(id, notificationTypes),
            cancellationToken);

        return Ok(result);
    }
}
