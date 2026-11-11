import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PageWrapper } from '@/pages/styles';

import { TRANSACTIONS_TAB_COLUMNS_WIDTH } from './constants';
import { useTransactionsTableState } from './hooks/useTransactionsTableState';
import { MerchTransactionsGridRow } from './types';
import { overrideTransactionsTabGridStyles } from './utils/overrideElementsTabGridStyles';

export function TransactionsTab() {
	const {
		columns,
		handleSortColumnChange,
		isFetching,
		pagination,
		rows,
		setRows,
		sortColumns,
	} = useTransactionsTableState();

	const getRowHeight = useMemo(
		() => (row: MerchTransactionsGridRow) => row.itemsRowHeight,
		[]
	);

	const overrideGridStyles = overrideTransactionsTabGridStyles(
		Boolean(pagination),
		TRANSACTIONS_TAB_COLUMNS_WIDTH
	);

	return (
		<PageWrapper>
			<PageHeader />
			<Loader isLoading={isFetching}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						rowHeight={getRowHeight}
						columns={columns}
						rows={rows}
						overrideDataGridStyles={overrideGridStyles}
						sortColumns={sortColumns}
						onRowsChange={setRows}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
						scrollToTopOnRowsChange={false}
						paginationProps={pagination}
					/>
				</DataGridHoverRowWrapper>
			</Loader>
		</PageWrapper>
	);
}
