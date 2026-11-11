import { useMemo } from 'react';

import { certificatesGridColumns } from '../CertificatesGrid/constants';
import { useCertificatesColumns } from '../CertificatesGrid/hooks/useCertificatesColumns';
import { useCertificatesFiltersReducer } from '../CertificatesGrid/hooks/useCertificatesFiltersReducer';
import { useCertificatesRows } from '../CertificatesGrid/hooks/useCertificatesRows';

import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetUserCertificates } from '@/queries/useGetUserCertificates';

export const useCertificatesGridState = () => {
	const { id } = useAuth();
	const toast = useToast();
	const [filters, setFilters] = useCertificatesFiltersReducer();

	const {
		page,
		rowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		setPage,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: certificatesGridColumns.receiptDate.key,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useGetUserCertificates(
		id,
		toast,
		rowsPerPage,
		page,
		order,
		orderBy,
		filters
	);

	const rows = useCertificatesRows(data?.items);
	const columns = useCertificatesColumns(filters, setFilters);

	const userCertificatesGridPagination = useMemo(
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

	const isDataLoading = isLoading || isFetching;

	return {
		rows,
		columns,
		userCertificatesGridPagination,
		handleSortColumnChange,
		sortColumns,
		isDataLoading,
	};
};
