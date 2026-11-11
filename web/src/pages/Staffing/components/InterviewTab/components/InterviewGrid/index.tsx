import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useCallback, useMemo } from 'react';

import { INTERVIEW_ROW_CLASSES, STATUS_CLASS_PREFIX } from '../../constants';
import { useInterviewColumns } from '../../hooks/useInterviewHeaders';
import { InterviewGridRow } from '../../types';
import { getOverrideInterviewGridStyles } from '../../utils/getOverridedInterviewGridStyles';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { DateFormat } from '@/constants/date';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { PASCAL_TO_KEBAB_REGEX } from '@/constants/staffing';
import { formatDate } from '@/helpers/formatDate';

import { InterviewGridProps } from './types';

function InterviewGridInner({
	rows,
	columnsWidths,
	sortColumns,
	options,
	paginationProps,
	filters,
	setFilters,
	handleSortColumnChange,
	onToggleExpand,
	isResetFiltersDisabled,
}: InterviewGridProps) {
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: InterviewGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const columns = useInterviewColumns({
		filters,
		setFilters,
		options,
		columnsWidths,
		onCellClick: handleCrossLinkClick,
		isResetFiltersDisabled,
	});

	const handleRowsChange = (updatedRows: InterviewGridRow[]) => {
		if (!onToggleExpand) {
			return;
		}

		const changedRow = updatedRows.find(
			(row, index) => row.expanded !== rows[index]?.expanded
		);

		if (changedRow) {
			onToggleExpand(changedRow.id);
		}
	};

	const overrideDataGridStyles = useMemo(
		() =>
			getOverrideInterviewGridStyles(
				columnsWidths,
				columns.length,
				rows.length
			),
		[columnsWidths, columns.length, rows.length]
	);

	const getRowHeight = useMemo(
		() => (row: InterviewGridRow) => row.itemRowHeight,
		[]
	);

	const rowClass = useMemo(
		() => (row: InterviewGridRow) => {
			const classes: string[] = [];

			if (!row.isMainInterview) {
				classes.push(INTERVIEW_ROW_CLASSES.stageRow);
			}

			if (row.status) {
				const statusClass = row.status
					.replace(PASCAL_TO_KEBAB_REGEX, '$1-$2')
					.toLowerCase();
				classes.push(`${STATUS_CLASS_PREFIX}${statusClass}`);
			} else {
				classes.push(INTERVIEW_ROW_CLASSES.statusWaitingForInterview);
			}

			const today = formatDate(new Date(), DateFormat.DD_MM_YYYY);

			if (row.date === today) {
				classes.push(INTERVIEW_ROW_CLASSES.dateToday);
			}

			if (!row.isActive) {
				classes.push(INTERVIEW_ROW_CLASSES.inActiveUser);
			}

			return classes.join(' ');
		},
		[]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				{...{
					sortColumns,
					overrideDataGridStyles,
					paginationProps,
				}}
				columns={columns}
				rows={rows}
				onRowsChange={handleRowsChange}
				defaultColumnOptions={{
					sortable: true,
				}}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				rowKeyGetter={(row) => row.id}
				rowHeight={getRowHeight}
				rowClass={rowClass}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const InterviewGrid = memo(InterviewGridInner);
