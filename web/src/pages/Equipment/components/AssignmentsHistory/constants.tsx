import { fromToDateObject } from '@/constants/defaultValues';
import { isHeadOrGroupManager, Roles } from '@/constants/roles';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { UserAccessClaims, UserClaims } from '@/types/claims';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import { AssignmentHistoryFilters } from './types';

export const DEFAULT_ASSIGNMENT_HISTORY_FILTERS: AssignmentHistoryFilters = {
	userId: '',
	equipmentId: '',
	userFullName: '',
	isAssigned: [],
	departments: [],
	equipmentName: '',
	type: [],
	serialNumber: '',
	inventoryNumber: '',
	issueDate: fromToDateObject,
	returnDate: fromToDateObject,
};

export const getAssignmentsHistoryColumnsWidths = (
	role: Roles,
	claims?: UserAccessClaims
): GridColumnWidthsConfiguration => {
	const hasManageAllEquipmentClaim = claims
		? checkIsHasClaims([UserClaims.ManageAllEquipment], claims)
		: false;
	const shouldHideColumns =
		isHeadOrGroupManager(role) && !hasManageAllEquipmentClaim;

	return {
		userId: 90,
		equipmentId: 90,
		user: 323,
		isAssigned: 120,
		department: shouldHideColumns ? 0 : 159,
		equipmentName: 323,
		equipmentTypeId: 138,
		serialNumber: 323,
		inventoryNumber: 150,
		issueDate: 116,
		returnDate: 125,
	};
};

export const ASSIGNMENTS_HISTORY_COLUMNS = {
	userId: {
		key: 'userId',
		name: 'Us. id',
	},
	equipmentId: {
		key: 'equipmentId',
		name: 'Eq. id',
	},
	user: {
		key: 'userFullName',
		name: 'User',
	},
	isAssigned: {
		key: 'isAssigned',
		name: 'Is assigned',
	},
	department: {
		key: 'userDepartmentId',
		name: 'Department',
	},
	equipmentName: {
		key: 'equipmentName',
		name: 'Equipment name',
	},
	equipmentTypeId: {
		key: 'equipmentTypeId',
		name: 'Type',
	},
	serialNumber: {
		key: 'serialNumber',
		name: 'Serial number',
	},
	inventoryNumber: {
		key: 'inventoryNumber',
		name: 'Inventory number',
	},
	issueDate: {
		key: 'issueDate',
		name: 'Issue date',
	},
	returnDate: {
		key: 'returnDate',
		name: 'Return date',
	},
};

export const getAssignmentsHistoryEditColumnIndex = (
	role: Roles,
	claims?: UserAccessClaims
) =>
	Object.values(getAssignmentsHistoryColumnsWidths(role, claims)).filter(
		Boolean
	).length + 2;
