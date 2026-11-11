import { useContext, useEffect, useMemo, useState } from 'react';
import { useTranslation } from 'react-i18next';

import { absenceTransactionHistoryColumns } from '../components/AbsenceTransactionHistory/constants';

import { INQUIRY_TYPE_ORDER_WEIGHT } from '@/constants/absence';
import { ALL_ID } from '@/constants/filters';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAbsenceHistoryContext } from '@/context/Absence';
import { useToast } from '@/context/Toast';
import {
	convertToAbsenceTransactionHistoryRows,
	getOrderValueForAbsenceRequest,
} from '@/helpers/absences';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useTableState } from '@/pages/hooks/useTableState';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useGetTransactionsHistoryByUser } from '@/queries/absences/useGetTransactionsHistoryByUser';
import { AbsenceTransactionHistoryGridRow } from '@/types/absences';

export const useTransactionsHistoryGridState = () => {
	const toast = useToast();
	const { userId } = useContext(PersonalDataContext);
	const { yearFilter, absenceFilter } = useAbsenceHistoryContext();
	const { t: tAbsenceRequests } = useTranslation('common', {
		keyPrefix: 'absenceRequests',
	});
	const { t: tAbsenceDescriptions } = useTranslation('profile', {
		keyPrefix: 'absence',
	});

	const [rows, setRows] = useState<Array<AbsenceTransactionHistoryGridRow>>([]);
	const [totalUsed, setTotalUsed] = useState<number | null>(null);

	const {
		order,
		orderBy,
		page,
		rowsPerPage,
		sortColumns,
		handleSortColumnChange,
		setPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: absenceTransactionHistoryColumns.transactionDate.key,
		resetPageDeps: [yearFilter, absenceFilter],
	});

	const { data, isLoading } = useGetTransactionsHistoryByUser(
		{
			userId,
			top: rowsPerPage,
			skip: page * rowsPerPage,
			filters: {
				inquiryTemplateName:
					absenceFilter.id === ALL_ID ? null : absenceFilter.id,
				year: yearFilter.id === ALL_ID ? null : yearFilter.description,
			},
			orderBy: getOrderValueForAbsenceRequest(
				`${capitalizeString(
					orderBy === absenceTransactionHistoryColumns.inquiryTemplateName.key
						? INQUIRY_TYPE_ORDER_WEIGHT
						: orderBy
				)} ${order}`
			),
		},
		{
			enabled: !!userId,
			onError: createOnErrorHandler(toast),
		}
	);

	const gridPagination = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[rows.length, page, rowsPerPage, data?.total]
	);

	useEffect(() => {
		if (data) {
			setRows(
				convertToAbsenceTransactionHistoryRows(
					tAbsenceRequests,
					tAbsenceDescriptions,
					data.items
				)
			);
			setTotalUsed(data.totalUsed);
		}
	}, [data, tAbsenceRequests, tAbsenceDescriptions]);

	return {
		order,
		orderBy,
		rows,
		setRows,
		sortColumns,
		gridPagination,
		handleSortColumnChange,
		isPending: isLoading,
		totalUsed,
	};
};
