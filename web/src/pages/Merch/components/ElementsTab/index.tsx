import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useMemo } from 'react';

import { overrideMerchGridStyles } from '../../utils/overrideElementsTabGrid';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	getSuccessfullyAddedMessage,
	NotificationType,
} from '@/constants/notifications';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { MerchElementForm } from '@/forms/MerchElementForm';
import { MerchStorageForm } from '@/forms/MerchStorageForm';
import { TRANSACTION_FIELD_TITLE } from '@/forms/MerchStorageForm/constants';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { PageWrapper } from '@/pages/styles';
import { useAddStorageTransaction } from '@/queries/merchStorage/useAddStorageTransaction';
import { UserClaims } from '@/types/claims';

import { ELEMENT_TAB_COLUMNS_WIDTH } from './constants';
import { useElementsTabContext } from './hooks/useElementsTabActions';
import { useElemetsTableState } from './hooks/useElementsTableState';
import { MerchElementsGridRow } from './types';

export function ElementsTab() {
	const { claims } = useAuth();
	const canManageItems = checkIsHasClaims(
		[UserClaims.ManageItemsAndKits],
		claims
	);

	const {
		handleOpen: handleOpenTransactionForm,
		handleClose: handleCloseTransactionForm,
		isOpen: isOpenTransaction,
	} = useShowModal();

	const toast = useToast();
	const {
		handleClose,
		handleOpen,
		isFetching,
		isOpen,
		columns,
		rows,
		sortColumns,
		setRows,
		id: merchId,
		pagination,
		handleSortColumnChange,
	} = useElemetsTableState();

	const {
		addMerchElement,
		updateMerchElement,
		updateMerchElementStatus,
		addedMerchElement,
		isUpdatingMerchElementStatus,
	} = useElementsTabContext(handleClose, handleOpenTransactionForm, merchId);

	const { mutateAsync: addStorageTransaction } = useAddStorageTransaction({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(TRANSACTION_FIELD_TITLE)
				)
			);
			handleCloseTransactionForm();
		},
		onError: createOnErrorHandler(toast),
	});

	const overrideDataGridStyles = overrideMerchGridStyles(
		Boolean(pagination),
		ELEMENT_TAB_COLUMNS_WIDTH
	);

	const getRowHeight = useMemo(
		() => (row: MerchElementsGridRow) => row.itemsRowHeight,
		[]
	);

	return (
		<PageWrapper>
			<PageHeader
				addButton={{ onClick: handleOpen, isHidden: !canManageItems }}
			/>
			<Loader isLoading={isFetching}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...{ rows, columns, sortColumns, overrideDataGridStyles }}
						rowHeight={getRowHeight}
						onRowsChange={setRows}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
						scrollToTopOnRowsChange={false}
						paginationProps={pagination}
					/>
				</DataGridHoverRowWrapper>
			</Loader>

			{isOpen && (
				<MerchElementForm
					{...{ handleClose, merchId }}
					handleSubmit={addMerchElement}
					handleUpdateSubmit={updateMerchElement}
					handleUpdateStatus={updateMerchElementStatus}
					isLoadingStatus={isUpdatingMerchElementStatus}
				/>
			)}

			{isOpenTransaction && addedMerchElement?.id && (
				<MerchStorageForm
					handleClose={handleCloseTransactionForm}
					merchId={addedMerchElement.id}
					handleSubmit={addStorageTransaction}
					isAfterElementAdding
				/>
			)}
		</PageWrapper>
	);
}
