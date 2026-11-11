import { fromToDateObject } from '@/constants/defaultValues';
import { GridColumnWidthsConfiguration } from '@/types/grid';

import {
	UserDevelopmentPlanFilters,
	UserDevelopmentPlanTableColumns,
} from './types';

export const defaultDevelopmentPlanFilterValues: UserDevelopmentPlanFilters = {
	startDate: fromToDateObject,
	endDate: fromToDateObject,
	deadlineDate: fromToDateObject,
	summary: '',
	status: [],
};

export const developmentPlanTabTableColumnsWidths: GridColumnWidthsConfiguration<UserDevelopmentPlanTableColumns> =
	{
		status: {
			width: 115,
			fixed: true,
		},
		deadlineDate: {
			width: 150,
			fixed: true,
		},
		summary: 300,
		startDate: {
			width: 115,
			fixed: true,
		},
		endDate: {
			width: 150,
			fixed: true,
		},
	};

export const developmentPlanTableKeys = {
	status: 'status',
	deadlineDate: 'deadlineDate',
	summary: 'summary',
	startDate: 'startDate',
	endDate: 'endDate',
};

export const EDIT_COLUMN_INDEX =
	Object.values(developmentPlanTabTableColumnsWidths).length + 2;
