import { useMemo } from 'react';

import {
	absenceRequestColumns,
	INQUIRY_TYPE_ORDER_WEIGHT,
} from '@/constants/absence';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { getOrderValueForAbsenceRequest } from '@/helpers/absences';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetAbsenceRequestsByUser } from '@/queries/absences/useGetAbsenceRequestsByUser';

import { useAbsenceRequestsFiltersReducer } from './useAbsenceRequestsFiltersReducer';
import { useAbsenceRequestsRows } from './useAbsenceRequestsRows';

type UseAbsenceRequestGridStateProps = {
	numberOfAbsences: number;
	userId?: number;
};

export const useAbsenceRequestGridState = ({
	numberOfAbsences,
	userId: externalUserId,
}: UseAbsenceRequestGridStateProps) => {
	const toast = useToast();
	const { id: authUserId } = useAuth();

	const userId = externalUserId ?? authUserId;

	const [filters, setFilters] = useAbsenceRequestsFiltersReducer();

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
		initialOrderBy: absenceRequestColumns.dateFrom.key,
		resetPageDeps: [filters],
	});

	const { data, isLoading } = useGetAbsenceRequestsByUser(
		{
			userId,
			top: rowsPerPage,
			skip: page * rowsPerPage,
			filters,
			orderBy: getOrderValueForAbsenceRequest(
				`${capitalizeString(
					orderBy === absenceRequestColumns.inquiryTemplateName.key
						? INQUIRY_TYPE_ORDER_WEIGHT
						: orderBy
				)} ${order}`
			),
			numberOfAbsences,
		},
		{
			enabled: !!userId,
			onError: createOnErrorHandler(toast),
		}
	);

	const rows = useAbsenceRequestsRows(data?.items || []);

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

	return {
		filters,
		setFilters,
		rows,
		sortColumns,
		gridPagination,
		handleSortColumnChange,
		isPending: isLoading,
	};
};
