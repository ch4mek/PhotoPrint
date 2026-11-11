import { css, Theme } from '@emotion/react';
import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useMemo } from 'react';

import {
	getRowClass,
	getRowKey,
	INACTIVE_USER_ROW_CLASS,
} from '../../constants';
import { useAttendanceHeaders } from '../../hooks/useAttendanceHeaders';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_DISABLED_GRID_CELL_OPACITY } from '@/constants/opacities';

import { DetailedGridInnerProps } from './types';

function DetailedGridInner({
	rows,
	columnsWidths,
	sortColumns,
	paginationProps,
	options,
	filters,
	setFilters,
	initialFilters,
	handleSortColumnChange,
}: DetailedGridInnerProps) {
	const columns = useAttendanceHeaders({
		idFilter: filters.id,
		userFilter: filters.user,
		departmentsFilter: filters.departments,
		groupFilter: filters.group,
		directionFilter: filters.direction,
		doorFilter: filters.door,
		locationFilter: filters.location,
		dateFilter: filters.date,
		timeFilter: filters.time,
		setFilters,
		initialFilters,
		departamentsOptions: options.departamentsOptions,
		userOptions: options.userOptions,
		groupOptions: options.groupOptions,
	});

	const overrideDataGridStyles = useMemo(
		() => (gridTheme: Theme) => {
			const getGridColumnTemplate = getGridColumnTemplateFromColumnWidths(
				columnsWidths,
				true,
				undefined,
				true
			);

			const clearFiltersColumnIndex = columns.length + 1;

			const baseStyles = overrideGridStylesWithEditColumn(
				!!rows.length,
				clearFiltersColumnIndex,
				getGridColumnTemplate
			);

			return css`
				${baseStyles(gridTheme)}

				.${INACTIVE_USER_ROW_CLASS} {
					pointer-events: none;

					& .rdg-cell > div {
						opacity: ${DEFAULT_DISABLED_GRID_CELL_OPACITY};
					}
				}
			`;
		},
		[rows.length, columnsWidths, columns.length]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				{...{
					columns,
					sortColumns,
					rows,
					paginationProps,
					overrideDataGridStyles,
				}}
				defaultColumnOptions={{
					sortable: true,
				}}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				rowKeyGetter={getRowKey}
				rowClass={getRowClass}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const DetailedGrid = memo(DetailedGridInner);
