import { UseQueryResult } from '@tanstack/react-query';

import { BillableWorkingPermissionsAssignments } from '../useBillableWorkingHoursPermissions/types';

import { ServerResponseError } from '@/queries/types';

export interface BillableWorkingHoursPermissionsRow {
	userId: number;
	assignmentId: number;
	userFullName: string;
	projectId: string;
	projectName: string;
	showSkeleton: boolean;
	color: string;
}

export interface UseBillableWorkingHoursPermissionsRowsOptions {
	billableWorkingHoursPermissionsRequest: UseQueryResult<
		BillableWorkingPermissionsAssignments,
		ServerResponseError
	>;
	page: number;
	rowsPerPage: number;
}
