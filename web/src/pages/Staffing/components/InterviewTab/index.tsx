import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useMemo, useState } from 'react';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { DEFAULT_ROWS_PER_PAGE } from '@/constants/pagination';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { InterviewForm } from '@/forms/InterviewForm';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetInterviews } from '@/queries/interview/useGetInterviews';
import { useInterviewUsersLookup } from '@/queries/interview/useInterviewUsersLookup';
import { useSaleManagersLookup } from '@/queries/interview/useSaleManagersLookup';

import { InterviewGrid } from './components/InterviewGrid';
import {
	getInterviewColumns,
	INITIAL_ORDER_BY_VALUE,
	INTERVIEW_DELETE_CONFIRMATION_MESSAGE,
	INTERVIEW_ENTITY_NAME,
	INTERVIEW_GRID_COLUMNS,
} from './constants';
import { useInterviewFilters } from './hooks/useInterviewFilters';
import { useInterviewGridOptions } from './hooks/useInterviewGridOptions';
import { useInterviewModals } from './hooks/useInterviewModals';
import { InterviewGridRow } from './types';
import { convertInterviewsDataIntoRows } from './utils/convertInterviewsDataIntoRows';
import { getOrderBySaleManager } from './utils/getOrderBySaleManager';

export function InterviewTab() {
	const {
		id: currentUserId,
		role: currentUserRole,
		departmentId: currentUserDepartmentId,
		resourceGroupId: currentUserResourceGroupId,
	} = useAuth();

	const { filters, setFilters } = useInterviewFilters();

	const [expandedIds, setExpandedIds] = useState<Set<number>>(new Set());

	const {
		sortColumns,
		rowsPerPage,
		page,
		setPage,
		handleSortColumnChange,
		handleOnPerPageChange,
	} = useTableState({
		initialRowsPerPage: Number(DEFAULT_ROWS_PER_PAGE),
		initialOrder: SortDirection.DESC,
		initialOrderBy: INITIAL_ORDER_BY_VALUE,
		resetPageDeps: [filters],
	});

	const { data: users, isLoading: isLoadingUsers } = useInterviewUsersLookup();
	const { data: managers, isLoading: isLoadingSaleManagers } =
		useSaleManagersLookup({ onlyActive: true });

	const { defaultOptions } = useInterviewGridOptions({ users, managers });

	const orderBy = sortColumns
		.map((col) => {
			if (col.columnKey === INTERVIEW_GRID_COLUMNS.date.key) {
				return `userInterviewDate ${col.direction}, userInterviewFromUtc ${SortDirection.ASC}`;
			}

			if (col.columnKey === INTERVIEW_GRID_COLUMNS.time.key) {
				return `userInterviewFromUtc ${col.direction}`;
			}

			if (col.columnKey === INTERVIEW_GRID_COLUMNS.user.key) {
				return `userFullNameRu eq null ${SortDirection.ASC}, userFullNameRu ${col.direction}`;
			}

			if (col.columnKey === INTERVIEW_GRID_COLUMNS.type.key) {
				return `interviewTypeOrderWeight ${col.direction}`;
			}

			if (col.columnKey === INTERVIEW_GRID_COLUMNS.status.key) {
				return `statusOrderWeight ${col.direction}`;
			}

			if (col.columnKey === INTERVIEW_GRID_COLUMNS.saleManager.key) {
				return getOrderBySaleManager(col.direction);
			}

			return `${col.columnKey} ${col.direction}`;
		})
		.join(',');

	const { data: interviews, isFetching: isLoadingInterviews } =
		useGetInterviews(Number(rowsPerPage), page, orderBy, filters);

	const {
		addStageModal,
		deleteInterviewModal,
		mainInterviewModal,
		stageInterviewModal,
	} = useInterviewModals(interviews);

	const columnsWidths = useMemo(() => getInterviewColumns(), []);

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

	const paginationProps = createGridPagination(
		rows.length,
		page,
		setPage,
		Number(rowsPerPage),
		handleOnPerPageChange,
		interviews?.total ?? 0
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

	return (
		<PageWrapper>
			<PageHeader addButton={{ onClick: mainInterviewModal.open }} />

			<Loader
				isLoading={
					isLoadingUsers || isLoadingSaleManagers || isLoadingInterviews
				}
			>
				<InterviewGrid
					options={defaultOptions}
					onToggleExpand={handleToggleExpand}
					{...{
						rows,
						columnsWidths,
						sortColumns,
						handleSortColumnChange,
						paginationProps,
						filters,
						setFilters,
					}}
				/>
			</Loader>

			{mainInterviewModal.isOpen && (
				<InterviewForm
					onClose={mainInterviewModal.close}
					interview={mainInterviewModal.interview}
				/>
			)}
			{stageInterviewModal.isOpen && (
				<InterviewForm
					onClose={stageInterviewModal.close}
					interview={stageInterviewModal.interview}
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
				/>
			)}
		</PageWrapper>
	);
}
