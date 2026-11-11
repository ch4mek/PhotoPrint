import { css, Theme, useTheme } from '@emotion/react';
import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useMemo } from 'react';

import {
	getRowClass,
	getRowKey,
	INACTIVE_USER_ROW_CLASS,
} from '../../constants';
import { useAttendanceHeaders } from '../../hooks/useAttendanceHeaders';
import { AttendanceTotalGridRow } from '../../types';
import { calculateTotalTime } from '../../utils/calculateTotalTime';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { DEFAULT_DISABLED_GRID_CELL_OPACITY } from '@/constants/opacities';
import { zIndexes } from '@/constants/zIndexes';

import {
	ATTENDANCE_TOTAL_SUMMARY_ROW_HEIGHT,
	ATTENDANCE_TOTAL_SUMMARY_ROW_ID,
} from './constants';
import { TotalGridInnerProps } from './types';

function TotalGridInner({
	rows,
	columnsWidths,
	sortColumns,
	paginationProps,
	options,
	filters,
	setFilters,
	initialFilters,
	handleSortColumnChange,
}: TotalGridInnerProps) {
	const theme = useTheme();

	const columns = useAttendanceHeaders({
		idFilter: filters.userId,
		userFilter: filters.user,
		departmentsFilter: filters.departments,
		groupFilter: filters.group,
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
				true
			);

			const clearFiltersColumnIndex = columns.length + 1;

			const baseStyles = overrideGridStylesWithEditColumn(
				!!rows.length,
				clearFiltersColumnIndex,
				getGridColumnTemplate
			);

			const EDIT_COLUMN_OFFSET = 2;

			const visibleColumnsCount = Object.values(columnsWidths).filter(
				(columnWidth) => {
					if (typeof columnWidth === 'number') {
						return columnWidth > 0;
					}

					if (typeof columnWidth === 'string') {
						const numValue = parseFloat(columnWidth);

						return !Number.isNaN(numValue) && numValue > 0;
					}

					return columnWidth?.width !== null && Number(columnWidth?.width) > 0;
				}
			).length;

			const summaryFromColumn = visibleColumnsCount;
			const summaryToColumn = visibleColumnsCount + EDIT_COLUMN_OFFSET;

			return css`
				${baseStyles(gridTheme)}

				.rdg-summary-row {
					.rdg-cell {
						position: sticky !important;
						z-index: ${zIndexes.logOutPopUp} !important;
						background-color: ${theme.palette['background-base-2']};
					}

					.rdg-cell:not(:last-of-type) {
						grid-column: 1 / -1 !important;
					}
					.rdg-cell:last-of-type {
						grid-column: ${summaryFromColumn} / ${summaryToColumn + 1} !important;
					}
				}

				.${INACTIVE_USER_ROW_CLASS} {
					pointer-events: none;

					& .rdg-cell > div {
						opacity: ${DEFAULT_DISABLED_GRID_CELL_OPACITY};
					}
				}
			`;
		},
		[rows.length, columnsWidths, columns.length, theme]
	);

	const summaryRow = useMemo(() => {
		const totalTime = calculateTotalTime(rows);

		const attendanceSummaryRow: AttendanceTotalGridRow = {
			id: ATTENDANCE_TOTAL_SUMMARY_ROW_ID,
			userId: '',
			user: '',
			group: null,
			department: '',
			date: '',
			time: '',
			totalTime,
			isUserActive: true,
		};

		return attendanceSummaryRow;
	}, [rows]);

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
				bottomSummaryRows={[summaryRow]}
				summaryRowHeight={
					rows.length === 0 ? 0 : ATTENDANCE_TOTAL_SUMMARY_ROW_HEIGHT
				}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const TotalGrid = memo(TotalGridInner);
