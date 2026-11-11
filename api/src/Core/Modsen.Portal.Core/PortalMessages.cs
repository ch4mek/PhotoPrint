using System.Text;

namespace Modsen.Portal.Core;

public static class PortalMessages
{
    public const string LoginFromAnotherIp = "You've been logged out. Another user (ip address) has signed into your account.";

    public const string BookingIsNoLongerAvailable = "Your booking is no longer available. We apologize for any inconvenience caused";
    public const string BookingHasBeenCancelled = "Your booking has been canceled. We apologize for any inconvenience caused";

    public const string BookingExistsForWorkplace = "Booking for workplace already exists!";
    public const string AttemptToCreateSeveralBookings = "You already have a booking for selected period. Please cancel the booking or choose another date(s)";
    public const string AttemptToCreateSeveralBookingsForGuest = "Guest with this name already has a booking for selected period. Please cancel the booking or choose another date(s)";
    public const string DifferentDepartments = "Different departments";
    public const string DifferentResourceGroups = "Different resource groups";
    public const string NotExistingUser = "There is no such user!";
    public const string NotExistingWorkspace = "There is no such workplace!";
    public const string NotHavePermissionToCancelBooking = "You does not have permission to cancel this booking";
    public const string CannotBookMeetingRoomForAnotherUser = "You cannot book meeting room for another user";
    public const string HeadCantBookMeetingRoomForUserFromOtherDepartment = "You cannot book meeting room for user from another department";
    public const string GroupManagerCantBookMeetingRoomForUserFromOtherGroup = "You cannot book meeting room for user from another group";
    public const string CannotDeleteMeetingRoomEntryForAnotherUser = "You cannot delete meeting room entry for another user";
    public const string HeadCannotDeleteMeetingRoomEntryForUserFromOtherDepartment = "You cannot delete meeting room entry for user from another department";
    public const string GroupManagerCannotDeleteMeetingRoomEntryForUserFromOtherGroup = "You cannot delete meeting room entry for user from another group";
    public const string CannotCheckCollisionsForAnotherUser = "You cannot check meeting room's collisions for another user";
    public const string HeadCannotCheckCollisionsForUserFromOtherDepartment = "You cannot check meeting room's collisions for user from another department";
    public const string GroupManagerCannotCheckCollisionsForUserFromOtherGroup = "You cannot check meeting room's collisions for user from another group";
    public const string ConflictingReservationTime = "The start time cannot be earlier than the current time.";
    public const string CannotBookInactiveMeetingRoom = "You cannot book inactive meeting room";
    public const string NotExistingMeetingRoomEntry = "Cannot find meeting room entry";
    public const string ReservationConflicts = "You cannot book meeting room for selected period. There are reservation conflicts.";
    public const string CannotGetCollisionsForInactiveMeetingRoom = "You cannot get collisions for inactive meeting room";
    public const string NotManagerCannotCheckCollisionsOfOtherUsers = "You are not a manager so you can't check collisions of other users";

    public const string UnsupportedFileFormat = "Unsupported file format";
    public const string FileNameIsMissing = "File name is missing";
    public const string UnsupportedRoleTypes = "Unsupported RoleTypes";
    public const string ApproverDoesNotExist = "User with provided approver ID does not exist";
    public const string ApproverDoesNotHavePermission = "Provided user does not have permission to approve";
    public const string EquipmentNotFound = "Equipment was not found";
    public const string UserNotFound = "User not found";
    public const string EquipmentAssignNotFound = "Equipment assign was not found";
    public const string DepartmentNotFound = "Department was not found";
    public const string GroupNotFound = "Group was not found";
    public const string EducationNotFound = "User education was not found";
    public const string EquipmentAlreadyAssigned = "This equipment has already been assigned";
    public const string HeadCannotSeeOtherDepartmentEquipment = "The head of the department cannot see the equipment assigned to the user from another department";
    public const string GroupManagerCannotSeeOtherResourceGroupEquipment = "The group manager cannot see the equipment assigned to the user from another resource group";
    public const string YouDoNotHavePermission = "You don't have permission";
    public const string ReturnDateLowerThanIssueDate = "The return date must be equal or later than the issue date";
    public const string ReturnDateWasSetForTheFuture = "You cannot set return date for the future";
    public const string EquipmentTypeNotFound = "Equipment type was not found";
    public const string ExistingInventoryNumber = "An equipment with the same inventory number already exists";
    public const string EquipmentReportAssemblyDateNotFound = "Equipment report doesn't have creation date property";
    public const string EquipmentReportSerialNumberNotFound = "Equipment report doesn't have hardware serial number property";
    public const string EquipmentReportNotFound = "Equipment report hasn't been found";
    public const string EquipmentReportRelevancePeriodsNotFound = "Equipment report relevance periods haven't been found";
    public const string EquipmentWasNeverAssigned = "This equipment was never assigned";
    public const string SeveralNullDatesForEquipment = "There are several null return dates for chosen equipment";
    public const string ExistingReturnDateForLastAssignment = "There is existing return date for the last assignment!";
    public const string ExistingSerialNumber = "An equipment with the same serial number already exists";
    public const string MandatoryRepairPlace = "Repair place is mandatory when status is set as Renovation";
    public const string BrokenStatusConflict = "Broken status Written off cannot be set when equipment is assigned";
    public const string EquipmentCannotBeDeleted = "Only assigns without return date and with Issue status can be deleted";
    public const string EquipmentDoesNotHaveAssigns = "Selected equipment does not contain assigns";

