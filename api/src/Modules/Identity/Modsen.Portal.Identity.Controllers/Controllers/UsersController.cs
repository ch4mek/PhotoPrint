using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Modsen.Portal.Core.Api.Controllers;
using Modsen.Portal.Core.Api.Response;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.PolicyNames;
using Modsen.Portal.Core.Services.Cqrs;
using Modsen.Portal.Core.Utils;
using Modsen.Portal.Identity.Controllers.Contracts.Input;
using Modsen.Portal.Identity.Controllers.Contracts.Output;
using Modsen.Portal.Identity.Requests.Core;
using Modsen.Portal.Identity.Requests.Dto;
using Modsen.Portal.Identity.Requests.Users.Requests;

namespace Modsen.Portal.Identity.Controllers.Controllers;

[Area("[Identity] Users")]
internal class UsersController : BaseApiController
{
    private readonly ISender _sender;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;

    public UsersController(
        ISender sender,
        IAuthenticatedUserContext authenticatedUserContext)
    {
        _sender = sender;
        _authenticatedUserContext = authenticatedUserContext;
    }

    /// <summary>
    /// Get user by id
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// </summary>
    /// <param name="id">ID of user</param>
    /// <returns>User model</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName)]
    public async Task<ActionResult<UserOutput>> GetByIdAsync(int id)
    {
        var user = await _sender.Send(new GetUserByIdRequest.Query(
                id,
                _authenticatedUserContext.DepartmentId,
                _authenticatedUserContext.ResourceGroupId,
                _authenticatedUserContext.GetRoles(),
                _authenticatedUserContext.GetUserClaimsNames()),
                HttpContext.RequestAborted);

        return UserOutput.Of(user);
    }

