import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import {
	ExpensesFilterOptions,
	ExpensesFilters,
	ExpensesGridRow,
	ExpensesSetFilters,
} from '../../types';

import { HandleRowClick } from '@/types/grid';

export interface ExpensesGridProps {
	rows: ExpensesGridRow[];
	summary: number[];
	filters: ExpensesFilters;
	setFilters: ExpensesSetFilters;
	isResetFiltersDisabled: boolean;
	sortColumns: SortColumn[];
	handleSortColumnChange: (sortColumns: SortColumn[]) => void;
	paginationProps?: GridPaginationProps;
	handleEdit: HandleRowClick<ExpensesGridRow>;
	handleDelete: HandleRowClick<ExpensesGridRow>;
	filterOptions: ExpensesFilterOptions;
}
