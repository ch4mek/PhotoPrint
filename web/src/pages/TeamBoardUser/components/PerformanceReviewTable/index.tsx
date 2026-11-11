import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { useDynamicRowsHeight } from '@/components/Grid/hooks/useDynamicRowsHeight';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { HeightsContext } from '@/context/CellHeightsContext';
import { getInfoDateTagContent } from '@/helpers/getInfoDateTagContent';
import {
	PERFORMANCE_REVIEW_TAB_TABLE_WIDTHS,
	PERFORMANCE_EDIT_COLUMN_INDEX,
	PERFORMANCE_REVIEW_LINE_HEIGHT,
} from '@/pages/TeamBoardUser/components/PerformanceReviewTable/constants';
import { usePerformanceReviewColumns } from '@/pages/TeamBoardUser/components/PerformanceReviewTable/hooks/usePerformanceReviewColumns';
import { PerformanceReviewTableProps } from '@/pages/TeamBoardUser/components/PerformanceReviewTable/types';
import { overridePerformanceReviewGridStyles } from '@/pages/TeamBoardUser/components/PerformanceReviewTable/utils';

export function PerformanceReviewTable({
	rows,
	setRows,
	sortColumns,
	onSortColumnsChange,
	filter,
	setFilter,
	areFiltersApplied,
	resetFilter,
	pagination,
	isLoading,
}: PerformanceReviewTableProps) {
	const infoDateTagContent = getInfoDateTagContent();

	const columns = usePerformanceReviewColumns(
		filter,
		setFilter,
		areFiltersApplied,
		resetFilter,
		infoDateTagContent
	);

	const { handleUpdateRows, getHeight, heightsMap } = useDynamicRowsHeight({
		setRows,
		lineHeight: PERFORMANCE_REVIEW_LINE_HEIGHT,
	});

	const overrideStyles = useMemo(
		() =>
			overridePerformanceReviewGridStyles(
				pagination ? !isLoading && !!rows.length : false,
				PERFORMANCE_EDIT_COLUMN_INDEX,
				getGridColumnTemplateFromColumnWidths(
					PERFORMANCE_REVIEW_TAB_TABLE_WIDTHS,
					true
				)
			),
		[pagination, rows.length, isLoading]
	);

	return (
		<HeightsContext.Provider value={heightsMap}>
			<DataGrid
				columns={columns}
				rows={rows}
				sortColumns={sortColumns}
				onSortColumnsChange={onSortColumnsChange}
				overrideDataGridStyles={overrideStyles}
				onRowsChange={handleUpdateRows}
				rowHeight={getHeight}
				scrollToTopOnRowsChange={false}
				paginationProps={!isLoading ? pagination : undefined}
				renderers={noRowsGridFallback}
				enableVirtualization={false}
			/>
		</HeightsContext.Provider>
	);
}
