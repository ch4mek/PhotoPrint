import { BillableWorkingHoursPermissionsQueryMode } from '../../../../grids/constants';

import {
	PermissionTypeId,
	PermissionResourceTypeId,
} from '@/types/configuration';

export interface CreatePermissionAssignmentOptions {
	userId: number;
	permissionType: PermissionTypeId;
	// For example: project ids
	resources: object;
	resourceType: PermissionResourceTypeId;
	userName: string;
	actionType?: BillableWorkingHoursPermissionsQueryMode;
}
