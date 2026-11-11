import { SortDirection } from '@mui/material';

import { ByPageResponse } from '@/queries/types';

export interface UseBillableWorkingHoursPermissionsOptions {
	page: number;
	rowsPerPage: number;
	order: SortDirection;
	orderBy: string;

	userIdFilter?: string;
	userFullNameFilter?: string;
	projectNameFilter?: string;

	userDepartmentId?: string;
	permissionName?: string;
	permissionId?: string;
}

interface BillableWorkingPermissionAssignment {
	assignmentId: number;
	userId: number;
	userFullName: string;
	projectId: string;
	projectName: string;
	userColor: string;
}

export type BillableWorkingPermissionsAssignments =
	ByPageResponse<BillableWorkingPermissionAssignment>;
