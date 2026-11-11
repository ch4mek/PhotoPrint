import { fromToDateObject } from '@/constants/defaultValues';
import { isGroupManager, isHeadOrGroupManager, Roles } from '@/constants/roles';
import { TRACKED_TIME_DEFAULT_VALUE } from '@/pages/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { AttendanceTotalFilters } from './types';

export const DEFAULT_ATTENDANCE_TOTAL_FILTERS: AttendanceTotalFilters = {
	userId: '',
	user: '',
	departments: [],
	group: '',
	date: fromToDateObject,
	time: TRACKED_TIME_DEFAULT_VALUE,
};

export const INITIAL_ORDER_BY_VALUE = 'date';

export const getAttendanceTotalColumns = (
	role: Roles
): GridColumnWidthsConfiguration => {
	const isDepartmentColumnVisible = !isHeadOrGroupManager(role);
	const isGroupColumnVisible = !isGroupManager(role);

	return {
		userId: {
			width: 67,
			fixed: true,
		},
		user: {
			width: 160,
		},
		department: isDepartmentColumnVisible
			? {
					width: 160,
			  }
			: {
					width: 0,
			  },
		group: isGroupColumnVisible
			? {
					width: 160,
			  }
			: {
					width: 0,
			  },
		date: {
			width: 160,
		},
		time: {
			width: 160,
		},
	};
};

export const ATTENDANCE_TOTAL_GRID_COLUMNS = {
	userId: {
		key: 'userId',
		name: 'Id',
	},
	user: {
		key: 'user',
		name: 'User',
	},
	department: {
		key: 'department',
		name: 'Department',
	},
	group: {
		key: 'group',
		name: 'Group',
	},
	date: {
		key: 'date',
		name: 'Date',
	},
	time: {
		key: 'time',
		name: 'Duration',
	},
};

export const ATTENDANCE_TOTAL_FILTER_KEYS: Array<keyof AttendanceTotalFilters> =
	['userId', 'user', 'departments', 'group', 'date', 'time'];

export const getRowKey = (row: { id: string }) => row.id;

export const INACTIVE_USER_ROW_CLASS = 'inactive-user-row';

export const getRowClass = (row: { isUserActive: boolean }) =>
	row.isUserActive ? undefined : INACTIVE_USER_ROW_CLASS;

export const MINUTES_IN_HOUR = 60;
export const TIME_PAD_LENGTH = 2;
export const TIME_PAD_CHAR = '0';
export const TIME_SEPARATOR = ':';

export const MIN_WORKING_TIME_IN_MINUTES = MINUTES_IN_HOUR * 8;