    public const string PermissionAssignmentNotFound = "Permission assignment was not found";
    public const string HeadOfDepartmentDoesNotHaveAccessToPermissionsOfUserFromAnotherDepartment =
        "Head of Department does not have access to permissions of user from another department";

    public const string CannotGrantPermissionsToNonEmployeeUser = "Cannot grant permissions to non-employee user";

    public const string CannotGrantPermissionsToUserWithoutDepartment = "Cannot grant permissions to user without specified department";

    public const string CannotParseValue = "Cannot parse value";
    public const string ProjectsNotExist = "Some of the projects provided do not exist";
    public const string SysAdminCanActivateOnlyEmployeeOrInternRole = "System administrator can activate only user with employee or intern role";
    public const string RedirectToFieldNotProperlyFormed = "RedirectTo field has not been properly formed";
    public static readonly CompositeFormat UserWithEmailAlreadyExists = CompositeFormat.Parse("Email - {0} is already used");
    public const string UserStillHasAssignedEquipment = "User does still have assigned equipment or approves equipment with active assignment";
    public const string UserStillHasScheduledDesk = "User does still have scheduled desk or meeting room bookings";
    public const string UserCannotDeactivateHimself = "User cannot deactivate himself";
    public const string HeadCanDeactivateUsersOnlyFromHisDepartment = "Head of department can deactivate users only from his/her department";
    public const string HeadCanActivateUsersOnlyFromHisDepartment = "Head of Department can activate users only from his/her department";
    public const string GroupManagerCanActivateUsersOnlyFromHisGroup = "Group manager can activate users only from his/her resource group";
    public const string UserIsApproverOrSubstitute = "The user is an approver or substitute of requests. Please remove them from the approvers or substitute to continue";
    public const string YouCannotActivateUser = "You cannot activate user";
    public const string UserIsStillAssignedToProjects = "User is still assigned to projects";
    public const string UserIsInGroup = "User is in the group";
    public const string UserIsProjectManager = "User is project manager";
    public const string FileIsNotFound = "File is not found";
    public const string YouCannotChangeYourDepartment = "You cannot change your department";
    public const string GmAndSysAdminCannotUpdateResourceGroup = "Group manager and system administrator cannot update resource group";
    public const string UnsupportedResourceType = "Unsupported resource type";
    public const string YouCannotCreateCeo = "You cannot create user with CEO role";
    public const string ModelIsNull = "Model is null";
    public const string CtoAndCeoCannotHaveDepartment = "CTO and CEO cannot have department";
    public const string SysAdminHeadGmEmployeeAndInternMustHaveDepartment = "SystemAdministrator, HeadOfDepartment, GroupManager, Employee and Intern must have department";
    public const string UserDoesNotHaveAccessToSomeProjects = "User does not have access to some of the projects provided";
    public const string UnsupportedUserUpdate = "Unsupported user update";
    public const string YouCannotChangeUsersRole = "You can't change User's role";
    public const string YouCannotChangeUsersDepartment = "You can't change User's department";
    public const string YouCannotChangeUsersLocation = "You can't change User's location";
    public const string YouCannotChangeUsersResourceGroup = "Group manager and system administrator cannot update resource group";
    public const string YouCannotChangeRoleToCeo = "You cannot change role to CEO";
    public const string YouCannotEditUsersWithCeoRole = "You cannot edit users with CEO role";
    public const string YouCanEditEmployeesOnlyFromYourDepartment = "You can edit Employees only from your department";
    public const string YouCanEditEmployeesOnlyFromYourGroup = "You can edit Employees only from your resource group";
    public const string HeadCannotAssignUserToGroupFromAnotherDepartment = "Head of department cannot assign user to resource group from another department";
    public const string ResourceGroupMustBeFromDepartment = "Resource group must be from department";
    public const string ExternalAuthenticationError = "External authentication error";
    public const string UserIsNotInDomain = "User is not in modsen-software.com domain";
    public const string AccountIsNotAuthorized = "Account is not authorized";
    public const string InternalAuthenticationError = "Internal authentication error";
    public const string UserIsInactive = "User is inactive";
    public const string YouCannotChangeUserWithDifferentDepartment = "You can't change user with different department";
    public const string YouCannotChangeUserWithDifferentGroup = "You can't change user with different resource group";
    public const string GroupWithTheSameNameExists = "Group with the same name already exists";
    public const string GroupManagerAlreadyHasActiveGroup = "Group manager already has an active group";
    public const string GroupManagerIsFromAnotherDepartment = "Group Manager is in the department other than the group's department";
    public const string GroupAlreadyHasManager = "Group already has a group manager";
    public const string GroupCannotBeDeleted = "Group cannot be deleted. To delete group you should remove active GM and users from it";
    public const string OnlyEmployeeGroupMemberCanBeUpdated = "Only employee group member can be updated";
    public const string UserIsNotGroupMember = "UserIsNotGroupMember";
    public const string RoleCannotHaveGroup = "User cannot have a group";
    public const string HeadCannotUpdateGroupFromAnotherDepartment = "Head of department cannot update group from another department";
    public const string HeadCannotAssignUserFromAnotherDepartmentToGroup = "Head of department cannot assign to resource group user from another department";
    public const string YouCannotViewPlanOfUserFromAnotherDepartment = "You can not view plan for user from not your department";
    public const string YouCannotViewPlanOfUserFromAnotherResourceGroup = "You can not view plan for user from not your resource group";
    public const string YouCannotViewYourOwnPlan = "You can not view your plan";
    public const string HeadCannotRemoveUserFromAnotherDepartmentFromGroup = "Head of department cannot remove from resource group user from another department";
    public const string UserSalaryNotFound = "User salary not found";
    public const string SalaryDateConflicts =
        "Salary has already been set for the selected date. Please choose a different date or update the existing entry";
    public const string ProjectNotFound = "Project not found";
    public const string ProjectAssignNotFound = "Project assignment not found";
    public const string ClientNotFound = "Client not found";
    public const string HeadOfDepartmentDoesNotHaveAccessToTheProject = "Head of Department does not have access to the project";
    public const string HeadOfDepartmentDoesNotHaveAccessToProjectsOfUserFromAnotherDepartment = "Head of Department does not have access to projects of user from another department";
    public const string GroupManagerDoesNotHaveAccessToProjectsOfUserFromAnotherResourceGroup = "Group manager does not have access to projects of user from another resource group";
    public const string HeadOfDepartmentCannotAssignProjectManager = "Head of department cannot assign project manager";
    public const string ProjectCantBePublicAndBillable = "You cannot set project as public and billable";
    public const string ProjectWithTheSameNameExists = "Project with the same name already exists";
    public const string JiraSettingNotFound = "Jira setting not found";
    public const string JiraSettingAlreadyExists = "Jira setting already exists";
    public const string ClientWithTheSameNameExists = "Client with the same name already exists";
    public const string ProjectHasSameStatus = "Selected project already has this status";
    public const string YouCantActiveProject = "You can't activate this project";
    public const string YouCantDeactivateProject = "You can't deactivate this project";
    public const string YouCantUpdateInactiveProject = "You can't update deactivated project";
    public const string YouCantUpdateInactiveClient = "You can’t edit deactivated client";
    public const string CantDeactivateProjectWithManagerAndUsers = "Project can not be deactivated. To deactivate project you should remove assigned PM and users from the project";
    public const string CannotAccessOtherUserData = "You can't get other user's information";
    public const string CannotSpecifyMonthWithoutTheYear = "The month of the year cannot be specified without the year";
    public const string CannotSpecifyWeekWithoutTheYear = "The week of the year cannot be specified without the year";
    public const string ClientAlreadyDeleted = "Selected client is already deleted";
    public const string InactiveClientAssign = "It's not possible to assign Project to inactive Client";
    public const string UserCantBeManager = "Selected user can't be a manager";
    public const string CurrentUserCantBeProjectManager = "Current user can't be a project manager";
    public const string EmployeeCanGetOnlyManagedProjects = "Employee can get only managed projects";
    public const string OnlyManagedUsersUsageRestriction = "Only Employee and Head of Department can filter users by managed projects";
    public const string GroupManagerCanGetOnlyTheSameResourceGroup = "Group manager can't get timesheet of a user from another resource group";
    public const string HeadDoNotHaveAccessToOtherDepartments = "You don’t have an access to users from other departments";
    public const string GroupManagerDoNotHaveAccessToOtherGroup = "You don’t have an access to users from other resource groups";
    public const string ProjectAssignmentForPeriodAlreadyExists = "Project assignment for this period already exist. Please check and try again";
    public const string YouDoNotHaveAccessToUserProject = "You don't have an access to User's assignment to project";
    public const string TimesheetIsFilledForPeriod = "Timesheet is filled for this period. Please check and try again";
    public const string TimesheetNotFound = "Timesheet was not found";
    public const string OnlyManagersCanModifyApprovedAndSubmittedTimesheets = "Employee and SysAdmin cannot modify timesheets with Approved or Submitted statuses";
    public const string HeadCantExportReportOfUserFromOtherDepartment = "You can't export user's report file from another department";
    public const string GroupManagerCantExportReportOfUserFromOtherGroup = "You can't export user's report file from another resource group";
    public const string EmployeesNotFound = "Employees don't found!";
    public const string HeadShouldChooseHisDepartment = "Head of department should choose his/her department";
    public const string HeadCannotExportCsvOfUserFromOtherDepartment = "You can't export user's csv file from another department";
    public const string GroupManagerCannotExportCsvOfUserFromOtherGroup = "You can't export user's csv file from another resource group";
    public const string UserIdHasNotBeenProvided = "The user id hasn't been provided";
    public const string ThereIsNoAppropriateRole = "There is no appropriate role";
    public const string CannotTrackBillableHoursWithoutPermission = "You can't track billable hours without permission at project";
    public const string CannotManageTimesheetAtInactiveProject = "You can't manage any timesheet information at inactive project";
    public const string TaskWithSameNameAlreadyExists = "Task with the same name already exists.\r\n It is not allowed to create the same Task name within the same day and project. Please, check month timesheet";
    public const string NoActiveAssignmentFoundForDate = "No active assignment found for the selected date";
    public const string UserCannotCreateTimesheetForOtherUsers = "The user can't create timesheet for other user";
    public const string CannotManageTimesheetOfInactiveUser = "You can't manage any timesheet information of inactive user";
    public const string HeadCannotCreateTimesheetForUserFromOtherDepartment = "Head of department can't create timesheet for user from other department";
    public const string GroupManagerCannotCreateTimesheetForUserFromOtherGroup = "Resource manager can't create timesheet for user from other resource group";
    public const string MonthMustBeBetweenOneAndTwelve = "The month must be between 1 and 12";
    public const string CannotGetTimesheetSubmitStatusForUser = "You can't get status for this user";
    public const string CannotUpdateStatusForYearEarlierThan2020 = "You can't update status for the year that earlier than 2020";
    public const string StatusHasAlreadyBeenSent = "This status has already been sent";
    public const string MonthHasAlreadyBeenApproved = "This month has already been approved";
    public const string CannotChangeOtherUsersTimesheetStatus = "You can't change other user's timesheet status";
    public const string TimesheetCannotBeRejectedBeforeUnderReviewStatus = "Timesheets can't be rejected before UnderReview status";
    public const string HeadCannotUpdateStatusForUserFromOtherDepartment = "Head of department can't update status for user not from his department";
    public const string GmCannotUpdateStatusForUserFromOtherGroup = "Group manager can't update status for user not from his resource group";
    public const string CurrentUserCanSendOnlyApprovedOrRejectedStatuses = "User with current role can send only status 'approved' or 'rejected'";
    public const string CurrentUserCanOnlySendUnderReviewStatus = "User with current role can send only status 'UnderReview'";
    public const string RegisteredLocationNotFound = "There's no such location";
    public const string OperationCanceledMessage = "The request has been accepted for processing, but the processing has not been completed. The request might or might not be eventually acted upon, and may be disallowed when processing occurs.";

