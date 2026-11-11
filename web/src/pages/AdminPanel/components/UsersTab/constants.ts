import { Roles, isGroupManager, isHeadOrGroupManager } from '@/constants/roles';
import { DefaultSortFields } from '@/constants/sort';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { GridColumnWidthsConfiguration } from '@/types/grid';
import { UsersGridRow } from '@/types/userInfo';

import { UsersFilterType } from './types';

export const DEFAULT_USER_ORDER_BY = DefaultSortFields.ID;

export const CLEAR_FILTERS_COLUMN_INDEX = 10;

export const SEARCH_INPUT_DELAY = 500;

export const DEFAULT_USER_FILTERS: UsersFilterType = {
	id: '',
	user: '',
	email: '',
	departments: [],
	group: '',
	location: [],
	position: [],
	status: IS_ACTIVE_DEFAULT_VALUE,
};

export const getUsersColumnsWidths = (
	role: Roles,
	hasUsersModuleAccess: boolean
): GridColumnWidthsConfiguration => ({
	id: {
		fixed: true,
		width: 67,
	},
	user: 200,
	email: 260,
	position: 153,
	department: isHeadOrGroupManager(role) && !hasUsersModuleAccess ? 0 : 159,
	group: isGroupManager(role) && !hasUsersModuleAccess ? 0 : 200,
	status: 88,
	location: 100,
});

export const getRowKey = (row: UsersGridRow) => row.id;

export const userMenuOptionsValues = {
	EDIT: 'Edit',
	ACTIVATE: 'Activate',
	DEACTIVATE: 'Deactivate',
};
