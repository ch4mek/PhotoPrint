import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { memo, useMemo } from 'react';

import { expensesGridColumnsWidths } from '../../constants';
import { useExpensesColumns } from '../../hooks/useExpensesColumns';
import { ExpensesSummaryGridRow } from '../../types';

import { getGridColumnTemplateFromColumnWidths } from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { useDataGridOffsetWidth } from '@/components/Grid/hooks/useGridWidth';
import { months } from '@/constants/date';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { getMonthColumnWidths } from '@/helpers/getMonthColumnWidths';
import { formatPrice } from '@/pages/ProfitAndLoss/utils/formatPrice';
import { getRowKey } from '@/pages/ProfitAndLoss/utils/getRowKey';

import { EXPENSES_TOTAL_ROW_HEIGHT } from './constants';
import { overrideExpensesGridStyles } from './styles';
import { ExpensesGridProps } from './types';
import { getMonthColumnWidth, isExpensesGridCompactView } from './utils';

function ExpensesGridInner({
	rows,
	summary,
	filters,
	setFilters,
	isResetFiltersDisabled,
	sortColumns,
	handleSortColumnChange,
	paginationProps,
	handleEdit,
	handleDelete,
	filterOptions,
}: ExpensesGridProps) {
	const { gridOffsetWidth, gridRef } = useDataGridOffsetWidth();

	const monthColumnWidth = getMonthColumnWidth(gridOffsetWidth);
	const isCompactGridView = isExpensesGridCompactView(monthColumnWidth);
	const columns = useExpensesColumns({
		filters,
		setFilters,
		isResetFiltersDisabled,
		monthColumnWidth,
		handleEdit,
		handleDelete,
		filterOptions,
	});

	const overrideDataGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			{
				...expensesGridColumnsWidths,
				...getMonthColumnWidths(months, monthColumnWidth),
			},
			true
		);

		return overrideExpensesGridStyles(gridColumnTemplate, isCompactGridView);
	}, [rows.length, monthColumnWidth, isCompactGridView]);

	const summaryRow = useMemo(() => {
		const expensesSummaryRow: ExpensesSummaryGridRow = {
			values: summary.map(formatPrice),
		};

		return expensesSummaryRow;
	}, [summary]);

	const summaryRowHeight = rows.length === 0 ? 0 : EXPENSES_TOTAL_ROW_HEIGHT;

	return (
		<DataGridHoverRowWrapper defineHeight={false} flexGrow>
			<DataGrid
				{...COMMON_GRID_PROPS}
				ref={gridRef}
				onSortColumnsChange={handleSortColumnChange}
				renderers={noRowsGridFallback}
				scrollToTopOnRowsChange={false}
				rowKeyGetter={getRowKey}
				{...{
					columns,
					rows,
					sortColumns,
					paginationProps,
					overrideDataGridStyles,
					summaryRowHeight,
				}}
				bottomSummaryRows={[summaryRow]}
				enableVirtualization={false}
			/>
		</DataGridHoverRowWrapper>
	);
}

export const ExpensesGrid = memo(ExpensesGridInner);
