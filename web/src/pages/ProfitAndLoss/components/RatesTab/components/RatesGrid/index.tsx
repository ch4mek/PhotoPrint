import { useTheme } from '@emotion/react';
import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { memo, useCallback, useMemo } from 'react';
import { RowsChangeData } from 'react-data-grid';

import { getRatesGridColumnsWidths } from '../../constants';
import { useRatesColumns } from '../../hooks/useRatesColumns';
import { RatesGridRow } from '../../types';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { ProfitAndLossGridProps } from '@/pages/ProfitAndLoss/types';
import { getOverrideRatesGridStyles } from '@/pages/ProfitAndLoss/utils/getOverrideRatesGridStyles';
import { getRatesRowClass } from '@/pages/ProfitAndLoss/utils/getRatesRowClass';
import { getRowKey } from '@/pages/ProfitAndLoss/utils/getRowKey';

function RatesGridInner({
	rows,
	filters,
	setFilters,
	isResetFiltersDisabled,
	sortColumns,
	filterOptions,
	handleSortColumnChange,
	paginationProps,
	onToggleExpand,
}: ProfitAndLossGridProps<RatesGridRow>) {
	const theme = useTheme();
	const isDesktopWidth = useHasScreenWidth(theme.breakpoints.desktopL);

	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrosslinkClick = useCallback(
		(row: RatesGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const columns = useRatesColumns({
		filters,
		setFilters,
		isResetFiltersDisabled,
		filterOptions,
		isDesktopWidth,
		onCellClick: handleCrosslinkClick,
	});

	const handleRowsChange = useCallback(
		(
			updatedRows: RatesGridRow[],
			{ indexes }: RowsChangeData<RatesGridRow>
		) => {
			const changedRow = updatedRows[indexes[0]];

			if (changedRow) {
				onToggleExpand(changedRow.id);
			}
		},
		[onToggleExpand]
	);

	const getRowHeight = useMemo(
		() => (row: RatesGridRow) => row.itemRowHeight,
		[]
	);

	const overrideRatesGridStyles = useMemo(
		() =>
			getOverrideRatesGridStyles(
				getRatesGridColumnsWidths(isDesktopWidth),
				rows.length
			),
		[isDesktopWidth, rows.length]
	);

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				onRowsChange={handleRowsChange}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				rowKeyGetter={getRowKey}
				rowHeight={getRowHeight}
				rowClass={getRatesRowClass}
				overrideDataGridStyles={overrideRatesGridStyles}
				{...{ columns, rows, sortColumns, paginationProps }}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const RatesGrid = memo(RatesGridInner);
