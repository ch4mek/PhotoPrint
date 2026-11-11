import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { Dispatch, SetStateAction } from 'react';
import { SortColumn } from 'react-data-grid';

import {
	DevelopmentPlanFilterHandlers,
	DevelopmentPlanFilters,
	DevelopmentPlanRow,
	DevelopmentPlanTabTableColumns,
} from '@/types/developmentPlan';
import { TeamboardTableMode } from '@/types/teamboard';

export interface DevelopmentPlanTableProps {
	mode: TeamboardTableMode;
	rows: Array<DevelopmentPlanRow>;
	setRows: Dispatch<SetStateAction<Array<DevelopmentPlanRow>>>;
	filter?: DevelopmentPlanFilters;
	setFilter?: DevelopmentPlanFilterHandlers;
	sortColumns?: Array<SortColumn>;
	onSortColumnsChange?: (newSortColumns: Array<SortColumn>) => void;
	areFiltersApplied?: boolean;
	pagination?: GridPaginationProps;
	isLoading?: boolean;
}

export type DevelopmentPlanFormSectionTableColumns = Pick<
	DevelopmentPlanTabTableColumns,
	'startDate' | 'deadlineDate' | 'summary'
>;
