import { useCallback, useMemo } from 'react';

import { PAYSLIPS_COLUMN_KEYS } from '../constants';
import { mapPayslipsResponseToRows } from '../util/mapPayslipsResponseToRows';

import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useUserPayslips } from '@/queries/teamboard/useUserPayslips';

import { usePayslipsFilters } from './usePayslipsFilters';

export const usePayslipsGridState = (userId?: number) => {
	const { filters, areFiltersApplied, resetFilters, setFilters } =
		usePayslipsFilters();

	const {
		rowsPerPage,
		setRowsPerPage,
		page,
		setPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialOrder: SortDirection.DESC,
		initialOrderBy: PAYSLIPS_COLUMN_KEYS.reportDate,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useUserPayslips({
		userId,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		orderBy: `${capitalizeString(orderBy)} ${order}`,
		filters,
	});

	const rows = useMemo(() => {
		if (!data) {
			return [];
		}

		return mapPayslipsResponseToRows(data.items);
	}, [data]);

	const handleOnPerPageChange = useCallback((rowsPerPageCounter: number) => {
		setRowsPerPage(rowsPerPageCounter);
		setPage(0);
	}, []);

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
		[rows.length, page, rowsPerPage, data?.total]
	);

	return {
		rows,
		pagination,
		isFetching: isLoading || isFetching,
		areFiltersApplied,
		filters,
		setFilters,
		resetFilters,
		sortColumns,
		handleSortColumnChange,
	};
};
