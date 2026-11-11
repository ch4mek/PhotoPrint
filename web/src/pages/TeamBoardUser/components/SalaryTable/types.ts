import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { FromToProps, GridCellDefaultProps } from '@/components/Grid';
import { TeamboardTableMode } from '@/types/teamboard';

export interface SalaryTableProps {
	pagination?: GridPaginationProps;
	rows: Array<SalaryRow>;
	mode: TeamboardTableMode;
	areFiltersApplied?: boolean;
	onFiltersReset?: () => void;
	filters?: SalaryTableFilters;
	filtersSetter?: SalaryFilterSetter;
	isLoading?: boolean;
	sortColumns?: Array<SortColumn>;
	onSortColumnsChange?: (newSortColumns: Array<SortColumn>) => void;
}

export interface SalaryRow extends GridCellDefaultProps {
	id: number;
	salaryFrom: string;
	salaryUsd: string;
	isCurrent?: boolean;
}

export interface SalaryTableFilters {
	date: FromToProps<Date>;
	salaryUsd: FromToProps<string>;
}

export enum SalaryTableFiltersKeys {
	date = 'date',
	salaryUsd = 'salaryUsd',
	set = 'set',
}

export interface SalaryFilterSetter {
	date: (newDate: FromToProps<Date>) => void;
	salaryUsd: (newSalary: FromToProps<string>) => void;
	set: (newFilters: SalaryTableFilters) => void;
}

export interface SalaryAction {
	type: SalaryTableFiltersKeys;
	payload?: FromToProps<Date> | FromToProps<string> | SalaryTableFilters;
}
