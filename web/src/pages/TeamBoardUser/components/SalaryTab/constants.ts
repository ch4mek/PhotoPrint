import { SalaryTableFilters } from '../SalaryTable/types';

import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';

export const DEFAULT_SALARY_FILTER_VALUES: SalaryTableFilters = {
	date: fromToDateObject,
	salaryUsd: fromToCurrencyObject,
};
