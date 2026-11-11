import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { overrideMerchGridStyles } from '../../utils/overrideElementsTabGrid';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { useAuth } from '@/context/AuthContext';
import { MerchKitsForm } from '@/forms/MerchKitsFrom';
import { MerchKitsTransactionForm } from '@/forms/MerchKitsTransactionForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { PageWrapper } from '@/pages/styles';
import { UserClaims } from '@/types/claims';

import { KITS_TAB_COLUMNS_WIDTH } from './constants';
import { useKitsTabContext } from './hooks/useKitsTabContext';
import { useKitsTableState } from './hooks/useKitsTableState';
import { MerchKitsGridRow } from './types';

export function KitsTab() {
	const { claims } = useAuth();
	const canManageKits = checkIsHasClaims(
		[UserClaims.ManageItemsAndKits],
		claims
	);

	const {
		isFetching,
		columns,
		rows,
		sortColumns,
		pagination,
		setRows,
		handleSortColumnChange,
		handleOpen,
		isOpen,
		handleClose,
		id,
		transactionId,
		handleTransactionClose,
		isTransactionOpen,
	} = useKitsTableState();

	const overrideDataGridStyles = overrideMerchGridStyles(
		Boolean(pagination),
		KITS_TAB_COLUMNS_WIDTH
	);

	const { updateMerchKit, addMerchKit, addKitTransaction } = useKitsTabContext(
		handleClose,
		handleTransactionClose
	);

	const getRowHeight = useMemo(
		() => (row: MerchKitsGridRow) => row.itemRowHeight,
		[]
	);

	return (
		<PageWrapper>
			<PageHeader
				addButton={{ onClick: handleOpen, isHidden: !canManageKits }}
			/>
			<Loader isLoading={isFetching}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...{ rows, columns, sortColumns, overrideDataGridStyles }}
						rowHeight={getRowHeight}
						renderers={noRowsGridFallback}
						paginationProps={pagination}
						onRowsChange={setRows}
						onSortColumnsChange={handleSortColumnChange}
						scrollToTopOnRowsChange={false}
					/>
				</DataGridHoverRowWrapper>
			</Loader>
			{isOpen && (
				<MerchKitsForm
					handleClose={handleClose}
					handleSubmit={addMerchKit}
					handleUpdateSubmit={updateMerchKit}
					merchId={id}
				/>
			)}
			{isTransactionOpen && transactionId && (
				<MerchKitsTransactionForm
					handleClose={handleTransactionClose}
					handleSubmit={addKitTransaction}
					kitId={transactionId}
				/>
			)}
		</PageWrapper>
	);
}
