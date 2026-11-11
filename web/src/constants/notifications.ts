export enum NotificationType {
	INFO = 'info',
	SUCCESS = 'success',
	ERROR = 'error',
}

export const SESSION_EXPIRED =
	'Your session has timed out. Please, log in again to continue working';

export const PROFILE_UPDATING = 'Your changes have been successfully saved';
export const NO_DATA = 'No data found';
export const CLIENT_ADDED = 'Client is successfully added';
export const CLIENT_UPDATED = 'Client is successfully updated';
export const PROJECT_ADDED = 'Project is successfully added';
export const PROJECT_UPDATED = 'Project is successfully updated';
export const USER_ADDED = 'User is successfully added';
export const USER_UPDATED = 'User is successfully updated';
export const CERTIFICATE_GENERATED_SUCCESSFULLY =
	'Certificate number successfully generated';
export const CERTIFICATE_DELETED_SUCCESSFULLY =
	'Certificate is deleted successfully';
export const CERTIFICATE_GENERATED_ERROR =
	'Error generating certificate number';
export const NOTIFICATION_MESSAGE = {
	PROJECT_UPDATED: 'Project updated',
	CANNOT_DEACTIVATE_PROJECT:
		'Project can not be deactivated. To deactivate project you should remove users from project',
};
export const MINIMUM_ALLOWED_FILE_SIZE = {
	message: (minimumFileSizeInMb: number) =>
		`File size should be greater than ${minimumFileSizeInMb} MB`,
};
export const MAXIMUM_ALLOWED_FILE_SIZE = {
	message: (maximumFileSizeInMb: number) =>
		`File max size should not exceed ${maximumFileSizeInMb} MB`,
};
export const RIGHT_EXTENSIONS = {
	message: (allowedExtensions: string) =>
		`You can upload files only in ${allowedExtensions} formats`,
};
export const MAXIMUM_NUMBER_OF_FILES = {
	message: (maxFilesAmount: number, attachedFiles: number) =>
		`Amount of files should not be greater than ${maxFilesAmount}. Only ${attachedFiles} ${
			attachedFiles > 1 ? 'files are' : 'file is'
		} attached`,
};
export const INVALID_DATE_RANGE = 'The date range is invalid';
export const ERROR_MESSAGE_UPDATE_PROJECT =
	'You can’t edit the Project in saved record. To edit Project you should clear tracked hours manually at first';
export const CANT_ASSIGN_ROLE = 'You can assign users with employee role only';
export const EQUIPMENT_CREATED =
	'Equipment details have been successfully added';
export const EQUIPMENT_UPDATED =
	'Equipment details have been successfully edited';
export const EQUIPMENT_FUTURE_RETURN_DATE_ERROR =
	'You cannot set return date for the future';
export const ALREADY_BOOKED =
	'You already have a booking for selected period. Please cancel the booking or choose another date(s)';
export const GUEST_ALREADY_BOOKED =
	'Guest with this name already has a booking for selected period. Please cancel the booking or choose another date(s)';
export const COPY_TO_CLIPBOARD = 'Signature has been successfully copied';
export const HAS_BEEN_CLEARED = 'Signature form has been successfully cleared';
export const HAS_BEEN_DOWNLOADED = 'Signature has been successfully downloaded';
export const HAS_BEEN_GENERATED = 'Signature has been successfully generated';
export const AUTHORIZATION_ERROR_MESSAGE = 'Error during authorization';
export const CANNOT_DEACTIVATE_USER = `The user can't be deactivated. Please, ensure the employee has returned issued equipment and is not an approver of any; has no project or PM assignments; has no active workplace bookings; and is not part of group`;
export const USER_NOT_FOUND = `User not found`;
export const SAME_SERIAL_NUMBER_ALREADY_EXISTS =
	'An equipment with the same serial number already exists';
export const SAME_INVENTORY_NUMBER_ALREADY_EXISTS =
	'An equipment with the same inventory number already exists';
export const PLEASE_CHANGE_SERIAL_NUMBER = 'Please change the serial number';
export const PLEASE_CHANGE_INVENTORY_NUMBER =
	'Please change the inventory number';
export const DESK_IS_ALREADY_BOOKED_RESPONSE_MESSAGE =
	'Booking for workplace already exists!';
export const ISSUE_DATE_SHOULD_NOT_BE_LESS_LAST_RETURN_DATE =
	'Issue date shouldn’t be less than latest Return date';
export const EMPLOYMENT_HISTORY_UPDATED = 'Employment history is updated';
export const INTERNSHIP_UPDATED = 'Internship information is updated';
export const REGISTERD_LOCATION_UPDATED = 'Registered location is updated';
export const PLAN_UPDATED = 'Plan is updated';
export const PLAN_ADDED = 'Plan is successfully added';
export const PERSONAL_DATA_UPDATED = 'Personal data is updated';
export const COMMENT_UPDATED = 'Comment is updated';
export const NEED_PROJECT_UPDATED = 'User’s details are succesfully updated';
export const PROJECT_HISTORY_UPDATED = 'Project history is updated';
export const SPECIALIZATION_DELETED = 'Specialization is successfully deleted';
export const FEEDBACK_SUCCESS_MESSAGE = 'Feedback history is updated';

export const ITEM_ADDED_POSTFIX = 'is successfully added';
export const ITEM_UPDATED_POSTFIX = 'is successfully updated';
export const ITEM_DELETED_POSTFIX = 'is successfully deleted';
export const ITEM_WILL_DELETED_POSTFIX = 'will be deleted.';
export const HARD_SKILL_DELETING_MESSAGE =
	'If the hard skill is assigned to any specialization, it will be removed from the specialization details.';
