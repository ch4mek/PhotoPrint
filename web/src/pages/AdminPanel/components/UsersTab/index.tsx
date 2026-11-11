import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { useCallback, useEffect, useMemo } from 'react';
import 'react-data-grid/lib/styles.css';

import { ActivationMessage } from '@/components/ActivationMessage';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import {
	getActivatingPopUpTitle,
	getDeactivatingPopUpTitle,
} from '@/constants/activityStatus';
import { personalInfoClaimAccess } from '@/constants/claims';
import {
	isChiefRole,
	isIntern,
	Roles,
	isManagerRole,
	isSysAdmin,
} from '@/constants/roles';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth, useConfiguration, useToast } from '@/context/index';
import { UserAddingForm } from '@/forms/AdminPanelForms/UserAddingForm';
import { UserForm } from '@/forms/AdminPanelForms/UserForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useInitialFiltersWithDepartmentByDefault } from '@/hooks/useInitialFiltersWithDepartmentByDefault';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { useGridOptions } from '@/pages/AdminPanel/components/UsersTab/hooks/useGridOptions';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetDepartments } from '@/queries/departments/useGetDepartments';
import { useUsersTabLookup } from '@/queries/user/useUsersTabLookup';
import { useUsersTab } from '@/queries/useUsersTab';
import { UserClaims } from '@/types/claims';
import { UsersGridRow } from '@/types/userInfo';

import { UsersGrid } from './components/UsersGrid';
import {
	DEFAULT_USER_FILTERS,
	DEFAULT_USER_ORDER_BY,
	getUsersColumnsWidths,
} from './constants';
import { useUserFilters } from './hooks/useUserFIlters';
import { useUserFormDefaultValues } from './hooks/useUserFormDefaultValues';
import { useUsersMutations } from './hooks/useUsersMutations';
import { useUsersRows } from './hooks/useUsersValues';

