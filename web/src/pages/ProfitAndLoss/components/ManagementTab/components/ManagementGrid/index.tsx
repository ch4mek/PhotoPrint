import { useTheme } from '@emotion/react';
import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { memo, useCallback, useMemo } from 'react';

import {
	summaryRowInitialValues,
	MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT,
} from '../../constants';
import { useManagementColumns } from '../../hooks/useManagementColumns';
import { ManagementGridProps, ManagementGridRow } from '../../types';
import { getOverrideManagementGridStyles } from '../../utils';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { getRatesRowClass } from '@/pages/ProfitAndLoss/utils/getRatesRowClass';
import { getRowKey } from '@/pages/ProfitAndLoss/utils/getRowKey';

function ManagementGridInner({
	rows,
	filters,
	setFilters,
	isResetFiltersDisabled,
	sortColumns,
	filterOptions,
	handleSortColumnChange,
	paginationProps,
	onToggleExpand,
	summary,
}: ManagementGridProps) {
	const theme = useTheme();
	const isDesktopWidth = useHasScreenWidth(theme.breakpoints.desktop);

	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrosslinkClick = useCallback(
		(row: ManagementGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const columns = useManagementColumns({
		filters,
		setFilters,
		isResetFiltersDisabled,
		filterOptions,
		isDesktopWidth,
		onCellClick: handleCrosslinkClick,
	});

	const handleRowsChange = useCallback(
		(updatedRows: ManagementGridRow[]) => {
			const changedRow = updatedRows.find(
				(row) => row.expanded !== rows.find(({ id }) => id === row.id)?.expanded
			);

			if (changedRow) {
				onToggleExpand(changedRow.id);
			}
		},
		[onToggleExpand, rows]
	);

	const getRowHeight = useMemo(
		() => (row: ManagementGridRow) => row.itemRowHeight,
		[]
	);

	const overrideManagementGridStyles = useMemo(
		() => getOverrideManagementGridStyles(isDesktopWidth, rows.length),
		[isDesktopWidth, rows.length]
	);

	const summaryRow = useMemo(() => {
		const {
			expenses,
			profit,
			incomeBynAmount,
			salaryAmount,
			averageKpi,
			averageKpiColor,
		} = summary;
		const managementSummaryRow: ManagementGridRow = {
			...summaryRowInitialValues,
			expenses,
			profit,
			incomeBYN: incomeBynAmount,
			salary: salaryAmount,
			kpi: averageKpi,
			kpiColor: averageKpiColor,
		};

		return managementSummaryRow;
	}, [summary]);

	const summaryRowHeight =
		rows.length === 0 ? 0 : MANAGEMENT_TOTAL_SUMMARY_ROW_HEIGHT;

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				enableVirtualization={false}
				onRowsChange={handleRowsChange}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				rowKeyGetter={getRowKey}
				rowHeight={getRowHeight}
				rowClass={getRatesRowClass}
				bottomSummaryRows={[summaryRow]}
				overrideDataGridStyles={overrideManagementGridStyles}
				{...{ columns, rows, sortColumns, paginationProps, summaryRowHeight }}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const ManagementGrid = memo(ManagementGridInner);
