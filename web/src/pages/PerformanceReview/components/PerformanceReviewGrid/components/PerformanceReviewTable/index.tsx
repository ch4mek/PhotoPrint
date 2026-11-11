import { DataGrid } from '@mms/mms-ui-library/components/Grids';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { PerformanceReviewTableProps } from './types';

export function PerformanceReviewTable({
	dataGridColumns,
	sortColumns,
	dataGridRows,
	overrideGridStyles,
	gridPagination,
	handleSortColumnChange,
}: PerformanceReviewTableProps) {
	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				columns={dataGridColumns}
				sortColumns={sortColumns}
				rows={dataGridRows}
				overrideDataGridStyles={overrideGridStyles}
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				paginationProps={gridPagination}
				renderers={noRowsGridFallback}
				onSortColumnsChange={handleSortColumnChange}
				scrollToTopOnRowsChange={false}
				enableVirtualization={false}
			/>
		</DataGridHoverRowWrapper>
	);
}
