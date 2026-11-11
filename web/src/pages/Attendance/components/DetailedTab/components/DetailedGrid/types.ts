import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import {
	AttendanceDetailedFilters,
	AttendanceDetailedGridRow,
	AttendanceDetailedOptions,
	AttendanceDetailedSetFilters,
} from '@/pages/Attendance/components/DetailedTab/types';
import { GridColumnWidthsConfiguration } from '@/types/grid';

export interface DetailedGridInnerProps {
	rows: AttendanceDetailedGridRow[];
	options: AttendanceDetailedOptions;
	columnsWidths: GridColumnWidthsConfiguration;
	sortColumns: SortColumn[];
	filters: AttendanceDetailedFilters;
	setFilters: AttendanceDetailedSetFilters;
	initialFilters: AttendanceDetailedFilters;
	handleSortColumnChange:
		| ((sortColumns: SortColumn[]) => void)
		| undefined
		| null;
	paginationProps?: GridPaginationProps;
}
