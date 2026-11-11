import { useContext, useMemo, useState } from 'react';

import { certificatesGridColumns } from '../CertificatesGrid/constants';
import { useCertificatesColumns } from '../CertificatesGrid/hooks/useCertificatesColumns';
import { useCertificatesFiltersReducer } from '../CertificatesGrid/hooks/useCertificatesFiltersReducer';
import { useCertificatesRows } from '../CertificatesGrid/hooks/useCertificatesRows';

import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useGetUserCertificates } from '@/queries/useGetUserCertificates';

export const useCertificatesGridState = (
	isCertificatesGridAvailable: boolean
) => {
	const { userId } = useContext(PersonalDataContext);
	const toast = useToast();
	const [filters, setFilters] = useCertificatesFiltersReducer();
	const [isFormVisible, setIsFormVisble] = useState(false);

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
		userId!,
		toast,
		rowsPerPage,
		page,
		order,
		orderBy,
		filters,
		{
			enabled: isCertificatesGridAvailable,
		}
	);

	const rows = useCertificatesRows(data?.items, userId);
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

	const handlePopupToggle = () => {
		setIsFormVisble((prevVisible) => !prevVisible);
	};

	const isDataLoading = isLoading || isFetching;

	return {
		rows,
		columns,
		userCertificatesGridPagination,
		handleSortColumnChange,
		sortColumns,
		isDataLoading,
		isFormVisible,
		handlePopupToggle,
	};
};
