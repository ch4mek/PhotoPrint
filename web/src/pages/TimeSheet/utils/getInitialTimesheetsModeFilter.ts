import {
	initialPMOEmployeeTimesheetMode,
	defaultInitialTimesheetMode,
	initialGroupManagerTimesheetMode,
} from '../constants';

import { isPMO } from '@/constants/departments';
import { Roles, isHeadOrGroupManager, isGroupManager } from '@/constants/roles';

export const getInitialTimesheetsModeFilters = (
	role: Roles,
	departmentId: string
) => {
	if (!isHeadOrGroupManager(role) && isPMO(departmentId)) {
		return initialPMOEmployeeTimesheetMode;
	}

	if (isGroupManager(role)) {
		return initialGroupManagerTimesheetMode;
	}

	return defaultInitialTimesheetMode;
};
