import { TimesheetStatusType } from '../types';

export function getPreferrableTimesheetStatus(
	firstStatus: TimesheetStatusType | null,
	secondStatus: TimesheetStatusType | null
) {
	if (firstStatus === null || firstStatus === TimesheetStatusType.Rejected) {
		return firstStatus;
	}

	return secondStatus;
}
