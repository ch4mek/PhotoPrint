import {
	activityLogicalOptions,
	billabilityLogicalOptions,
} from '@/constants/activityStatus';

import { ProjectTableColumnWidths } from './types';
import { cellClass } from './utils';

export const HIDE_BOTTOM_BORDER_CLASS = 'hide-bottom-border';

export const USER_PROJECTS_TABLE_COLUMNS_WIDTHS: ProjectTableColumnWidths = {
	projectName: 150,
	clientName: 150,
	status: 88,
	billability: 102,
	startDate: 107,
	endDate: 101,
};

export const USER_PROJECTS_SECTION_TABLE_COLUMNS_WIDTHS: ProjectTableColumnWidths =
	{
		projectName: 200,
		clientName: 200,
		status: 0,
		billability: 93,
		startDate: 100,
		endDate: 100,
	};

export const PROJECT_TABLE_COLUMNS = {
	projectName: {
		key: 'projectName',
		name: 'Project name',
		sortable: false,
		cellClass,
	},
	clientName: {
		key: 'clientName',
		name: 'Client',
		sortable: false,
		cellClass,
	},
	status: {
		key: 'isActive',
		name: 'Status',
		sortable: false,
		cellClass,
		logicalTextOptions: activityLogicalOptions,
	},
	billabillity: {
		key: 'isTimeOnProjectBillable',
		name: 'Billability',
		sortable: false,
		cellClass,
		logicalTextOptions: billabilityLogicalOptions,
	},
	startDate: {
		key: 'userOnProjectFrom',
		name: 'Start date',
		sortable: false,
		cellClass,
	},
	endDate: {
		key: 'userOnProjectTo',
		name: 'End date',
		sortable: false,
		cellClass,
	},
};
