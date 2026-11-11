import { useEffect, useMemo, useState } from 'react';

import { INITIAL_ORDER_BY_COLUMN } from '../constants';
import { MerchStorageGridRow } from '../types';
import { mapMerchItemsResponseToRows } from '../utils/mapMerchItemsResponseToRows';

import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useMerchElementsLookup } from '@/queries/merchElements/useGetMerchElementsLookup';
import { useGetMerchStorage } from '@/queries/merchStorage/useGetMerchStorage';

import { useStorageColumns } from './useStorageColumns';
import { useStorageFilters } from './useStorageFilters';

export const useStorageTableState = () => {
	const { id, handleOpen, handleClose, isOpen } = useShowModal();

	const [rows, setRows] = useState<MerchStorageGridRow[]>([]);
	const {
		sortColumns,
		handleSortColumnChange,
		rowsPerPage,
		page,
		order,
		orderBy,
		handleOnPerPageChange,
		setPage,
	} = useTableState({
		initialOrderBy: INITIAL_ORDER_BY_COLUMN,
	});
	const {
		configurableValues: { merchStorages },
		persistentValues: { merchStorageStatuses },
	} = useConfiguration();

	const handleEditClick = (itemId?: string | number) => () => {
		handleOpen(itemId);
	};

	const { setFilters, gridFilters, responseBodyFilters } = useStorageFilters(
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

	const { data: allMerchElements } = useMerchElementsLookup();

	const { data, isFetching } = useGetMerchStorage(
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
							handleEditClick
						)
					);
				}
			},
		}
	);

	const allMerchStorageNames = useMemo(
		() => (allMerchElements ? allMerchElements.map(({ name }) => name) : []),
		[data]
	);

	const columns = useStorageColumns({
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
		handleClose,
		handleOpen,
		isOpen,
		id,
		handleSortColumnChange,
		pagination,
		rows,
		setRows,
		columns,
		isFetching,
	};
};