    public const string SoftSkillNameAlreadyOccupied = "Soft skill with the same name already exists";
    public const string SomeHardSkillsNotFound = "A hard skill or some hard skills don't exist";
    public const string SomeSeniorityLevelsNotFound = "A seniority level or some seniority levels don't exist";
    public const string SomeSpecializationTemplatesNotFound = "A specialization template or some specialization templates don't exist";
    public const string SomeHardSkillsDontBelongToSpecializationTemplate = "Some hard skills don't belong to specialization template";

    //Merch messages:
    public const string MerchItemNotFound = "Merch item not found";
    public const string MerchItemNameExists = "Merch item with the same name already exists";
    public const string NotAllSizesAreListed = "Not all sizes are listed";
    public const string NotAllStoragesAreListed = "Not all storages are listed";
    public const string MerchKitNotFound = "Kit not found";
    public const string MerchKitNameExists = "Kit with the same name already exists";
    public const string OnlyOneClothingInKit = "You can add only 1 'Clothing' type item";
    public const string NotAllCompoundsExistOrAvailable = "Not all compounds are exist or have available status";
    public const string KitNotFound = "Kit not found";
    public const string KitIsNotAvailable = "Kit is not in available status";
    public const string MerchItemIsNotAvailable = "Merch item is not in available status";

