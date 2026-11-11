import { BillableWorkingHoursPermissionsQueryMode } from './grids/constants';

export const getPermissionAssignmentToastMessage = (
	actionType?: BillableWorkingHoursPermissionsQueryMode
) =>
	`Permission to track billable hours has been successfully ${
		actionType === BillableWorkingHoursPermissionsQueryMode.ADD
			? 'added to '
			: 'revoked from '
	}`;
