import {
	TIMESHEET_CLOSED_ROW_HEIGHT,
	TIMESHEET_MANAGER_ROW_HEIGHT_MOBILE,
} from '../../TimesheetEngineer/constants';

import { filterByPeriod } from '@/constants/filters';
import {
	Roles,
	isEmployee,
	isGroupManager,
	isManagerRole,
} from '@/constants/roles';
import { timesheetModesIds } from '@/pages/TimeSheet/constants';
import { TimesheetModeFilterOption } from '@/pages/TimeSheet/types';

const { groupModeId, personalModeId, projectsModeId } = timesheetModesIds;

export const getRowHeight = (
	role: Roles,
	isTabletWidth: boolean,
	periodFilter: string,
	departmentTypeFilter: TimesheetModeFilterOption
) => {
	const isMonthFilter = filterByPeriod.monthFilter === periodFilter;
	const isPersonalMode = personalModeId === departmentTypeFilter.id;
	const isGroupMode = groupModeId === departmentTypeFilter.id;
	const isProjectsMode = projectsModeId === departmentTypeFilter.id;

	const isNarrowMonthView = !isTabletWidth && isMonthFilter;

	const isRoleWithModeSpecificHeight =
		(isGroupManager(role) && isGroupMode) ||
		(isEmployee(role) && isProjectsMode);

	const hasExtendedNameCellRole =
		isManagerRole(role) || isRoleWithModeSpecificHeight;

	const shouldExtendNameCell =
		isNarrowMonthView && !isPersonalMode && hasExtendedNameCellRole;

	return shouldExtendNameCell
		? TIMESHEET_MANAGER_ROW_HEIGHT_MOBILE
		: TIMESHEET_CLOSED_ROW_HEIGHT;
};
