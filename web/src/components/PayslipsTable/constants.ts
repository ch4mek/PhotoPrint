import { fromToDateObject } from '@/constants/defaultValues';

import { PayslipsTableFilters } from './types';

export const PAYSLIPS_COLUMN_KEYS = {
	reportDate: 'reportDate',
	fileName: 'fileName',
};

export const PAYSLIPS_TABLE_COLUMNS_WIDTHS = {
	reportDate: 113,
	fileName: 113,
};

export const PAYSLIPS_SECTION_EDIT_COLUMN_INDEX = 4;
export const PAYSLIPS_COLUMN_SPAN = 2;

export const DEFAULT_PAYSLIPS_FILTER_VALUES: PayslipsTableFilters = {
	reportDate: fromToDateObject,
};
