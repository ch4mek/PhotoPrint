import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { overrideTeamboardSectionTableStyles } from '../../utils';
import { MAX_VISIBLE_ROWS_COUNT } from '../SummaryTabLayout/constants';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { TeamboardTableMode } from '@/types/teamboard';

import { USER_PROJECTS_TABLE_COLUMNS_WIDTHS } from './constants';
import { useProjectTableColumns } from './hooks/useProjectTableColumns';
import { ProjectsTableProps } from './types';
import {
	getEditColumnIndex,
	getRowKey,
	getTeamboardTableColumnWidths,
} from './utils';

export function ProjectsTable({
	rows,
	displayFilters = false,
	filters,
	filterSetters,
	onFiltersReset,
	areFiltersApplied,
	pagination,
	isUserTab = false,
	mode,
	isColumnsSortable = true,
	sortColumn,
	handleSortColumnChange,
	isLoading,
}: ProjectsTableProps) {
	const columnWidths = useMemo(
		() =>
			isUserTab
				? USER_PROJECTS_TABLE_COLUMNS_WIDTHS
				: getTeamboardTableColumnWidths(mode),
		[isUserTab, mode]
	);

	const columns = useProjectTableColumns(
		displayFilters,
		mode,
		columnWidths,
		filters,
		filterSetters,
		areFiltersApplied,
		onFiltersReset,
		isColumnsSortable
	);

	const overrideProjectsGridStyles = useMemo(
		() =>
			overrideTeamboardSectionTableStyles(
				pagination ? !isLoading && !!rows.length : false,
				mode,
				getEditColumnIndex(columnWidths),
				getGridColumnTemplateFromColumnWidths(columnWidths, true)
			),
		[columnWidths, mode, pagination, rows.length, isLoading]
	);

	const isSection = mode === TeamboardTableMode.Section;

	return (
		<DataGridHoverRowWrapper
			defineHeight={false}
			flexGrow={mode === TeamboardTableMode.Tab}
		>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization
				columns={columns}
				rows={rows}
				rowKeyGetter={getRowKey}
				renderers={noRowsGridFallback}
				overrideDataGridStyles={overrideProjectsGridStyles}
				scrollToTopOnRowsChange={false}
				paginationProps={pagination}
				sortColumns={sortColumn}
				onSortColumnsChange={handleSortColumnChange}
				{...(isSection && displayMinimalGridRowsProp)}
				{...(isSection && {
					maxRowsCount: MAX_VISIBLE_ROWS_COUNT,
				})}
			/>
		</DataGridHoverRowWrapper>
	);
}
