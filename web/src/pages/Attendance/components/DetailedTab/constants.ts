import { fromToDateObject } from '@/constants/defaultValues';
import { isGroupManager, isHeadOrGroupManager, Roles } from '@/constants/roles';
import { TRACKED_TIME_DEFAULT_VALUE } from '@/pages/constants';
import { AttendenceDirection } from '@/types/attendances';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { AttendanceDetailedFilters } from './types';

export const DEFAULT_ATTENDANCE_DETAILED_FILTERS: AttendanceDetailedFilters = {
	id: '',
	user: '',
	departments: [],
	group: '',
	direction: [],
	door: '',
	location: '',
	date: fromToDateObject,
	time: TRACKED_TIME_DEFAULT_VALUE,
};

export const getAttendanceDetailedColumns = (
	role: Roles
): GridColumnWidthsConfiguration => {
	const isDepartmentColumnVisible = !isHeadOrGroupManager(role);
	const isGroupColumnVisible = !isGroupManager(role);

	return {
		id: {
			width: 67,
			fixed: true,
		},
		user: {
			width: 200,
		},
		department: isDepartmentColumnVisible
			? {
					width: 159,
					fixed: true,
			  }
			: {
					width: 0,
			  },
		group: isGroupColumnVisible
			? {
					width: 87,
					fixed: true,
			  }
			: {
					width: 0,
			  },
		direction: {
			width: 100,
		},
		door: {
			width: 80,
		},
		location: {
			width: 100,
		},
		date: {
			width: 100,
		},
		time: {
			width: 85,
		},
	};
};

export const DIRECTION_OPTIONS = [
	{ id: AttendenceDirection.Exit, description: AttendenceDirection.Exit },
	{ id: AttendenceDirection.Enter, description: AttendenceDirection.Enter },
];

export const ATTENDANCE_DETAILED_GRID_COLUMNS = {
	id: {
		key: 'id',
		name: 'Id',
	},
	user: {
		key: 'user',
		name: 'User',
	},
	department: {
		key: 'userDepartmentId',
		name: 'Department',
	},
	group: {
		key: 'userResourceGroupName',
		name: 'Group',
	},
	direction: {
		key: 'direction',
		name: 'Direction',
	},
	door: {
		key: 'door',
		name: 'Door',
	},
	location: {
		key: 'location',
		name: 'Location',
	},
	date: {
		key: 'date',
		name: 'Date',
	},
	time: {
		key: 'time',
		name: 'Time',
	},
};

export const getRowKey = (row: { id: string }) => row.id;

export const INACTIVE_USER_ROW_CLASS = 'inactive-user-row';

export const getRowClass = (row: { isUserActive: boolean }) =>
	row.isUserActive ? undefined : INACTIVE_USER_ROW_CLASS;
