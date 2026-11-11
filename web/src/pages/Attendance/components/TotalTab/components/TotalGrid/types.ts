import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import {
	AttendanceTotalFilters,
	AttendanceTotalGridRow,
	AttendanceTotalOptions,
	AttendanceTotalSetFilters,
} from '../../types';

import { GridColumnWidthsConfiguration } from '@/types/grid';

export interface TotalGridInnerProps {
	rows: AttendanceTotalGridRow[];
	options: AttendanceTotalOptions;
	columnsWidths: GridColumnWidthsConfiguration;
	sortColumns: SortColumn[];
	filters: AttendanceTotalFilters;
	setFilters: AttendanceTotalSetFilters;
	initialFilters: AttendanceTotalFilters;
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	paginationProps?: GridPaginationProps;
}
