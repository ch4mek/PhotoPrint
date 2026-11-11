import {
	PROJECT_ASSIGNMENT_ISSUE,
	SOMETHING_WENT_WRONG,
	TIMESHEET_PROJECT_ISSUE,
} from '@/constants/errors';

export const convertProjectMutationErrorToToast = (errorMessage: string) => {
	if (
		errorMessage.includes(PROJECT_ASSIGNMENT_ISSUE) ||
		errorMessage.includes(TIMESHEET_PROJECT_ISSUE)
	) {
		return errorMessage;
	}

	return SOMETHING_WENT_WRONG;
};
