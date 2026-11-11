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

import { SALARY_TABLE_COLUMNS_WIDTHS } from './constants';
import { useSalaryTableColumns } from './hooks/useSalaryTableColumns';
import { SalaryTableProps } from './types';
import { getRowKey } from './utils';

const SALARY_SECTION_EDIT_COLUMN_INDEX = 4;

export function SalaryTable({
	rows,
	pagination,
	mode,
	filters,
	filtersSetter,
	onFiltersReset,
	areFiltersApplied,
	isLoading,
	sortColumns,
	onSortColumnsChange,
}: SalaryTableProps) {
	const columns = useSalaryTableColumns(
		mode,
		filters,
		areFiltersApplied,
		filtersSetter,
		onFiltersReset
	);

	const overrideProjectsGridStyles = useMemo(
		() =>
			overrideTeamboardSectionTableStyles(
				pagination ? !!rows.length : false,
				mode,
				SALARY_SECTION_EDIT_COLUMN_INDEX,
				getGridColumnTemplateFromColumnWidths(SALARY_TABLE_COLUMNS_WIDTHS, true)
			),
		[rows.length, mode, pagination]
	);

	const isSection = mode === TeamboardTableMode.Section;
	const isPopup = mode === TeamboardTableMode.PopUp;

	return (
		<DataGridHoverRowWrapper
			defineHeight={false}
			flexGrow={mode === TeamboardTableMode.Tab}
		>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization
				columns={columns}
				sortColumns={sortColumns}
				onSortColumnsChange={onSortColumnsChange}
				rows={rows}
				rowKeyGetter={getRowKey}
				overrideDataGridStyles={overrideProjectsGridStyles}
				paginationProps={isLoading ? undefined : pagination}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				{...((isSection || isPopup) && displayMinimalGridRowsProp)}
				{...((isSection || isPopup) && {
					maxRowsCount: MAX_VISIBLE_ROWS_COUNT,
				})}
			/>
		</DataGridHoverRowWrapper>
	);
}