export function UsersTab() {
	const { role, departmentId, claims } = useAuth();
	const toast = useToast();

	const {
		isLoading: loading,
		configurableValues: { departmentTypes, roleTypes },
		persistentValues: { locationTypes },
	} = useConfiguration();

	const { page, setPage } = usePageWithSearch();

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const {
		id: editId,
		handleOpen: handleEditOpen,
		handleClose: handleEditClose,
		isOpen: isEditOpen,
	} = useShowModal();
	const {
		handleOpen: handleAddOpen,
		handleClose: handleAddClose,
		isOpen: isAddOpen,
	} = useShowModal();
	const {
		isOpen: isActivateDialogOpen,
		handleOpen: openActivateDialog,
		handleClose: closeActivateDialog,
		id: selectedUser,
	} = useShowModal<UsersGridRow>();

	const {
		rowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({ initialOrderBy: DEFAULT_USER_ORDER_BY });

	const initialFilters = useInitialFiltersWithDepartmentByDefault(
		DEFAULT_USER_FILTERS,
		role,
		departmentId,
		claims,
		personalInfoClaimAccess
	);
	const { filters, setFilters } = useUserFilters(initialFilters);

	const hasUsersModuleAccess = checkIsHasClaims(
		personalInfoClaimAccess,
		claims
	);
	const hasManageAllInternsClaim = checkIsHasClaims(
		[UserClaims.ManageAllInterns],
		claims
	);
	const hasHRModuleClaim = checkIsHasClaims(
		[UserClaims.ManageHRModule],
		claims
	);
	const columnsWidths = useMemo(
		() => getUsersColumnsWidths(role, hasUsersModuleAccess),
		[role, hasUsersModuleAccess]
	);

	const enableCellClick = useCallback(
		({ position, positionKey, id }: UsersGridRow) =>
			id !== undefined &&
			canOpenProfile(+id) &&
			(hasUsersModuleAccess ||
				(hasManageAllInternsClaim && isIntern(positionKey as Roles)) ||
				(!isSysAdmin(role) && !isChiefRole(position as Roles))),
		[hasUsersModuleAccess, hasManageAllInternsClaim, role, canOpenProfile]
	);

	const { isLoading, data, dataUpdatedAt, isFetching, refetch } = useUsersTab(
		page + 1,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		departmentTypes,
		locationTypes,
		roleTypes
	);
	const { data: users, refetch: refetchUsersLookup } = useUsersTabLookup(toast);
	const { data: departmentsInfo, isLoading: isLoadingDepartments } =
		useGetDepartments({
			enabled: isManagerRole(role) || isSysAdmin(role) || hasUsersModuleAccess,
		});

	const {
		isEmailDuplicate,
		setIsEmailDuplicate,
		isUserCreating,
		updateUserStatusMutation,
		onUserCreate,
		onUserUpdate,
	} = useUsersMutations({
		refetch,
		refetchUsersLookup,
		page,
		rowsPerPage,
		order,
		orderBy,
	});

	const handleCrossLinkClick = useCallback(
		(id?: string | number): VoidFunction | undefined => {
			if (!id || !canOpenProfile(Number(id))) {
				return;
			}

			return () => handlePageOpen(String(id));
		},
		[canOpenProfile, handlePageOpen]
	);

	const handleEdit = useCallback(
		(itemId?: string | number) => () => {
			handleEditOpen(itemId);
		},
		[handleEditOpen]
	);

	const options = useGridOptions({
		users,
		departmentsInfo,
		hasUsersModuleAccess,
	});
	const { defaultValues } = useUserFormDefaultValues({
		data,
		editId,
		isEditOpen,
		departmentsInfo,
	});
	const userRows = useUsersRows({
		items: data?.items,
		dataUpdatedAt,
		page,
		rowsPerPage,
		showSkeleton: isLoading || isFetching,
		handleEdit,
		handlePageOpen: handleCrossLinkClick,
		hasUsersModuleAccess,
		hasManageAllInternsClaim,
	});
	const paginationProps = useMemo(
		() =>
			createGridPagination(
				userRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[userRows.length, page, rowsPerPage, data?.total, handleOnPerPageChange]
	);
	const handleEmailChange = useCallback(() => {
		setIsEmailDuplicate(false);
	}, []);
	const handleUserEditFormClose = useCallback(() => {
		setIsEmailDuplicate(false);
		handleEditClose();
	}, []);
	const handleToggleUserActive = useCallback(
		(row: UsersGridRow) => {
			openActivateDialog(row);
		},
		[openActivateDialog]
	);
	const handleConfirmToggleUserActive = useCallback(() => {
		if (!selectedUser) {
			return;
		}

		updateUserStatusMutation.mutate({
			id: selectedUser.id,
			firstName: selectedUser.firstName,
			lastName: selectedUser.lastName,
			isActive: !selectedUser.isActive,
		});

		closeActivateDialog();
	}, [selectedUser, updateUserStatusMutation, closeActivateDialog]);

	useEffect(() => setPage(0), [filters, sortColumns, rowsPerPage]);

	if (loading || isLoadingDepartments) {
		return null;
	}

	const canAddUser = isManagerRole(role) || hasHRModuleClaim;

	return (
		<PageWrapper>
			<PageHeader
				addButton={{
					onClick: handleAddOpen,
					isHidden: !canAddUser,
				}}
			/>
			<Loader isLoading={isLoading || isFetching}>
				<UsersGrid
					rows={userRows}
					{...{
						paginationProps,
						sortColumns,
						handleSortColumnChange,
						columnsWidths,
						options,
						filters,
						setFilters,
						initialFilters,
						hasUsersModuleAccess,
						handleToggleUserActive,
						handleCrossLinkClick,
						enableCellClick,
					}}
				/>
			</Loader>

			{isAddOpen && (
				<UserAddingForm
					{...{ hasHRModuleClaim }}
					onClose={handleAddClose}
					onSubmit={onUserCreate}
					isLoading={isUserCreating}
				/>
			)}

			{isEditOpen && editId !== undefined && (
				<UserForm
					handleClose={handleUserEditFormClose}
					initialValues={defaultValues}
					onSubmit={onUserUpdate}
					id={Number(editId)}
					onEmailChange={handleEmailChange}
					{...{ isEmailDuplicate, hasHRModuleClaim }}
				/>
			)}

			{isActivateDialogOpen && selectedUser && (
				<PopUp
					title={
						selectedUser.isActive
							? getDeactivatingPopUpTitle('User')
							: getActivatingPopUpTitle('User')
					}
					onClose={closeActivateDialog}
					type="approve"
					showCloseButton={false}
					controls={{
						positiveControl: { onClick: handleConfirmToggleUserActive },
						negativeControl: { onClick: closeActivateDialog },
					}}
				>
					<ActivationMessage
						isActive={selectedUser.isActive}
						name={selectedUser.fullName}
					/>
				</PopUp>
			)}
		</PageWrapper>
	);
}
