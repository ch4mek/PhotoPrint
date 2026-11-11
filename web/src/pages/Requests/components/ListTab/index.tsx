import { useTheme } from '@emotion/react';
import { useCallback, useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { requestStatusOptions } from '@/constants/absence';
import { NO_VALUE_FILTER_OPTION } from '@/constants/filters';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { PopUpSearchParams } from '@/constants/searchParams';
import { SortDirection } from '@/constants/sort';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { convertToSelectionModalOptions } from '@/helpers/convertToSelectionModalOptions';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { getFullName } from '@/helpers/utils';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetAllInquries } from '@/queries/inquiries/useGetAllInquries';
import { useInquiriesLookup } from '@/queries/inquiries/useInquiriesLookup';
import { useRequestUsersLookup } from '@/queries/inquiries/useRequestUsersLookup';

import { RequestsGrid } from './components/RequestsGrid';
import { DEFAULT_REQUESTS_FILTERS, REQUESTS_COLUMNS } from './constants';
import { useRequestRows } from './hooks/useRequestRows';
import { useRequestsFilters } from './hooks/useRequestsFilters';
import { FormParams } from './types';
import { getOrderValueForAbsenceRequest } from './utils';

export function ListTab() {
	const toast = useToast();
	const theme = useTheme();

	const {
		persistentValues: { absenceTypes },
		configurableValues: { roleTypes, departmentTypes },
	} = useConfiguration();

	const { filters, setFilters } = useRequestsFilters(DEFAULT_REQUESTS_FILTERS);

	const { data: users } = useRequestUsersLookup({
		onError: createOnErrorHandler(toast),
	});

	const { data: inquiries } = useInquiriesLookup({
		onError: createOnErrorHandler(toast),
	});

	const handleOpenPopUp = useOpenPopUpViaUrl(PopUpSearchParams.InquiryIdParam);

	const handleOpen = useCallback(
		(values: FormParams) => {
			if (values.inquiryId) {
				handleOpenPopUp(String(values.inquiryId));
			}
		},
		[handleOpenPopUp]
	);

	const departments = useMemo(
		() => [...departmentTypes, NO_VALUE_FILTER_OPTION],
		[departmentTypes]
	);

	const options = useMemo(
		() => ({
			users: users
				? users.map(({ firstName, lastName, firstNameRu, lastNameRu }) =>
						getFullName(firstName, lastName, firstNameRu, lastNameRu)
				  )
				: [],
			positions: roleTypes,
			absenceNames: (inquiries ?? []).map(({ name }) => name),
			departments: convertToSelectionModalOptions(departments, 'fullName'),
			statuses: requestStatusOptions,
			onFormOpen: handleOpen,
		}),
		[users, inquiries, absenceTypes, roleTypes, handleOpen]
	);

	const {
		page,
		setPage,
		rowsPerPage,
		setRowsPerPage,
		order,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: REQUESTS_COLUMNS.id.key,
		resetPageDeps: [filters],
	});

	const { data, isLoading, isFetching } = useGetAllInquries(
		rowsPerPage,
		page,
		getOrderValueForAbsenceRequest(order, orderBy),
		filters,
		departments.length,
		absenceTypes.length,
		roleTypes.length
	);

	const rows = useRequestRows(roleTypes, data?.items);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const paginationProps = useMemo(
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

	const isLoadingData = isLoading || isFetching;

	return (
		<PageWrapper style={{ paddingTop: theme.spaces.m }}>
			<Loader isLoading={isLoadingData}>
				<RequestsGrid
					{...{
						rows,
						sortColumns,
						handleSortColumnChange,
						paginationProps,
						filters,
						setFilters,
						options,
					}}
				/>
			</Loader>
		</PageWrapper>
	);
}
