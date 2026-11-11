import { UserAccessFilters } from '../types';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { activityLogicalOptions } from '@/constants/activityStatus';
import { BillabilityStatus } from '@/constants/billabilityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';
import {
	GRID_FROZEN_EDIT_COLUMN_DIFF,
	IS_ACTIVE_DEFAULT_VALUE,
} from '@/pages/constants';

export const USER_ACCESS_TAB_GRID_COLUMNS_WIDTH = {
	userFullName: 230,
	isActive: 100,
	isTimeOnProjectBillable: 124,
	userOnProjectFrom: 124,
	userOnProjectTo: 124,
};

export const USER_ACCESS_TAB_GRID_COLUMNS = {
	userFullName: {
		key: 'userFullName',
		name: 'User name',
	},
	isActive: {
		key: 'isActive',
		name: 'Status',
		logicalTextOptions: activityLogicalOptions,
	},
	isTimeOnProjectBillable: {
		key: 'isTimeOnProjectBillable',
		name: 'Billability',
	},
	userOnProjectFrom: {
		key: 'userOnProjectFrom',
		name: 'Start date',
	},
	userOnProjectTo: {
		key: 'userOnProjectTo',
		name: 'End date',
	},
};

export const USER_ACCESS_TAB_GRID_COLUMS_AMOUNT =
	Object.keys(USER_ACCESS_TAB_GRID_COLUMNS_WIDTH).length +
	GRID_FROZEN_EDIT_COLUMN_DIFF;

export const DEFAULT_USERS_ACCESS_FILTERS: UserAccessFilters = {
	userName: '',
	billability: [],
	status: IS_ACTIVE_DEFAULT_VALUE,
	startDate: fromToDateObject,
	endDate: fromToDateObject,
};

export const billabilityOptions: Array<SelectionModalOption> = [
	convertConstantToSelectionModalOption(BillabilityStatus.Billable),
	convertConstantToSelectionModalOption(BillabilityStatus.NonBillable),
];
