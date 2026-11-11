import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	CERTIFICATE_ROW_HEIGHT,
	MAX_SECTION_ROWS_COUNT,
} from '@/components/UserProfile/constansts';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';

import {
	certificatesGridColumnWidths,
	RESET_FILTER_COLUMN_INDEX,
} from './constants';
import { overrideCertificatesGridStyles } from './styles';
import { UserCertificatesGridProps } from './types';

export function CertificatesGrid({
	rows,
	columns,
	pagination,
	handleSortColumnChange,
	sortColumns,
}: UserCertificatesGridProps) {
	const overrideDataGridStyles = useMemo(() => {
		const gridColumnsTemplate = getGridColumnTemplateFromColumnWidths(
			certificatesGridColumnWidths,
			true
		);

		return overrideCertificatesGridStyles(
			overrideGridStylesWithEditColumn(
				Boolean(rows.length),
				RESET_FILTER_COLUMN_INDEX,
				gridColumnsTemplate
			)
		);
	}, [rows.length]);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				rowHeight={CERTIFICATE_ROW_HEIGHT}
				rows={rows}
				columns={columns}
				sortColumns={sortColumns}
				minRowsCount={MAX_SECTION_ROWS_COUNT}
				restrictionRowHeight={CERTIFICATE_ROW_HEIGHT}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				overrideDataGridStyles={overrideDataGridStyles}
				paginationProps={pagination}
			/>
		</DataGridHoverRowWrapper>
	);
}