    //Inquiries messages:
    public const string InquiryTemplateNotFound = "Inquiry is not found";
    public const string AgreementTemplateNotFound = "Agreement is not found";
    public const string UserAgreementNotFound = "User agreement is not found";
    public const string UserInquiryNotFound = "User inquiry is not found";
    public const string ApproversCountIsMoreThanAllowed = "You cannot add more than 6 approvers";
    public const string ApproversCountIsLessThanAllowed = "You must add al least 1 approver";
    public const string DuplicatedScopeAndSubstitute = "Scope and substitute scope are duplicated";
    public const string AcceptPolicyMustHasSubstituteUser = "Accept policy must has substitute user";
    public const string AcceptPolicyMustNotHasSubstituteUser = "Accept policy must not has substitute user";
    public const string AcceptPolicyMustNotHasSubstituteScope = "Accept policy must not has substitute scope";

    // Cv messages:
    public const string CvNameExists = "CV name is already used";
    public const string EnteredIdenticalCvSpecialization = "Specializations cannot match";
    public const string EnteredIdenticalCvLanguages = "Languages cannot match";
    public const string EnteredIdenticalCvSkills = "Skills cannot match";
    public const string EnteredIdenticalCvSkillGroupNames = "Skill group name is already used";
    public const string EnteredIdenticalCvSkillGroupRuNames = "Skill group russian name is already used";
    public const string CvNotFound = "Cv is not found";

    public const string DigitalHrFirstMessage = "Приветствую 👋\nЧем могу помочь?";
    public const string DigitalHrEmptyMessage = "К сожалению, я не владею данной информацией. Если у вас есть другие вопросы, связанные с HR или внутренними процессами, пожалуйста, дайте знать!";

    //Finance messages
    public const string ProjectRateDateConflicts = "Rate with the same date already exists";
    public const string ProjectRateNotFound = "Project rate is not found";
    public const string ProjectRateCannotBeEdited = "This roject rate cannot be edited";
    public const string ProjectRatesOnlyForPrivateProjects = "Project rates can only be set for private projects.";
    public const string ExpenseCategoryExists = "Expense category already exists";
    public const string ExpenseCategoryNotFound = "Expense category is not found";
    public const string ExpenseNotFound = "Expense entry is not found";
    public const string ExpenseAlreadyExists = "Expense entry with such year, month and expense category already exists";

    //LabBoard messages
    public const string InternshipNotFound = "Internship is not found";
    public const string NotAllowedOperationWithIntern = "This operation with intern is not allowed";
    public const string CannotCreateInternshipForNotIntern = "Internship can be created only for interns";
    public const string InternshipAlreadyExists = "Internship already exists";
}
