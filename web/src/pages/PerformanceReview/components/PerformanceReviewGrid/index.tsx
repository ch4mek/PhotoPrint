import { memo, useCallback, useEffect, useMemo } from 'react';

import { Loader } from '@/components/Loader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { PopUpSearchParams } from '@/constants/searchParams';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { AddingPerformanceReviewForm } from '@/forms/PerformanceReviewForms';
import { ConversationForm } from '@/forms/TeamboardForm/Conversation';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createToast } from '@/helpers/createToast';
import { getInfoDateTagContent } from '@/helpers/getInfoDateTagContent';
import { usePersonalInfoPopupGuard } from '@/hooks/usePersonalInfoPopupGuard';
import { useTableState } from '@/pages/hooks/useTableState';
import { useSetAllUsers } from '@/pages/PerformanceReview/components/PerformanceReviewGrid/hooks/useSetAllUsers';
import { PerformanceReviewWarning } from '@/pages/PerformanceReview/components/PerformanceReviewWarning';
import { usePerformanceReviewModals } from '@/pages/PerformanceReview/hooks/usePerformanceReviewModalsState';
import { PersonalDataContext } from '@/pages/TeamBoardUser/context/personalDataContext';
import { useConversationFormState } from '@/pages/TeamBoardUser/hooks/useConversationFormState';
import { PersonalDataContextValues } from '@/pages/TeamBoardUser/types';
import { useGetPerformanceReview } from '@/queries/performanceReview';
import { createSelector } from '@/stores/helpers';
import { usePerformanceReviewStore } from '@/stores/performanceReview/store';
import { PerformanceReviewUserData } from '@/types/performanceReview';

import { PerformanceReviewTable } from './components/PerformanceReviewTable';
import { getPerformanceReviewColumnWidth } from './constants';
import { useGetGridColumns } from './hooks/useGetGridColumns';
import { usePerformanceReviewFilters } from './hooks/usePerformanceReviewFilters';
import { PerformanceReviewGridRow, PerformanceReviewOptions } from './types';
import { generateColumnOptions } from './utils/generateColumnOptions';
import { generateDataGridRows } from './utils/generateDataGridRows';
import { overridePerformanceReviewGridStyles } from './utils/overridePerfomanceReviewGridStyles';

function PerformanceReviewGridComponent() {
	const toast = useToast();
	const { role } = useAuth();

	const canOpenProfile = usePersonalInfoPopupGuard();
	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const {
		configurableValues: { departmentTypes },
	} = useConfiguration();

	const {
		handleUserDataUpdate,
		performanceUserData,
		isOpenWarning,
		handleOpenWarning,
		handleCloseWarning,
		isOpenPerformanceAdding,
		handleClosePerformanceAdding,
		handleOpenPerformanceAdding,
		initialPerformanceValue,
		initialConversationValue,
		handlePerformanceDataUpdate,
		setIsInitialDataDisabled,
		isInitialDataDisabled,
	} = usePerformanceReviewModals();

	const { filters, setFilters } = usePerformanceReviewFilters();

	const {
		page,
		setPage,
		rowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		resetPageDeps: [filters],
	});

	const { users, isShowIssuedStatus } = usePerformanceReviewStore(
		createSelector('users', 'isShowIssuedStatus')
	);

	const options: PerformanceReviewOptions = useMemo(
		() => generateColumnOptions(departmentTypes, users),
		[users, departmentTypes]
	);

	const { data, isLoading, isFetching, error } = useGetPerformanceReview({
		top: rowsPerPage,
		page,
		order,
		orderBy,
		filters,
		allDepartments: options.departments,
		issuedOnly: isShowIssuedStatus,
	});

	const {
		id,
		handleAddClick,
		handleConservationEditClick,
		handleFormSubmit,
		handleClose,
		isOpen,
	} = useConversationFormState(
		undefined,
		performanceUserData?.id,
		handleOpenWarning
	);

	useSetAllUsers();

	useEffect(() => {
		if (error) {
			toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));
		}
	}, [error]);

	const handleTagChoose = useCallback(
		(
				userData: PerformanceReviewUserData,
				date: string,
				performanceReviewId: number,
				conversationId?: number
			) =>
			() => {
				handleUserDataUpdate(userData);

				if (conversationId) {
					handleConservationEditClick(conversationId)();
				} else {
					handleAddClick();
					handlePerformanceDataUpdate(date, performanceReviewId);
				}
			},
		[]
	);

	const handleAddPerformanceButtonClick = useCallback(
		(row: PerformanceReviewGridRow) => {
			const { id: userId, fullName } = row;

			handleUserDataUpdate({
				id: userId,
				fullName,
			});
			setIsInitialDataDisabled(false);
			handleOpenPerformanceAdding();
		},
		[]
	);

	const handleCrossLinkClick = useCallback((row: PerformanceReviewGridRow) => {
		handlePageOpen(row.id.toString());
	}, []);

	const gridColumnWidths = useMemo(
		() => getPerformanceReviewColumnWidth(role),
		[role]
	);

	const infoDateTagContent = getInfoDateTagContent();

	const dataGridColumns = useGetGridColumns({
		filters,
		filterSetter: setFilters,
		options,
		handleTagChoose,
		handleContentClick: handleCrossLinkClick,
		gridColumnsWidths: gridColumnWidths,
		handleIconClick: handleAddPerformanceButtonClick,
		enableCellClick: (row) => canOpenProfile(row.id),
		infoDateTagContent,
	});

	const dataGridRows = useMemo(
		() => generateDataGridRows(departmentTypes, data?.items),
		[data, departmentTypes]
	);

	const gridPagination = useMemo(
		() =>
			createGridPagination(
				dataGridRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[dataGridRows.length, page, rowsPerPage, data?.total]
	);

	const overrideGridStyles = useMemo(
		() =>
			overridePerformanceReviewGridStyles(
				!!dataGridRows.length,
				gridColumnWidths,
				role
			),
		[dataGridRows.length, role, gridColumnWidths]
	);

	const personalDataContextValue = useMemo(
		() =>
			({
				userId: performanceUserData?.id,
			} as PersonalDataContextValues),
		[performanceUserData?.id]
	);

	return (
		<Loader isLoading={isLoading || isFetching}>
			<PerformanceReviewTable
				{...{
					dataGridColumns,
					sortColumns,
					dataGridRows,
					overrideGridStyles,
					gridPagination,
					handleSortColumnChange,
				}}
			/>

			{isOpen && (
				<PersonalDataContext.Provider value={personalDataContextValue}>
					<ConversationForm
						handleClose={handleClose}
						onSubmit={handleFormSubmit}
						initialValues={initialConversationValue}
						id={id}
					/>
				</PersonalDataContext.Provider>
			)}

			{isOpenWarning && (
				<PerformanceReviewWarning
					handleToggleDialog={handleCloseWarning}
					handleOpenAddingPopup={handleOpenPerformanceAdding}
				/>
			)}
			{isOpenPerformanceAdding && (
				<AddingPerformanceReviewForm
					onClose={handleClosePerformanceAdding}
					initialValues={initialPerformanceValue}
					isInitialValueDisabled={isInitialDataDisabled}
				/>
			)}
		</Loader>
	);
}

export const PerformanceReviewGrid = memo(PerformanceReviewGridComponent);
