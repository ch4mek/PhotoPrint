import { useEffect, useMemo, useState } from 'react';

import { MerchElementsGridRow } from '../types';
import { mapMerchItemsResponseToRows } from '../utils/mapMerchItemsResponseToRows';

import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetMerchElements } from '@/queries/merchElements/useGetMerchElements';
import { useMerchElementsLookup } from '@/queries/merchElements/useGetMerchElementsLookup';

import { useElementsColumns } from './useElementsColumns';
import { useElementsFilters } from './useElementsFilters';

export const useElemetsTableState = () => {
	const { id, handleOpen, handleClose, isOpen } = useShowModal();
	const [rows, setRows] = useState<MerchElementsGridRow[]>([]);
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
		persistentValues: { merchItemStatuses },
	} = useConfiguration();

	const { setFilters, gridFilters, responseBodyFilters } = useElementsFilters(
		merchStorages,
		order,
		orderBy
	);

	const allMerchStorages = useMemo(() => {
		const allStorages = convertToSelectionModalOptions(merchStorages, 'name');

		return allStorages;
	}, [merchStorages]);

	const allMerchStatuses = useMemo(() => {
		const allStatuses = convertToSelectionModalOptions(
			merchItemStatuses,
			'name'
		);

		return allStatuses;
	}, [merchItemStatuses]);

	const handleEditClick = (itemId?: string | number) => () => {
		handleOpen(itemId);
	};

	const { data, isFetching } = useGetMerchElements(
		{
			filters: gridFilters,
			top: rowsPerPage,
			skip: rowsPerPage * page,
		},
		merchItemStatuses,
		orderBy,
		responseBodyFilters,
		{
			onSuccess: () => {
				if (data) {
					setRows(mapMerchItemsResponseToRows(data.items, handleEditClick));
				}
			},
		},
		order
	);

	const { data: allMerchElements } = useMerchElementsLookup();

	const allMerchElementsNames = useMemo(
		() => (allMerchElements ? allMerchElements.map(({ name }) => name) : []),
		[allMerchElements]
	);

	const columns = useElementsColumns({
		filters: gridFilters,
		setFilters,
		allMerchStorages,
		allMerchStatuses,
		allMerchElementsNames,
	});

	useEffect(() => setPage(0), [responseBodyFilters, sortColumns]);

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
		handleSortColumnChange,
		isOpen,
		handleClose,
		handleOpen,
		pagination,
		rows,
		setRows,
		columns,
		isFetching,
	};
};
