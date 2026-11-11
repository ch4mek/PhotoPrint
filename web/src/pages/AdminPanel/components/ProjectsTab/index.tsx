import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useQueryClient } from '@tanstack/react-query';
import { useMemo, useRef, useState, UIEvent } from 'react';
import { DataGridHandle } from 'react-data-grid';

import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { isChiefRole, isManagerRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useListScrollObserver } from '@/context/ListScrollObserver';
import { useToast } from '@/context/Toast';
import { ProjectForm } from '@/forms/ProjectForm';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getRandomColor } from '@/helpers/getRandomColor';
import { updateGridItems } from '@/helpers/updateGridItems';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useCreateProject } from '@/queries/useCreateProject';
import { PROJECT_QUERY_KEY } from '@/queries/useProject';
import { useProjects } from '@/queries/useProjects';
import { useUpdateProject } from '@/queries/useUpdateProject';
import { useUpdateProjectStatus } from '@/queries/useUpdateProjectStatus';
import { ProjectsGridRow } from '@/types/project';
import { UserOnProject } from '@/types/userInfo';

import 'react-data-grid/lib/styles.css';
import { getProjectColumnsWidths } from './constants';
import { useProjectsFilters } from './hooks/useProjectsFilters';
import { useProjectsColumns } from './hooks/useProjectsHeaders';
import { useProjectsRows } from './hooks/useProjectsValues';
import { ProjectsDataGridHoverRowWrapper } from './styles';

const PROJECT_INDEX_EDIT_COLUMN = 11;

const getRowKey = (row: ProjectsGridRow) => row.id;

export function ProjectsTab() {
	const queryClient = useQueryClient();
	const toast = useToast();
	const { id, handleOpen, handleClose, isOpen, updateRecord } = useShowModal();
	const listScrollObserver = useListScrollObserver();

	const [resId, setResId] = useState<UserOnProject[] | null>();
	const { role } = useAuth();

	const { filters, setFilters } = useProjectsFilters();

	const {
		page,
		setPage,
		rowsPerPage,
		handleOnPerPageChange,
		order,
		sortColumns,
		handleSortColumnChange,
		orderBy,
	} = useTableState({
		resetPageDeps: [filters],
	});
	const gridTemplateRef = useRef<DataGridHandle>(null);

	const {
		projectFilter,
		clientFilter,
		factTrackedTimeFilter,
		billableTrackedTimeFilter,
		statusFilter,
		publicFilter,
	} = filters;

	const projectColumns = useProjectsColumns({
		...filters,
		setFilters,
		displayTrackedTimeColumns: isChiefRole(role),
	});

	const { isLoading, data, dataUpdatedAt, isFetching, refetch } = useProjects(
		page + 1,
		Number(rowsPerPage),
		order,
		orderBy,
		filters
	);

	const handleEdit = (itemdId?: string | number) => () => {
		updateRecord(undefined, itemdId);
	};

	const createProjectMutation = useCreateProject(
		{
			onSuccess: () => {
				refetch();
			},
		},
		toast
	);

	const updateProjectMutation = useUpdateProject(
		{
			onSuccess: (updatedProjectItem) => {
				queryClient.setQueryData(
					[
						'projects',
						page + 1,
						rowsPerPage,
						order,
						orderBy,
						projectFilter,
						clientFilter,
						factTrackedTimeFilter,
						billableTrackedTimeFilter,
						statusFilter,
						publicFilter,
					],
					(prev: unknown) =>
						updateGridItems(prev, updatedProjectItem, orderBy, order)
				);
				queryClient.setQueryData(
					[PROJECT_QUERY_KEY, updatedProjectItem.id],
					updatedProjectItem
				);

				refetch();
			},
		},
		toast
	);

	const updateProjectStatusMutation = useUpdateProjectStatus(
		{
			onSuccess: (res) => {
				setResId(res.projectUsers);
				refetch();
			},
		},
		toast
	);

	const defaultValues = useMemo(() => {
		const item = data?.items.find((project) => project.id === id);

		if (id && item) {
			const {
				name,
				clientName,
				clientId,
				isPublic,
				isBillable,
				projectUsers,
				color,
				isActive,
				managerId,
				managerFullName,
			} = item;

			return {
				name,
				client: {
					id: clientId!,
					name: clientName,
				},
				isPublic,
				isBillable,
				users: [],
				projectUsers: projectUsers.map(({ userId }) => ({
					userId: Number(userId),
				})),
				color,
				isActive,
				manager:
					managerId && managerFullName
						? {
								id: managerId,
								fullName: managerFullName,
						  }
						: null,
			};
		}

		return {
			name: '',
			client: null,
			isPublic: false,
			isBillable: false,
			users: [],
			color: getRandomColor(),
			isActive: true,
			manager: null,
		};
	}, [isOpen, id]);

	const projectRows = useProjectsRows(
		data?.items,
		dataUpdatedAt,
		page,
		rowsPerPage,
		isFetching,
		isLoading,
		handleEdit
	);

	const handleGridScroll = (event: UIEvent<HTMLDivElement>) => {
		const { scrollTop, clientHeight, scrollHeight } =
			event.target as HTMLElement;
		const bottomRowsOffsetCount = 2;

		const startIdx = Math.floor(scrollTop / DEFAULT_GRID_ROW_HEIGHT);
		const visibleRowCount = Math.ceil(clientHeight / DEFAULT_GRID_ROW_HEIGHT);
		const endIdx =
			Math.min(
				startIdx + visibleRowCount,
				scrollHeight / DEFAULT_GRID_ROW_HEIGHT
			) - bottomRowsOffsetCount;
		listScrollObserver.notifyScrollListeners(startIdx, endIdx);
	};

	const overrideProjectsGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			getProjectColumnsWidths(isChiefRole(role)),
			true
		);

		return overrideGridStylesWithEditColumn(
			!!projectRows.length,
			PROJECT_INDEX_EDIT_COLUMN,
			gridColumnTemplate
		);
	}, [projectRows.length]);

	const projectGridPagination = useMemo(
		() =>
			createGridPagination(
				projectRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				data?.total
			),
		[projectRows.length, page, rowsPerPage, data?.total]
	);

	return (
		<PageWrapper>
			<PageHeader
				addButton={{ onClick: handleOpen, isHidden: !isManagerRole(role) }}
			/>
			<Loader isLoading={isLoading || isFetching}>
				<ProjectsDataGridHoverRowWrapper defineHeight={false}>
					<DataGrid
						{...COMMON_GRID_PROPS}
						enableVirtualization
						columns={projectColumns}
						rows={projectRows}
						ref={gridTemplateRef}
						defaultColumnOptions={{
							sortable: true,
						}}
						rowKeyGetter={getRowKey}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideProjectsGridStyles}
						paginationProps={projectGridPagination}
						renderers={noRowsGridFallback}
						hideVerticalScrollbar={
							projectRows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT &&
							(isLoading || isFetching)
						}
						onScroll={handleGridScroll}
						scrollToTopOnRowsChange={false}
					/>
				</ProjectsDataGridHoverRowWrapper>
			</Loader>

			{isOpen && (
				<ProjectForm
					resId={resId}
					handleClose={handleClose}
					initialValues={defaultValues}
					onChangeStatus={updateProjectStatusMutation.mutateAsync}
					onSubmit={
						id
							? (updateProjectMutation.mutateAsync as any)
							: createProjectMutation.mutateAsync
					}
					id={id as number}
				/>
			)}
		</PageWrapper>
	);
}
