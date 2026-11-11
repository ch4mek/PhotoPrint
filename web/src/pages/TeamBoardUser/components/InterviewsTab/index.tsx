import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useContext, useMemo, useState } from 'react';

import { PersonalDataContext } from '../../context/personalDataContext';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { InterviewForm } from '@/forms/InterviewForm';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getFullName } from '@/helpers/utils';
import { useTableState } from '@/pages/hooks/useTableState';
import { InterviewGrid } from '@/pages/Staffing/components/InterviewTab/components/InterviewGrid';
import {
	DEFAULT_INTERVIEW_GRID_FILTERS,
	INTERVIEW_DELETE_CONFIRMATION_MESSAGE,
	INTERVIEW_ENTITY_NAME,
} from '@/pages/Staffing/components/InterviewTab/constants';
import { useInterviewGridOptions } from '@/pages/Staffing/components/InterviewTab/hooks/useInterviewGridOptions';
import { useInterviewModals } from '@/pages/Staffing/components/InterviewTab/hooks/useInterviewModals';
import { InterviewGridRow } from '@/pages/Staffing/components/InterviewTab/types';
import { convertInterviewsDataIntoRows } from '@/pages/Staffing/components/InterviewTab/utils/convertInterviewsDataIntoRows';
import { useGetInterviews } from '@/queries/interview/useGetInterviews';
import { useSaleManagersLookup } from '@/queries/interview/useSaleManagersLookup';

import {
	ADD_BUTTON_TITLE,
	columnsWidths,
	INITIAL_ORDER_BY_VALUE,
} from './constants';
import { useInterviewFilters } from './hooks/useInterviewFilters';
import { buildInterviewOrderBy } from './utils';

export function InterviewsTab() {
	const {
		userId,
		data: { firstName, lastName, firstNameRu, lastNameRu },
	} = useContext(PersonalDataContext);

	const {
		id: currentUserId,
		role: currentUserRole,
		departmentId: currentUserDepartmentId,
		resourceGroupId: currentUserResourceGroupId,
	} = useAuth();

	const { filters, setFilters, isResetFiltersDisabled } = useInterviewFilters();

	const {
		page,
		setPage,
		rowsPerPage,
		sortColumns,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
		resetPageDeps: [filters],
	});

	const orderBy = useMemo(
		() => buildInterviewOrderBy(sortColumns),
		[sortColumns]
	);

	const { data: managers, isLoading: isLoadingSaleManagers } =
		useSaleManagersLookup({ onlyActive: true });

	const { defaultOptions } = useInterviewGridOptions({ managers });

	const interviewQueryFilters = useMemo(
		() => ({
			...filters,
			user: DEFAULT_INTERVIEW_GRID_FILTERS.user,
			userId: String(userId),
		}),
		[filters, userId]
	);

	const { data: interviews, isFetching: isLoadingInterviews } =
		useGetInterviews(rowsPerPage, page, orderBy, interviewQueryFilters, {
			enabled: !!userId,
		});

	const {
		addStageModal,
		deleteInterviewModal,
		mainInterviewModal,
		stageInterviewModal,
	} = useInterviewModals(interviews);

	const [expandedIds, setExpandedIds] = useState<Set<number>>(new Set());

	const rows = useMemo<InterviewGridRow[]>(
		() =>
			convertInterviewsDataIntoRows(
				expandedIds,
				mainInterviewModal.open,
				stageInterviewModal.open,
				deleteInterviewModal.open,
				addStageModal.open,
				interviews,
				currentUserId,
				currentUserRole,
				currentUserDepartmentId,
				currentUserResourceGroupId
			),
		[
			interviews,
			expandedIds,
			currentUserId,
			currentUserRole,
			currentUserDepartmentId,
			currentUserResourceGroupId,
			mainInterviewModal.open,
			stageInterviewModal.open,
			deleteInterviewModal.open,
			addStageModal.open,
		]
	);

	const handleToggleExpand = (id: number) => {
		setExpandedIds((prev) => {
			const next = new Set(prev);

			if (next.has(id)) {
				next.delete(id);
			} else {
				next.add(id);
			}

			return next;
		});
	};

	const paginationProps = useMemo(
		() =>
			createGridPagination(
				rows.length,
				page,
				setPage,
				Number(rowsPerPage),
				handleOnPerPageChange,
				interviews?.total ?? 0
			),
		[
			rows.length,
			page,
			setPage,
			rowsPerPage,
			handleOnPerPageChange,
			interviews?.total,
		]
	);

	const gridFilters = useMemo(
		() => ({
			...filters,
			user: DEFAULT_INTERVIEW_GRID_FILTERS.user,
		}),
		[filters]
	);

	const gridSetFilters = useMemo(
		() => ({
			...setFilters,
			user: () => {},
		}),
		[setFilters]
	);

	const preselectedUser = useMemo(() => {
		if (userId) {
			return {
				id: userId,
				name: getFullName(firstName, lastName, firstNameRu, lastNameRu),
			};
		}
	}, [userId, firstName, lastName, firstNameRu, lastNameRu]);

	return (
		<>
			<TabTableWrapper>
				<TabHeader
					onAddClick={mainInterviewModal.open}
					buttonTitle={ADD_BUTTON_TITLE}
				/>
				<Loader
					isLoading={isLoadingSaleManagers || isLoadingInterviews}
					size="small"
				>
					<InterviewGrid
						onToggleExpand={handleToggleExpand}
						options={defaultOptions}
						filters={gridFilters}
						setFilters={gridSetFilters}
						{...{
							rows,
							columnsWidths,
							sortColumns,
							handleSortColumnChange,
							paginationProps,
							isResetFiltersDisabled,
						}}
					/>
				</Loader>
			</TabTableWrapper>

			{mainInterviewModal.isOpen && (
				<InterviewForm
					onClose={mainInterviewModal.close}
					interview={mainInterviewModal.interview}
					preselectedUser={preselectedUser}
				/>
			)}
			{stageInterviewModal.isOpen && (
				<InterviewForm
					onClose={stageInterviewModal.close}
					interview={stageInterviewModal.interview}
					preselectedUser={preselectedUser}
				/>
			)}
			{deleteInterviewModal.isOpen && deleteInterviewModal.interview && (
				<DeleteConfirmationDialog
					title={INTERVIEW_ENTITY_NAME}
					handleClose={deleteInterviewModal.close}
					handleSubmit={deleteInterviewModal.submit}
					isLoading={deleteInterviewModal.isLoading}
					content={
						<Typography variant="m-400">
							{INTERVIEW_DELETE_CONFIRMATION_MESSAGE}
							<Typography variant="m-600">
								{' '}
								{deleteInterviewModal.interview?.name}
							</Typography>
							?
						</Typography>
					}
					showConfirmationMessage={false}
				/>
			)}
			{addStageModal.isOpen && (
				<InterviewForm
					onClose={addStageModal.close}
					parentInterview={addStageModal.parentInterview}
					preselectedUser={preselectedUser}
				/>
			)}
		</>
	);
}