    /// <summary>
    /// Get users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// Allowed Claims : ManageHRModule
    /// </summary>
    /// <param name="queryOptions">options for searching, ordering and paging users</param>
    /// <returns>List of users</returns>
    [ProducesResponseType(typeof(PageableListOfItems<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("search")]
    [Authorize(Policy = ChiefHeadGmSysAdminOnlyPolicyName.Name)]
    public async Task<ActionResult<PageableListOfItems<UserOutput>>> GetUsersAsync(
        ODataQueryOptions<UserCoreDto> queryOptions)
    {
        var pageableUsers = await _sender.Send(
            new GetUsersRequest.Query(queryOptions),
                HttpContext.RequestAborted);

        return Ok(pageableUsers.Items.Select(UserOutput.Of)
            .ToPageable()
            .WithPaging(pageableUsers.Page, pageableUsers.PageSize)
            .WithTotal(pageableUsers.Total));
    }


    /// <summary>
    /// Get users tab
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// Allowed Claims : ManageHRModule
    /// </summary>
    /// <param name="queryOptions">options for searching, ordering and paging users</param>
    /// <returns>List of users</returns>
    [ProducesResponseType(typeof(PageableListOfItems<UserOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet("tab/search")]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName)]
    public async Task<ActionResult<PageableListOfItems<UserOutput>>> GetUsersTabAsync(ODataQueryOptions<UserCoreDto> queryOptions)
    {
        var pageableUsers = await _sender.Send(
            new GetUsersTabRequest.Query(queryOptions),
            HttpContext.RequestAborted);

        return Ok(pageableUsers.Items.Select(UserOutput.Of)
            .ToPageable()
            .WithPaging(pageableUsers.Page, pageableUsers.PageSize)
            .WithTotal(pageableUsers.Total));
    }

    /// <summary>
    /// Get current user
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>User model</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Route("current")]
    [Authorize]
    public async Task<ActionResult<UserOutput>> GetCurrentUserAsync()
    {
        var userDto = await _sender.Send(new GetCurrentUserRequest.Query(), HttpContext.RequestAborted);

        return UserOutput.Of(userDto);
    }

    /// <summary>
    /// Lookup users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// </summary>
    /// <param name="keyWord">string of keyword</param>
    /// <param name="isActive">Indicates whether to retrieve active/inactive or all users</param>
    /// <param name="roles">Roles</param>
    /// <returns>List of users</returns>
    [ProducesResponseType(typeof(List<UserLookupOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Route("lookup")]
    [Authorize(Policy = ChiefHeadGmSysAdminOnlyPolicyName.Name)]
    public async Task<ActionResult<List<UserLookupOutput>>> GetUserLookupAsync(
        [FromQuery] string? keyWord,
        [FromQuery] bool? isActive,
        [FromQuery] List<string> roles)
    {
        var query = new GetUserLookupRequest.Query(keyWord, isActive, roles);

        var users = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(users.Select(UserLookupOutput.Of));
    }

    /// <summary>
    /// Lookup users
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// Allowed Claims : ManageHRModule
    /// </summary>
    /// <param name="keyWord">string of keyword</param>
    /// <param name="isActive">Indicates whether to retrieve active/inactive or all users</param>
    /// <param name="roles">Roles</param>
    /// <returns>List of users</returns>
    [ProducesResponseType(typeof(List<UserLookupWithRussianNamesShortOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Route("tab/lookup")]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRAccountantCoordinatorPolicyName)]
    public async Task<ActionResult<List<UserLookupWithRussianNamesShortOutput>>> GetUserTabLookupAsync(
        [FromQuery] string? keyWord,
        [FromQuery] bool? isActive,
        [FromQuery] List<string> roles)
    {
        var query = new GetUserTabLookupRequest.Query(keyWord, isActive, roles);

        var users = await _sender.Send(query, HttpContext.RequestAborted);

        return Ok(users.Select(UserLookupWithRussianNamesShortOutput.Of));
    }

    /// <summary>
    /// Get signature for current user
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>Signature model</returns>
    [ProducesResponseType(typeof(UserSignatureOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpGet]
    [Route("current/signature")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserSignatureOutput>> GetCurrentUserSignatureAsync()
    {
        var userSignatureDto = await _sender.Send(new GetCurrentUserSignatureRequest.Query(), HttpContext.RequestAborted);

        return Ok(UserSignatureOutput.Of(userSignatureDto));
    }

    /// <summary>
    /// Update current user signature
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="updateSignatureModel">model of new signature</param>
    /// <returns>Updated signature</returns>
    [ProducesResponseType(typeof(UserSignatureOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Route("current/signature")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserSignatureOutput>> UpdateCurrentUserSignatureAsync(UpdateUserSignatureInput updateSignatureModel)
    {
        var updateUserSignatureCommand = new UpdateUserSignatureRequest.Command(
            updateSignatureModel.FullName,
            updateSignatureModel.Position,
            updateSignatureModel.Phone
            );

        var userSignatureDto = await _sender.Send(updateUserSignatureCommand, HttpContext.RequestAborted);

        return Ok(UserSignatureOutput.Of(userSignatureDto));
    }

    /// <summary>
    /// Clear signature of current user
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <returns>Updated signature</returns>
    [ProducesResponseType(typeof(UserSignatureOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpDelete]
    [Route("current/signature")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<UserSignatureOutput>> ResetCurrentUserSignatureAsync()
    {
        var userSignatureDto = await _sender.Send(new ResetUserSignatureRequest.Command(), HttpContext.RequestAborted);

        return Ok(UserSignatureOutput.Of(userSignatureDto));
    }

    /// <summary>
    /// Update current user
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="model">updated model of user</param>
    /// <returns>Updated user model</returns>
    [ProducesResponseType(typeof(CurrentUserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Route("current")]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<CurrentUserOutput>> UpdateCurrentUserAsync([FromBody] UpdateCurrentUserInput model)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var user = await _sender.Send(new UpdateCurrentUserRequest.Command(
            model.TimeZone,
            model.AvatarColor,
            model.AvatarFileId,
            model.DepartmentId,
            model.Location), cancellationToken);

        var currentUserOutput = UserOutput.Of(user);

        return Ok(currentUserOutput);
    }

    /// <summary>
    /// Create a new user
    /// Allowed Roles : CEO, CTO, HeadOfDepartment
    /// </summary>
    /// <param name="model">model of the new user</param>
    /// <returns>Created user's model</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPost]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadHrPolicyName)]
    public async Task<ActionResult<UserOutput>> CreateUserAsync([FromBody] CreateUserInput model)
    {
        var createdUser = await _sender.Send(new CreateUserRequest.Command(
                model.FirstName,
                model.LastName,
                model.FirstNameRu!,
                model.LastNameRu!,
                model.PatronymicRu!,
                model.Color,
                model.CountryId,
                model.CurrentLocation,
                model.RegisteredLocationId,
                model.Email,
                model.PersonalEmail,
                model.PersonalEmailSecond,
                model.PersonalPhone,
                model.Telegram,
                model.LinkedIn,
                model.Position,
                model.Location,
                model.DepartmentId,
                model.ResourceGroupId,
                model.MilitaryServiceStatus,
                model.IsGraduateJobPlacement,
                model.ContractDateFrom,
                model.ContractDateTo,
                model.HireDate,
                model.DateOfBirth),
            HttpContext.RequestAborted);

        return Ok(UserOutput.Of(createdUser));
    }

    /// <summary>
    /// Update a user
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator, GroupManager
    /// </summary>
    /// <param name="model">new model of the user</param>
    /// <returns>Updated user's model</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut]
    [Authorize(Policy = ChiefHeadGmSysAdminOnlyPolicyName.Name)]
    public async Task<ActionResult<UserOutput>> UpdateUserAsync([FromBody] UpdateUserInput model)
    {
        var cancellationToken = HttpContext.RequestAborted;

        var userDto = await _sender.Send(new UpdateUserRequest.Command(
                model.Id,
                model.FirstName,
                model.LastName,
                model.FirstNameRu,
                model.LastNameRu,
                model.Color,
                model.CountryId,
                model.RegisteredLocationId,
                model.CurrentLocation,
                model.Email,
                model.Position,
                model.Location,
                model.DepartmentId,
                model.ResourceGroupId,
                _authenticatedUserContext.GetRoles(),
                _authenticatedUserContext.DepartmentId,
                model.HireDate),
                cancellationToken);

        return UserOutput.Of(userDto);
    }

    /// <summary>
    /// Update user's location
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="input">Data to update user's location</param>
    /// <returns>Updated location</returns>
    [HttpPatch("location")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = DefaultPolicyName.Name)]
    public async Task<ActionResult<string>> UpdateUserLocationAsync(
        [FromBody] UpdateUserLocationInput input)
    {
        var command = new UpdateUserLocationRequest.Command(
            input.UserId,
            input.Location);

        var result = await _sender.Send(command);

        return result;
    }

    /// <summary>
    /// Update user's registered location
    /// Allowed Roles : Authenticated user
    /// </summary>
    /// <param name="userId">User for update registered location</param>
    /// <param name="input">Data to update user's registered location</param>
    /// <returns>Updated registered location</returns>
    [HttpPatch("{userId:int}/registered-location")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<ActionResult<RegisteredLocationOutput>> UpdateUserRegisteredLocationAsync(
        [FromRoute] int userId,
        [FromBody] UpdateUserRegisteredLocationInput input)
    {
        var command = new UpdateUserRegisteredLocationRequest.Command(
            userId,
            input.RegisteredLocationId,
            input.CurrentLocation,
            input.Location);

        var registeredLocationDto = await _sender.Send(command);

        return Ok(RegisteredLocationOutput.Of(registeredLocationDto));
    }

    /// <summary>
    /// Update user's hire date
    /// Allowed Roles : CEO, CTO, HeadOfDepartment GroupManager
    /// </summary>
    /// <param name="userId">User id for update hire date</param>
    /// <param name="input">Data to update user's hire date</param>
    /// <returns>Updated hire date</returns>
    [HttpPatch("{userId:int}/hireDate")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmHRPolicyName)]
    public async Task<IActionResult> UpdateUserHireDateAsync(
        [FromRoute] int userId,
        [FromBody] UpdateUserHireDateInput input)
    {
        var command = new UpdateUserHireDateRequest.Command(
            userId,
            input.HireDate);

        var result = await _sender.Send(command);

        return Ok(result);
    }

    /// <summary>
    /// Update user status (active/inactive)
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, SystemAdministrator
    /// </summary>
    /// <param name="id">User ID</param>
    /// <param name="input">Input with isActive property</param>
    /// <returns>Updated User</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPut("{id:int}/IsActive")]
    [Authorize(Policy = PortalPolicyNames.ChiefHeadGmSysAdminHRPolicyName)]
    public async Task<ActionResult<UserOutput>> UpdateUserStatusAsync(
        [FromRoute] int id,
        [FromBody] UserStatusInput input)
    {
        var cancellationToken = HttpContext.RequestAborted;

        ICommand<UserDto> command = input.IsActive
            ? new ActivateUserRequest.Command(id)
            : new DeactivateUserRequest.Command(id);

        var user = await _sender.Send(command, cancellationToken);

        return Ok(user);
    }

    /// <summary>
    /// Update user avatar color
    /// Allowed Roles : CEO, CTO, HeadOfDepartment, GroupManager
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="color">Hex color value</param>
    /// <returns>Updated User</returns>
    [ProducesResponseType(typeof(UserOutput), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ExceptionResponse), StatusCodes.Status500InternalServerError)]
    [HttpPatch("~/api/users/{userId:int}/avatar-color/{color}")]
    [Authorize(Policy = ChiefHeadAndGmOnlyPolicyName.Name)]
    public async Task<ActionResult<UserOutput>> UpdateUserColor(
        [FromRoute] int userId,
        [FromRoute] string color)
    {
        var cancellationToken = HttpContext.RequestAborted;
        var command = new UpdateUserAvatarColorRequest.Command(userId, color);
        var user = await _sender.Send(command, cancellationToken);
        return Ok(user);
    }
}
