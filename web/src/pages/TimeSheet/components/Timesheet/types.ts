import { TimesheetModeFilterOption, TimesheetStatusType } from '../../types';

import { FilterOption } from '@/pages/TimeSheet/components/TimesheetPageControls/types';

export interface TimeSheetProps {
	month: number;
	userId: number;
	period: Date[];
	departmentTypeFilter: TimesheetModeFilterOption;
	clientFilter: FilterOption;
	projectFilter: FilterOption;
	userFilter: FilterOption;
	groupFilter: FilterOption;
	periodFilter: string;
	isLoading: boolean;
	departmentId: string;
	engineerStatus?: TimesheetStatusType | null;
}
