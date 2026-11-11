import { defaultIdColumnWidth } from '@/components/Grid/constants';
import {
	IS_ACTIVE_DEFAULT_VALUE,
	TRACKED_TIME_DEFAULT_VALUE,
} from '@/pages/constants';

export const PROJECT_NAME_DEFAULT_VALUE = '';
export const CLIENT_NAME_DEFAULT_VALUE = '';
export const PROJECT_ID_DEFAULT_VALUE = '';

export const ACCESS_USERS_CELL_CLASS = 'access-users';

export const PROJECT_COLUMNS_WIDTH = {
	id: defaultIdColumnWidth,
	project: 200,
	client: 200,
	factTracked: {
		width: 121,
		maxWidth: 182,
	},
	billableTracked: {
		width: 137,
		maxWidth: 182,
	},
	status: {
		width: 88,
		maxWidth: 182,
	},
	private: {
		width: 91,
		fixed: true,
	},
	billable: {
		width: 103,
		fixed: true,
	},
	accessUsers: {
		width: 250,
		fixed: true,
	},
};

export const getProjectColumnsWidths = (displayTrackedColumn: boolean) => {
	if (displayTrackedColumn) {
		return PROJECT_COLUMNS_WIDTH;
	}

	return { ...PROJECT_COLUMNS_WIDTH, factTracked: null, billableTracked: null };
};

export const PROJECT_FILTERS_INITIAL_STATE = {
	idFilter: PROJECT_ID_DEFAULT_VALUE,
	projectFilter: PROJECT_NAME_DEFAULT_VALUE,
	clientFilter: CLIENT_NAME_DEFAULT_VALUE,
	factTrackedTimeFilter: TRACKED_TIME_DEFAULT_VALUE,
	billableTrackedTimeFilter: TRACKED_TIME_DEFAULT_VALUE,
	statusFilter: IS_ACTIVE_DEFAULT_VALUE,
	billableFilter: [],
	publicFilter: [],
};