export const SOFT_SKILL_DELETING_MESSAGE =
	'will be deleted. It will be removed from all users.';
export const ASSIGNED_SPECIALIZATION_DELETING_MESSAGE =
	'You can’t delete specialization that is assigned to a user';

export const USER_HARD_SKILL_UPDATED = 'Hard skills are updated';
export const CONVERSATION_SUCCESS_MESSAGE = 'Conversation history is updated';
export const USER_SOFT_SKILLS_ARE_UPDATED = 'Soft skills are updated';
export const USER_SALARY_UPDATED = 'Salary history is updated';
export const DEVELOPMENT_PLAN_SUCCESS_MESSAGE = 'Development plan is updated';
export const JIRA_SETTINGS_SAVED = 'Settings saved';
export const JIRA_SETTING_DELETED = 'Setting deleted';
export const PROJECT_RATE_ADDED = 'New rate has been successfully added';
export const PROJECT_RATE_UPDATED = 'Rate has been successfully updated';

export const PERFORMANCE_REVIEW_HISTORY_UPDATED =
	'Performance review history is updated';

export const PAYLOAD_DETAILS_UPDATED =
	'User’s details are successfully updated';

export const GROUP_CREATED = 'Group is successfully added';
export const GROUP_UPDATED = 'Group is successfully updated';
export const GROUP_DELETED = 'Group is successfully deleted';

export const REQUEST_ABSENCE_CREATED = 'Absence request was successfully sent';

export const REQUEST_DOCUMENTS_CREATED =
	'Documents request was successfully sent';

export const ABSENCE_AND_AGREEMENTS_DELETED =
	'Absence and agreements are successfully deleted';

export const SAME_ABSENCE_NAME_ALREADY_EXISTS =
	'Absence with the same name already exists';

export const SHORT_SAME_ABSENCE_NAME_ALREADY_EXISTS =
	'Absence name is already used';

export const ABSENCE_REQUEST_NOT_FOUND = 'Absence request is not found';

export const ABSENCE_REQUEST_NOTIFICATION_SENT =
	'The notification was successfully sent';

export const getSuccessfullyAddedMessage = (field: string | null) =>
	`${field} is successfully added`;
export const getSuccessfullyUpdatedMessage = (field: string | null) =>
	`${field} is successfully updated`;
export const getSuccessfullyDeletedMessage = (field: string | null) =>
	`${field} is successfully deleted`;
export const getSuccessfullyActivatedMessage = (field: string | null) =>
	`${field} is successfully activated`;
export const getSuccessfullyArchivedMessage = (field: string | null) =>
	`${field} is successfully archived`;
export const getSuccessfullyRemovedMessage = (field: string | null) =>
	`${field} is successfully removed`;

export const HAS_BEEN_SUCCESSFULLY_MESSAGE = 'has been successfully';

export const getHasBeenSuccessfullyAddedMessage = (field: string | null) =>
	`New ${field?.toLocaleLowerCase()} ${HAS_BEEN_SUCCESSFULLY_MESSAGE} added`;
export const getHasBeenSuccessfullyUpdatedMessage = (field: string | null) =>
	`${field} ${HAS_BEEN_SUCCESSFULLY_MESSAGE} updated`;
export const getHasBeenSuccessfullyDeletedMessage = (field: string | null) =>
	`${field} ${HAS_BEEN_SUCCESSFULLY_MESSAGE} deleted`;

export const COMMENT_MESSAGES = {
	UPDATED_SUCCESS: 'Comment is successfully updated',
	CREATED_SUCCESS: 'Comment is successfully created',
	DELETED_SUCCESS: 'Comment is successfully deleted',
	UPDATE_ERROR: 'Failed to update comment',
	CREATE_ERROR: 'Failed to create comment',
	DELETE_ERROR: 'Failed to delete comment',
	PARSE_ERROR: 'Failed to parse comment data',
	NOTIFY_ERROR: 'Failed to notify approvers',
} as const;

export const EMPLOYEE_CONTRACT_EXPIRES_IN = {
	45: '45 days',
	90: '3 months',
};

export const MAXIMUM_ALLOWED_CLOTHING_KITS_TITLE =
	"Maximum 'Clothing' type compounds exceeded";

export const MAXIMUM_ALLOWED_CLOTHING_KITS_MESSAGE =
	"You can add only 1 'Clothing' type item";

export const MICROFRONTEND_MESSAGES = {
	UPDATED_SUCCESS: 'Microfrontend is successfully updated',
	CREATED_SUCCESS: 'Microfrontend is successfully created',
	DELETED_SUCCESS: 'Microfrontend is successfully deleted',
	UPDATE_ERROR: 'Microfrontend to update comment',
	CREATE_ERROR: 'Microfrontend to create comment',
	DELETE_ERROR: 'Microfrontend to delete comment',
};

export const INTERVIEW_MESSAGES = {
	CREATED_SUCCESS: 'Interview has been successfully added',
	UPDATE_SUCCESS: 'Interview details have been successfully updated',
};

export const DISCOUNT_MESSAGES = {
	EXPORT_ERROR: 'File export failed. Please try again',
};

export const PROJECT_EXIT_MESSAGES = {
	CREATED_SUCCESS: 'Project exit reason has been successfully added',
	EDITED_SUCCESS: 'Project exit reason has been successfully edited',
};

export const JIRA_IMPORT_SUCCESS_MESSAGE =
	'Jira timesheets successfully imported';
