import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useQueryClient } from '@tanstack/react-query';
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
import { merchQueriesKeys } from '@/constants/queriesKeys';
import { useToast } from '@/context/Toast';
import { MerchStorageForm } from '@/forms/MerchStorageForm';
import { TRANSACTION_FIELD_TITLE } from '@/forms/MerchStorageForm/constants';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { PageWrapper } from '@/pages/styles';
import { useAddStorageTransaction } from '@/queries/merchStorage/useAddStorageTransaction';

import { STORAGE_TAB_COLUMNS_WIDTH } from './constants';
import { useStorageTableState } from './hooks/useStorageTableState';
import { MerchStorageGridRow } from './types';

export function StorageTab() {
	const queryClient = useQueryClient();
	const toast = useToast();
	const {
		isFetching,
		columns,
		rows,
		sortColumns,
		setRows,
		pagination,
		handleSortColumnChange,
		isOpen,
		id: merchId,
		handleClose,
	} = useStorageTableState();

	const { mutateAsync: addStorageTransaction } = useAddStorageTransaction({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					getSuccessfullyAddedMessage(TRANSACTION_FIELD_TITLE)
				)
			);
			handleClose();
			queryClient.invalidateQueries({
				queryKey: [merchQueriesKeys.getMerchStorageData],
			});
		},
		onError: createOnErrorHandler(toast),
	});

	const overrideGridStyles = overrideMerchGridStyles(
		Boolean(pagination),
		STORAGE_TAB_COLUMNS_WIDTH
	);

	const getRowHeight = useMemo(
		() => (row: MerchStorageGridRow) => row.itemsRowHeight,
		[]
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
			{isOpen && merchId && (
				<MerchStorageForm
					{...{ handleClose, merchId }}
					handleSubmit={addStorageTransaction}
				/>
			)}
		</PageWrapper>
	);
}
