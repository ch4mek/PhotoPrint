import { billabilityLogicalOptions } from '@/constants/activityStatus';

import { ProjectSectionColumnWidths } from './types';

export const projectSectionColumns = {
	projectName: {
		key: 'projectName',
		sortable: false,
	},
	clientName: {
		key: 'clientName',
		sortable: false,
	},
	billabillity: {
		key: 'isTimeOnProjectBillable',
		sortable: false,
		logicalTextOptions: billabilityLogicalOptions,
	},
	startDate: {
		key: 'userOnProjectFrom',
		sortable: false,
	},
	endDate: {
		key: 'userOnProjectTo',
		sortable: false,
	},
};

export const columnsWidth: ProjectSectionColumnWidths = {
	projectName: 200,
	clientName: 200,
	billability: 93,
	startDate: 100,
	endDate: 100,
};

export const columnsWidthForEmployee: ProjectSectionColumnWidths = {
	...columnsWidth,
	clientName: 0,
	billability: 0,
};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(projectSectionColumns).length + 2;
