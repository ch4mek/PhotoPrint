import { TimesheetModeFilterOption, TimesheetStatusType } from '../../types';
import { TimeTrackItem } from '../TimesheetEngineer/types';
import { FilterOption } from '../TimesheetPageControls/types';

import { Roles } from '@/constants/roles';

export enum TimeSheetManagerRowType {
	UserTimesheet = 'UserTimesheet',
	UserInfo = 'UserInfo',
}

export interface TimeSheetManagerProps {
	month: number;
	departmentTypeFilter: TimesheetModeFilterOption;
	clientFilter: FilterOption;
	projectFilter: FilterOption;
	userFilter: FilterOption;
	periodFilter: string;
	period: Date[];
	groupFilter?: FilterOption;
}

export interface TimeSheetManagerRow {
	userId: number;
	isActiveUser: boolean;
	userDepartmentId: string;
	role: Roles;
	userName: string;
	color: string;
	timeMap: Map<string, TimeTrackItem | undefined>;
	userTimesheetRowHeight?: number;
	periodLength: number;
	isExpanded: boolean;
	shouldDisplaySkeleton: boolean;
	status: TimesheetStatusType | null;
}
