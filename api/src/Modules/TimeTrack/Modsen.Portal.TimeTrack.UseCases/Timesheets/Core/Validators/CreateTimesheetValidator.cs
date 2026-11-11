using Modsen.Portal.Core;
using Modsen.Portal.Core.Context;
using Modsen.Portal.Core.Exceptions;
using Modsen.Portal.Core.Identity;
using Modsen.Portal.TimeTrack.DataAccess.Abstractions.Users;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.BusinessRules;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.CoreDto;
using Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.DbQueries;

namespace Modsen.Portal.TimeTrack.UseCases.Timesheets.Core.Validators;

internal interface ICreateTimesheetValidator
{
    Task ValidateAsync(CreateTimesheetDto createDto, CancellationToken cancellationToken);
}

internal class CreateTimesheetValidator : ICreateTimesheetValidator
{
    private readonly IHasUserActiveAssignmentForDateBusinessRule _hasUserActiveAssignmentForDateBusinessRule;
    private readonly IGetProjectBillableStatusIfExistsDbQuery _getProjectBillableStatusIfExistsDbQuery;
    private readonly ICanModifyTimesheetForDateBusinessRule _canModifyTimesheetForDateBusinessRule;
    private readonly ICanTrackBillableHoursBusinessRule _canTrackBillableHoursBusinessRule;
    private readonly IAuthenticatedUserContext _authenticatedUserContext;
    private readonly IGetUserDetailsDbQuery _getUserDetailsDbQuery;
    private readonly IGetProjectActiveStatusDbQuery _getProjectActiveStatusDbQuery;
    private readonly IGetUserActiveStatusDbQuery _getUserActiveStatusDbQuery;

    public CreateTimesheetValidator(IAuthenticatedUserContext authenticatedUserContext,
        IGetUserDetailsDbQuery getUserDetailsDbQuery,
        ICanTrackBillableHoursBusinessRule canTrackBillableHoursBusinessRule,
        IGetProjectBillableStatusIfExistsDbQuery getProjectBillableStatusIfExistsDbQuery,
        ICanModifyTimesheetForDateBusinessRule canModifyTimesheetForDateBusinessRule,
        IHasUserActiveAssignmentForDateBusinessRule hasUserActiveAssignmentForDateBusinessRule,
        IGetProjectActiveStatusDbQuery getProjectActiveStatusDbQuery,
        IGetUserActiveStatusDbQuery getUserActiveStatusDbQuery)
    {
        _authenticatedUserContext = authenticatedUserContext;
        _getUserDetailsDbQuery = getUserDetailsDbQuery;
        _canTrackBillableHoursBusinessRule = canTrackBillableHoursBusinessRule;
        _getProjectBillableStatusIfExistsDbQuery = getProjectBillableStatusIfExistsDbQuery;
        _canModifyTimesheetForDateBusinessRule = canModifyTimesheetForDateBusinessRule;
        _hasUserActiveAssignmentForDateBusinessRule = hasUserActiveAssignmentForDateBusinessRule;
        _getProjectActiveStatusDbQuery = getProjectActiveStatusDbQuery;
        _getUserActiveStatusDbQuery = getUserActiveStatusDbQuery;
    }

    public async Task ValidateAsync(CreateTimesheetDto createDto, CancellationToken cancellationToken)
    {
        var projectBillableStatus = await _getProjectBillableStatusIfExistsDbQuery.ExecuteAsync(
            createDto.ProjectId,
            cancellationToken);

        if (!projectBillableStatus.HasValue)
        {
            throw new EntityNotFoundException($"Project with ID {createDto.ProjectId} does not exists");
        }

        var userActiveStatus = await _getUserActiveStatusDbQuery.ExecuteAsync(
            createDto.UserId,
            cancellationToken);

        if (!userActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetOfInactiveUser);
        }

        var projectActiveStatus = await _getProjectActiveStatusDbQuery.ExecuteAsync(
            createDto.ProjectId,
            cancellationToken);

        if (!projectActiveStatus)
        {
            throw new ActionNotAllowedException(PortalMessages.CannotManageTimesheetAtInactiveProject);
        }

        if (!_authenticatedUserContext.IsManager)
        {
            if (createDto.UserId != _authenticatedUserContext.UserId && !createDto.AllowAutomatic)
            {
                throw new ActionNotAllowedException(PortalMessages.UserCannotCreateTimesheetForOtherUsers);
            }

            var canTrackBillable = await _canTrackBillableHoursBusinessRule.IsCorrespondsToBusiness(
                createDto.UserId,
                createDto.ProjectId,
                createDto.BillableTrackedInMinutes,
                cancellationToken);

            if (!canTrackBillable)
            {
                throw new InvalidDataAppException(PortalMessages.CannotTrackBillableHoursWithoutPermission);
            }
        }
        else
        {
            await EnsureManagerCanCreateTimesheetAsync(createDto, cancellationToken);
        }

        var hasActiveAssignment = await _hasUserActiveAssignmentForDateBusinessRule.IsCorrespondsToBusiness(
            createDto.ProjectId,
            createDto.UserId,
            createDto.ReportForDate,
            cancellationToken);

        if (!hasActiveAssignment)
        {
            throw new InvalidDataAppException(PortalMessages.NoActiveAssignmentFoundForDate);
        }

        var canCreateTimesheetForDate = await _canModifyTimesheetForDateBusinessRule.IsCorrespondsToBusiness(
            createDto.UserId,
            createDto.ReportForDate,
            cancellationToken);

        if (!canCreateTimesheetForDate)
        {
            throw new ActionNotAllowedException(PortalMessages.OnlyManagersCanModifyApprovedAndSubmittedTimesheets);
        }
    }

    private async Task EnsureManagerCanCreateTimesheetAsync(CreateTimesheetDto createDto,
        CancellationToken cancellationToken)
    {
        var currentRoles = _authenticatedUserContext.GetRoles();
        var currentDepartmentId = _authenticatedUserContext.DepartmentId;
        var currentResourceGroupId = _authenticatedUserContext.ResourceGroupId;

        var userDetails = await _getUserDetailsDbQuery.Execute(createDto.UserId, cancellationToken)
            ?? throw new InvalidDataAppException(PortalMessages.UserNotFound);

        if (userDetails.Role.IsCeo() || userDetails.Role.IsCto())
        {
            throw new InvalidDataAppException($"{userDetails.Role} can't have timesheets");
        }

        if (currentRoles.IsHeadOfDepartment() &&
            userDetails.DepartmentId != currentDepartmentId)
        {
            throw new ActionNotAllowedException(PortalMessages.HeadCannotCreateTimesheetForUserFromOtherDepartment);
        }

        if (currentRoles.IsGroupManager() &&
            userDetails.ResourceGroupId != currentResourceGroupId &&
            userDetails.UserId != _authenticatedUserContext.UserId)
        {
            throw new ActionNotAllowedException(PortalMessages.GroupManagerCannotCreateTimesheetForUserFromOtherGroup);
        }
    }
}
