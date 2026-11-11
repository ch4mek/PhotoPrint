import { UserClaimTableFilters } from '@/types/settings';

const ID_COLUMN_WIDTH = 67;
const NAME_COLUMN_WIDTH = 260;
const NAME_COLUMN_WIDTH_MOBILE = 200;
const DEPARTMENT_COLUMN_WIDTH = 77;
export const CLAIM_COLUMN_WIDTH = 85;

export const NAME_COLUMN_KEY = 'userFullName';
export const SORT_USERS_ID = 'userId';

export const firstClaimsColumnSetup = {
	name: 'Id',
	key: SORT_USERS_ID,
	width: `${ID_COLUMN_WIDTH}px`,
};

export const getFirstUsersClaimsColumnSetup = (isExpanded: boolean) => ({
	name: 'User',
	key: 'name',
	width: `${isExpanded ? NAME_COLUMN_WIDTH : NAME_COLUMN_WIDTH_MOBILE}px`,
});

export const userDepartmentColumnSetup = {
	name: 'Dep.',
	key: 'departmentId',
	width: `${DEPARTMENT_COLUMN_WIDTH}px`,
};

const sharedColumnProps = {
	sortable: true,
	fixed: true,
	resizable: false,
};

export const userClaimsIdColumnProps = {
	...firstClaimsColumnSetup,
	...sharedColumnProps,
};

export const getUserClaimsColumnProps = (isCollapsed: boolean) => ({
	...getFirstUsersClaimsColumnSetup(isCollapsed),
	...sharedColumnProps,
});

export const departmentClaimsColumnProps = {
	...userDepartmentColumnSetup,
	...sharedColumnProps,
};

export const DEFAULT_USER_CLAIMS_FILTER_VALUES: UserClaimTableFilters = {
	userClaimName: '',
	userId: '',
	departments: [],
};

export const PERMISSIONS_POPUP_TITLE = 'Permissions';
export const PERMISSIONS_SUCCESSFULY_UPDATE_TEXT =
	'Permissions have been successfully updated';
