import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	displayMinimalGridRowsProp,
} from '@/components/Grid/constants';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import {
	PAYSLIPS_SECTION_EDIT_COLUMN_INDEX,
	PAYSLIPS_TABLE_COLUMNS_WIDTHS,
} from './constants';
import { usePayslipsTableColumns } from './hooks/usePayslipsTableColumns';
import { PayslipsTableProps } from './types';
import { getRowKey } from './util/getRowKey';
import { overridePayslipsGridStyles } from './util/overridePayslipsGridStyles';

export function PayslipsTable({
	pagination,
	rows,
	areFiltersApplied,
	filters,
	filtersSetter,
	onFiltersReset,
	onSortColumnChange,
	sortColumns,
}: PayslipsTableProps) {
	const columns = usePayslipsTableColumns(
		filters,
		filtersSetter,
		areFiltersApplied,
		onFiltersReset
	);

	const overrideDataGridStyles = useMemo(
		() =>
			overridePayslipsGridStyles(
				pagination ? !!rows.length : false,
				PAYSLIPS_SECTION_EDIT_COLUMN_INDEX,
				getGridColumnTemplateFromColumnWidths(
					PAYSLIPS_TABLE_COLUMNS_WIDTHS,
					true
				)
			),
		[rows.length, pagination]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				{...displayMinimalGridRowsProp}
				enableVirtualization
				columns={columns}
				sortColumns={sortColumns}
				overrideDataGridStyles={overrideDataGridStyles}
				onSortColumnsChange={onSortColumnChange}
				rows={rows}
				rowKeyGetter={getRowKey}
				paginationProps={pagination}
				scrollToTopOnRowsChange={false}
				renderers={noRowsGridFallback}
			/>
		</DataGridHoverRowWrapper>
	);
}
