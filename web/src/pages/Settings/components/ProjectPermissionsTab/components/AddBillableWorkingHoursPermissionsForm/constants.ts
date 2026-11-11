import { BillableWorkingHoursPermissionsQueryMode } from '../../grids/constants';

import { ActivityStatus } from '@/constants/activityStatus';

import { BillableWorkingHoursPermissionsFormValues } from './types';

export const ADD = BillableWorkingHoursPermissionsQueryMode.ADD;
export const REVOKE = BillableWorkingHoursPermissionsQueryMode.REVOKE;

export const allAssignedPrivateProjectsPaginationParams = {
	top: 100000,
	page: 0,
};

export const initialBillableWorkingHoursPermissionsFormValues: BillableWorkingHoursPermissionsFormValues =
	{
		user: null,
		projects: [],
	};

export const allAssignedPrivateProjectsFilters = {
	isActive: [ActivityStatus.Active],
	projectName: '',
	clientName: '',
	billability: [],
	startDate: {},
	endDate: {},
};
