import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';

import { RequestsColumnsWidths } from '../../constants';
import { useVacationRequestsColumns } from '../../hooks/useVacationRequestsColumns';
import { RequestGridRow } from '../../types';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { DATA_GRID_ROW_HEIGHT_WITH_TAG } from '@/constants/commonStyles';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';

import { RESET_FILTER_COLUMN_INDEX } from './constants';
import { RequestGridProps } from './types';

export function RequestsGrid({
	rows,
	sortColumns,
	handleSortColumnChange,
	paginationProps,
	filters,
	setFilters,
	options,
}: RequestGridProps) {
	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrossLinkClick = useCallback(
		(row: RequestGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const enableCellClick = useCallback(
		(row: RequestGridRow) => canOpenProfile(row.userId),
		[canOpenProfile]
	);

	const columns = useVacationRequestsColumns(
		filters,
		setFilters,
		options,
		enableCellClick,
		handleCrossLinkClick
	);

	const overrideRequestsGridStyle = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			RequestsColumnsWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!rows.length,
			RESET_FILTER_COLUMN_INDEX,
			gridColumnTemplate
		);
	}, [rows.length]);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				columns={columns}
				rows={rows || []}
				enableVirtualization
				rowHeight={DATA_GRID_ROW_HEIGHT_WITH_TAG}
				scrollToTopOnRowsChange
				sortColumns={sortColumns}
				onSortColumnsChange={handleSortColumnChange}
				overrideDataGridStyles={overrideRequestsGridStyle}
				paginationProps={paginationProps}
				renderers={noRowsGridFallback}
			/>
		</DataGridHoverRowWrapper>
	);
}
