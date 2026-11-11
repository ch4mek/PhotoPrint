import { TimesheetCalendarResponse } from '@/queries/timesheet/types';

export interface TimeshitsWidgetProps {
	data: TimesheetCalendarResponse[];
	isLoading: boolean;
}
