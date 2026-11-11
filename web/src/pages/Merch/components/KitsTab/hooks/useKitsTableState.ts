import { useEffect, useMemo, useState } from 'react';

import { MerchKitsGridRow } from '../types';
import { mapMerchItemsResponseToRows } from '../utils/mapMerchKitsResponseToRows';

import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetMerchKits } from '@/queries/merchKits/useGetMerchKits';

import { useKitsColumns } from './useKitsColumns';
import { useKitsFilters } from './useKitsFilters';

export const useKitsTableState = () => {
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const {
		id: transactionId,
		handleOpen: handleTransactionOpen,
		handleClose: handleTransactionClose,
		isOpen: isTransactionOpen,
	} = useShowModal();

	const [rows, setRows] = useState<MerchKitsGridRow[]>([]);
	const {
		sortColumns,
		handleSortColumnChange,
		rowsPerPage,
		page,
		order,
		orderBy,
		handleOnPerPageChange,
		setPage,
	} = useTableState();
	const {
		configurableValues: { merchStorages },
		persistentValues: { merchStorageStatuses },
	} = useConfiguration();

	const { setFilters, gridFilters, responseBodyFilters } = useKitsFilters(
		order,
		orderBy
	);

	const allMerchStorages = useMemo(() => {
		const allStorages = convertToSelectionModalOptions(merchStorages, 'name');

		return allStorages;
	}, [merchStorages]);

	const allMerchStatuses = useMemo(() => {
		const allStatuses = convertToSelectionModalOptions(
			merchStorageStatuses,
			'description'
		);

		return allStatuses;
	}, [merchStorageStatuses]);

	const handleEditClick = (itemId?: string | number) => () => {
		handleOpen(itemId);
	};

	const handleTransactionClick = (kitId?: string | number) => () => {
		handleTransactionOpen(kitId);
	};

	const { data, isFetching } = useGetMerchKits(
		{
			filters: gridFilters,
			top: rowsPerPage,
			skip: rowsPerPage * page,
		},
		responseBodyFilters,
		{
			onSuccess: () => {
				if (data) {
					setRows(
						mapMerchItemsResponseToRows(
							data.items,
							merchStorageStatuses,
							handleEditClick,
							handleTransactionClick
						)
					);
				}
			},
		}
	);

	const allMerchStorageNames = useMemo(
		() => (data?.items ? data.items.map(({ name }) => name) : []),
		[data]
	);

	const columns = useKitsColumns({
		filters: gridFilters,
		setFilters,
		allMerchStorages,
		allMerchStatuses,
		allMerchStorageNames,
	});

	useEffect(() => setPage(0), [responseBodyFilters, sortColumns, rowsPerPage]);

	const pagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows, page, rowsPerPage, data?.total]
	);

	return {
		sortColumns,
		id,
		isOpen,
		pagination,
		rows,
		columns,
		isFetching,
		setRows,
		handleClose,
		handleOpen,
		handleSortColumnChange,
		handleEditClick,
		transactionId,
		handleTransactionClose,
		handleTransactionOpen,
		isTransactionOpen,
		handleTransactionClick,
	};
};
