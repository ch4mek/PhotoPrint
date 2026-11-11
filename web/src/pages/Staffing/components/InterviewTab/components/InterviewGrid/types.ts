import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import {
	InterviewFilters,
	InterviewGridRow,
	InterviewOptions,
	InterviewSetFilters,
} from '../../types';

import { GridColumnWidthsConfiguration } from '@/types/grid';

export interface InterviewGridProps {
	rows: InterviewGridRow[];
	options: InterviewOptions;
	columnsWidths: GridColumnWidthsConfiguration;
	sortColumns: SortColumn[];
	filters: InterviewFilters;
	setFilters: InterviewSetFilters;

	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	paginationProps?: GridPaginationProps;
	onToggleExpand?: (id: number) => void;
	isResetFiltersDisabled?: boolean;
}
