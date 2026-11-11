import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useContext, useMemo } from 'react';

import { ADD_CERTIFICATE_BUTTON_LABEL } from '../constants';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	CERTIFICATE_ROW_HEIGHT,
	MAX_SECTION_ROWS_COUNT,
} from '@/pages/TeamBoardUser/constants';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { isFullAccess } from '@/pages/TeamBoardUser/utils';

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
	handleCertificateFormToggle,
}: UserCertificatesGridProps) {
	const { accessLevel } = useContext(PersonalDataContext);

	const canAddCertificates = isFullAccess(accessLevel);

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
		<TabTableWrapper>
			{canAddCertificates && (
				<TabHeader
					onAddClick={handleCertificateFormToggle}
					buttonTitle={ADD_CERTIFICATE_BUTTON_LABEL}
				/>
			)}
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
		</TabTableWrapper>
	);
}
