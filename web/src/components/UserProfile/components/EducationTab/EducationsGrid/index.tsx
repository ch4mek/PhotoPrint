import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import { CLEAR_FILTERS_COLUMN_INDEX, columnWidths } from './constants';
import { EducationsGridProps } from './types';

export function EducationsGrid({
	rows,
	columns,
	sortColumns,
	handleSortColumnChange,
	pagination,
}: EducationsGridProps) {
	const overrideEducationsGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			columnWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			Boolean(rows.length),
			CLEAR_FILTERS_COLUMN_INDEX,
			gridColumnTemplate
		);
	}, [rows.length]);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				columns={columns}
				rows={rows}
				overrideDataGridStyles={overrideEducationsGridStyles}
				renderers={noRowsGridFallback}
				enableVirtualization
				sortColumns={sortColumns}
				onSortColumnsChange={handleSortColumnChange}
				paginationProps={pagination}
			/>
		</DataGridHoverRowWrapper>
	);
}
