import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import { GridColumnWidth } from '@/types/grid';

import { UserSalaryFilters, UserSalaryGridConstants } from './types';

export const defaultUserSalaryFilters: UserSalaryFilters = {
	salaryFrom: fromToDateObject,
	salaryUsd: fromToCurrencyObject,
};

export const userSalaryGridColumnsKeys = {
	salaryFrom: 'salaryFrom',
	salaryUsd: 'salaryUsd',
};

export const useSalaryGridColumnWidth: UserSalaryGridConstants<GridColumnWidth> =
	{
		salaryFrom: 100,
		salaryUsd: 100,
	};

export const RESET_FILTER_COLUMN_INDEX =
	Object.values(defaultUserSalaryFilters).length + 2;
