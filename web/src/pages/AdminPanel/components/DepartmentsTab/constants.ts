import { UsersFilterType } from '../UsersTab/types';

import { Roles } from '@/constants/roles';
import { SortDirection } from '@/constants/sort';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';

export const departmentGroupManagersQueryParams: UsersFilterType = {
	id: '',
	user: '',
	email: '',
	departments: [],
	location: [],
	position: [Roles.GM],
	status: IS_ACTIVE_DEFAULT_VALUE,
	group: '',
};

export const DEPARTMENT_GROUP_MANAGERS_QUERY_PAGE = 1;
export const DEPARTMENT_GROUP_MANAGERS_QUERY_TOP = 10000;
export const DEPARTMENT_GROUP_MANAGERS_QUERY_SORT_DIRECTION = SortDirection.ASC;
export const DEPARTMENT_GROUP_MANAGERS_QUERY_ORDER_KEY = 'id';
export const PLUS_ICON_VIEW_BOX = '-2 -2 24 24';
