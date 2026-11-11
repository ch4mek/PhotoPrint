import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { SortColumn } from 'react-data-grid';

import {
	PerformanceReviewFilter,
	PerformanceReviewFilterSetter,
	PerformanceReviewRow,
} from '@/pages/TeamBoardUser/types';

export interface PerformanceReviewTableProps {
	rows: Array<PerformanceReviewRow>;
	setRows: Dispatch<SetStateAction<Array<PerformanceReviewRow>>>;
	sortColumns?: Array<SortColumn>;
	onSortColumnsChange?: (newSortColumns: Array<SortColumn>) => void;
	filter?: PerformanceReviewFilter;
	setFilter?: PerformanceReviewFilterSetter;
	areFiltersApplied?: boolean;
	resetFilter?: () => void;
	pagination?: GridPaginationProps;
	isLoading?: boolean;
}
