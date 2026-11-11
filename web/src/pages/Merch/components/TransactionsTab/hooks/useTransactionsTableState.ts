import { useMemo, useState } from 'react';

import {
	TRANSACTIONS_TAB_GRID_COLUMNS,
	TransactionTypes,
	TRANSACTION_TYPE_ORDER_BY,
} from '../constants';
import { MerchTransactionsGridRow } from '../types';
import { mapMerchTransactionsResponseToRows } from '../utils/mapMerchTransactionsResponseToRows';

import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getFullName } from '@/helpers/utils';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useMerchElementsLookup } from '@/queries/merchElements/useGetMerchElementsLookup';
import { useGetMerchTransactions } from '@/queries/merchTransactions/useGetMerchTransactions';
import { useRecipientsLookup } from '@/queries/merchTransactions/useRecipientsLookup';

import { useTransactionsColumns } from './useTransactionsColumns';
import { useTransactionsFilters } from './useTransactionsFilters';

export const useTransactionsTableState = () => {
	const [rows, setRows] = useState<MerchTransactionsGridRow[]>([]);
	const { id, handleClose, isOpen } = useShowModal();

	const { setFilters, gridFilters, responseBodyFilters } =
		useTransactionsFilters();

	const {
		sortColumns,
		handleSortColumnChange,
		rowsPerPage,
		page,
		handleOnPerPageChange,
		setPage,
		order,
		orderBy,
	} = useTableState({
		initialOrder: SortDirection.DESC,
		initialOrderBy: TRANSACTIONS_TAB_GRID_COLUMNS.id.key,
		resetPageDeps: [responseBodyFilters, gridFilters],
	});

	const {
		configurableValues: { merchStorages },
		persistentValues: { merchTransactionTypes },
	} = useConfiguration();

	const { data: allMerchElements } = useMerchElementsLookup();
	const { data: allActiveUsers } = useRecipientsLookup();

	const { data, isFetching } = useGetMerchTransactions(
		{
			filters: gridFilters,
			top: rowsPerPage,
			skip: rowsPerPage * page,
			orderBy:
				orderBy === TRANSACTIONS_TAB_GRID_COLUMNS.transactionType.key
					? TRANSACTION_TYPE_ORDER_BY
					: orderBy,
		},
		order,
		merchStorages,
		merchTransactionTypes,
		allActiveUsers ?? [],
		responseBodyFilters,
		{
			onSuccess: () => {
				if (data) {
					setRows(mapMerchTransactionsResponseToRows(data.items));
				}
			},
		}
	);

	const allMerchTransactionsNames = useMemo(
		() => (allMerchElements ? allMerchElements.map(({ name }) => name) : []),
		[allMerchElements]
	);

	const allMerchTransactionsCreators = useMemo(
		() => Array.from(new Set(rows.map(({ creator }) => creator))),
		[rows]
	);

	const allMerchSenders = useMemo(() => {
		const allStorages = convertToSelectionModalOptions(
			merchStorages,
			'name'
		).map(({ description }) => ({
			id: description,
			description,
		}));

		return [...allStorages, NO_VALUE_FILTER_OPTION];
	}, [merchStorages]);

	const allMerchStorages = useMemo(() => {
		const allStorages = convertToSelectionModalOptions(
			merchStorages,
			'name'
		).map(({ description }) => ({
			id: description,
			description,
		}));

		return allStorages;
	}, [merchStorages]);

	const allMerchTransactionTypes = useMemo(() => {
		const allStorages = convertToSelectionModalOptions(
			merchTransactionTypes,
			'name'
		).map(({ description }) => ({
			id: description,
			description:
				TransactionTypes.find(
					({ id: transactionId }) => description === transactionId
				)?.name ?? '',
		}));

		return allStorages;
	}, [merchStorages]);

	const allRecipients = useMemo(
		() => [
			...allMerchStorages,
			...(allActiveUsers ?? []).map(
				({ firstName, lastName, firstNameRu, lastNameRu }) => {
					const fullNameEnRu = getFullName(
						firstName,
						lastName,
						firstNameRu,
						lastNameRu
					);

					return {
						id: fullNameEnRu,
						description: fullNameEnRu,
					};
				}
			),
		],
		[allMerchStorages, allActiveUsers]
	);

	const columns = useTransactionsColumns({
		filters: gridFilters,
		setFilters,
		allMerchTransactionsNames,
		allMerchTransactionsCreators,
		allMerchSenders,
		allMerchStorages,
		allMerchTransactionTypes,
		allRecipients,
	});

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
		handleSortColumnChange,
		pagination,
		rows,
		setRows,
		id,
		handleClose,
		isOpen,
		columns,
		isFetching,
	};
};
