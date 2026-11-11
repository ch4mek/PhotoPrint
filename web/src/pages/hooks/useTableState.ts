import { useCallback, useEffect, useMemo, useState } from 'react';
import { SortColumn } from 'react-data-grid';

import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection, DefaultSortFields } from '@/constants/sort';
import {
	convertToDataGridSortDirection,
	convertToNativeSortDirection,
} from '@/helpers/convertSortDirection';

import { UseTaableStateProps } from './types';

export const useTableState = ({
	initialRowsPerPage = Number(DEFAULT_ROWS_PER_PAGE),
	initialOrder = SortDirection.ASC,
	initialOrderBy = DefaultSortFields.ID,
	resetPageDeps = [],
}: UseTaableStateProps = {}) => {
	const [rowsPerPage, setRowsPerPage] = useState(initialRowsPerPage);
	const [order, setOrder] = useState(initialOrder);
	const [orderBy, setOrderBy] = useState(initialOrderBy);
	const [page, setPage] = useState(0);

	const handleSortColumnChange = useCallback(
		(newSortColumns: SortColumn[]) => {
			if (!newSortColumns.length) {
				setOrder(
					initialOrder === SortDirection.DESC ? SortDirection.ASC : initialOrder
				);

				setOrderBy(orderBy);
				setPage(0);

				return;
			}

			const newSortColumnKey = newSortColumns[0].columnKey;
			const newSortOrder = convertToNativeSortDirection(
				newSortColumns[0].direction
			);

			setOrder(newSortOrder);
			setOrderBy(newSortColumnKey);
			setPage(0);
		},
		[orderBy, initialOrder]
	);

	useEffect(() => {
		setPage(0);
	}, resetPageDeps);

	const sortColumns: SortColumn[] = useMemo(
		() => [
			{
				columnKey: orderBy,
				direction: convertToDataGridSortDirection(order),
			},
		],
		[orderBy, order]
	);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	return {
		page,
		setPage,
		rowsPerPage,
		setRowsPerPage,
		order,
		setOrder,
		orderBy,
		setOrderBy,
		sortColumns,
		handleSortColumnChange,
		handleOnPerPageChange,
	};
};
