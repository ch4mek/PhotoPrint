import { ProjectTableColumnWidths } from '@/components/UserProfile/types';
import {
	activityLogicalOptions,
	billabilityLogicalOptions,
} from '@/constants/activityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { ProjectsTableFilters } from '@/types/projectsTableFilters';

export const projectTableColumns = {
	projectName: {
		key: 'projectName',
		sortable: true,
	},
	clientName: {
		key: 'clientName',
		sortable: true,
	},
	isActive: {
		key: 'isActive',
		sortable: true,
		logicalTextOptions: activityLogicalOptions,
	},
	billabillity: {
		key: 'isTimeOnProjectBillable',
		sortable: true,
		logicalTextOptions: billabilityLogicalOptions,
	},
	startDate: {
		key: 'userOnProjectFrom',
		sortable: true,
	},
	endDate: {
		key: 'userOnProjectTo',
		sortable: true,
	},
};

export const defaultFilterValues: ProjectsTableFilters = {
	projectName: '',
	clientName: '',
	isActive: IS_ACTIVE_DEFAULT_VALUE,
	billability: [],
	startDate: fromToDateObject,
	endDate: fromToDateObject,
};

export const columnsWidth: ProjectTableColumnWidths = {
	projectName: 150,
	clientName: 150,
	isActive: 88,
	billability: 102,
	startDate: 107,
	endDate: 101,
};

export const columnsWidthForEmployee: ProjectTableColumnWidths = {
	...columnsWidth,
	clientName: 0,
	billability: 0,
};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(defaultFilterValues).length + 2;
