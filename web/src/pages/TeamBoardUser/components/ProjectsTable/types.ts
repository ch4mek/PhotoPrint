import { GridPaginationProps } from '@mms/mms-ui-library/components/Grids';
import { SortColumn } from 'react-data-grid';

import { ProjectRow } from '../../types';

import {
	ProjectsTableFilters,
	ProjectTableFilterSetter,
} from '@/types/projectsTableFilters';
import { TeamboardTableMode } from '@/types/teamboard';

export interface ProjectTableColumnWidths {
	[key: string]: number;
	projectName: number;
	clientName: number;
	status: number;
	billability: number;
	startDate: number;
	endDate: number;
}

export interface ProjectsTableProps {
	rows: Array<ProjectRow>;
	isLoading: boolean;
	displayFilters?: boolean;
	filters?: ProjectsTableFilters;
	filterSetters?: ProjectTableFilterSetter;
	areFiltersApplied?: boolean;
	onFiltersReset?: () => void;
	pagination?: GridPaginationProps;
	isUserTab?: boolean;
	mode: TeamboardTableMode;
	isColumnsSortable?: boolean;
	sortColumn?: SortColumn[];
	handleSortColumnChange?: (newSortColumns: SortColumn[]) => void;
}
