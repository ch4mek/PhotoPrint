import { TimesheetStatusType } from '../../types';

export interface TimesheetSubmitStatusProps {
	status: TimesheetStatusType | null;
	isHeaderStatus?: boolean;
}
