import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useHasScreenWidth } from '@mms/mms-ui-library/hooks/useHasScreenWidth';
import { breakpoints } from '@mms/mms-ui-library/theme';
import { useCallback, useMemo } from 'react';

import { useUserClaimsFilters } from '../../context/UserClaimsFiltersContext';
import { UserClaimsForm } from '../UserClaimsForm';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { NotificationType } from '@/constants/notifications';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createToast } from '@/helpers/createToast';
import { createOnErrorHandler } from '@/helpers/requestErrorHandler';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { User } from '@/pages/Settings/components/ClaimsTabs/types';
import { convertUsersRows } from '@/pages/Settings/components/ClaimsTabs/utils/convertUsersRows';
import { useGetAllUsersClaims } from '@/queries/useGetAdminUserClaims';
import { useGetClaimsGroups } from '@/queries/useGetClaimsGroups';
import { useUpdateUserClaims } from '@/queries/useUpdateUserClaims';

import {
	PERMISSIONS_SUCCESSFULY_UPDATE_TEXT,
	SORT_USERS_ID,
} from './constants';
import {
	useColumns,
	useGridStyles,
	useUserClaimsFilterOptions,
	useUserClaimsGridFilters,
} from './hooks';

export function UserClaimsGrid() {
	const toast = useToast();
	const isDesktopSWidth = useHasScreenWidth(breakpoints.desktopS);
	const { selectedClaim } = useUserClaimsFilters();
	const { filters, setFilters } = useUserClaimsGridFilters();
	const {
		id: userData,
		handleOpen,
		handleClose,
		isOpen,
	} = useShowModal<User>();

	const {
		rowsPerPage,
		handleOnPerPageChange,
		page,
		setPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.ASC,
		initialOrderBy: SORT_USERS_ID,
		resetPageDeps: [filters],
	});

	const { options, departmentTypes } = useUserClaimsFilterOptions();

	const { data: groupClaims } = useGetClaimsGroups({
		onError: createOnErrorHandler(toast),
	});

	const { data, isFetching, isLoading, refetch } = useGetAllUsersClaims({
		page,
		rowsPerPage,
		order,
		orderByCondition: orderBy,
		filters,
		claimName: selectedClaim?.id || '',
		allDepartmentsLength: departmentTypes.length,
		queryOptions: {
			onError: createOnErrorHandler(toast),
		},
	});

	const updateUserClaimsMutation = useUpdateUserClaims({
		onSuccess: () => {
			toast.open(
				createToast(
					NotificationType.SUCCESS,
					PERMISSIONS_SUCCESSFULY_UPDATE_TEXT
				)
			);
			refetch();
			handleClose();
		},
		onError: createOnErrorHandler(toast),
	});

	const columns = useColumns(
		groupClaims || [],
		filters,
		setFilters,
		options,
		isDesktopSWidth
	);

	const { overrideUserClaimsGridStyle } = useGridStyles(
		groupClaims,
		columns.length,
		isDesktopSWidth
	);

	const handleEdit = useCallback(
		(user: User) => () => {
			handleOpen(user);
		},
		[handleOpen]
	);

	const claimNames = useMemo(
		() => groupClaims?.map(({ name }) => name) || [],
		[groupClaims]
	);

	const rows = useMemo(
		() =>
			data
				? convertUsersRows(claimNames, handleEdit, departmentTypes, data.items)
				: [],
		[data, claimNames, handleEdit, departmentTypes]
	);

	const userClaimsTablePagination = useMemo(
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

	return (
		<>
			<Loader isLoading={isFetching || isLoading}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...{ rows, columns, sortColumns }}
						overrideDataGridStyles={overrideUserClaimsGridStyle}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
						paginationProps={userClaimsTablePagination}
						{...COMMON_GRID_PROPS}
					/>
				</DataGridHoverRowWrapper>
			</Loader>
			{isOpen && userData && (
				<UserClaimsForm
					{...{ groupClaims, handleClose }}
					user={userData}
					onSubmit={updateUserClaimsMutation.mutateAsync}
					isLoading={updateUserClaimsMutation.isPending}
				/>
			)}
		</>
	);
}
