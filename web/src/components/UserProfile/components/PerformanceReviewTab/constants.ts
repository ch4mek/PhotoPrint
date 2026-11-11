import { fromToDateObject } from '@/constants/defaultValues';
import { GridColumnWidth } from '@/types/grid';

import {
	UserPerformanceReviewFilters,
	UserPerformanceReviewGridColumnsNames,
	UserPerformanceReviewGridConstants,
} from './types';

export const DEFAULT_USER_PERFORMANCE_REVIEW_SORT = 'performanceReviewDate';

export const defaultUserPerformanceReviewFilters: UserPerformanceReviewFilters =
	{
		performanceReviewDate: fromToDateObject,
		agreements: '',
	};

export const userPerformanceReviewColumns: UserPerformanceReviewGridConstants<UserPerformanceReviewGridColumnsNames> =
	{
		performanceReviewDate: {
			key: 'performanceReviewDate',
		},
		agreements: {
			key: 'agreements',
		},
		files: {
			key: 'files',
		},
		links: {
			key: 'links',
		},
	};

export const userPerformanceReviewGridColumnWidths: UserPerformanceReviewGridConstants<GridColumnWidth> =
	{
		performanceReviewDate: {
			width: 116,
			fixed: true,
		},
		agreements: 300,
		files: {
			width: 180,
			fixed: true,
		},
		links: {
			width: 84,
			fixed: true,
		},
	};

export const EDIT_COLUMN_INDEX =
	Object.values(userPerformanceReviewColumns).length + 2;
