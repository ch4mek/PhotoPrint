import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { SortColumn } from 'react-data-grid';

import { FeedbackFilter, FeedbackFilterSetter, FeedbackRow } from '../../types';

import { TeamboardTableMode } from '@/types/teamboard';

export interface FeedbackTableProps {
	mode: TeamboardTableMode;
	rows: Array<FeedbackRow>;
	setRows: Dispatch<SetStateAction<Array<FeedbackRow>>>;
	filter?: FeedbackFilter;
	setFilter?: FeedbackFilterSetter;
	sortColumns?: Array<SortColumn>;
	onSortColumnsChange?: (newSortColumns: Array<SortColumn>) => void;
	areFiltersApplied?: boolean;
	resetFilter?: () => void;
	pagination?: GridPaginationProps;
	isLoading?: boolean;
}
