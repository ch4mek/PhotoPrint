import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useCallback, useMemo, useRef } from 'react';
import { DataGridHandle } from 'react-data-grid';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import {
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { useOpenPopUpViaUrl } from '@/components/PopUpUrlInterceptor/hooks/useOpenPopUpViaUrl';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { PopUpSearchParams } from '@/constants/searchParams';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { ProjectExitForm } from '@/forms/ProjectExitForm';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { createGridPagination } from '@/helpers/createGridPagination';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useGetProjectExits } from '@/queries/projectExits/useGetProjectExits';
import { UserClaims } from '@/types/claims';
import { ProjectExitsGridRow } from '@/types/projectExits';

import {
	DELETE_DIALOG_CONTENT,
	DELETE_DIALOG_TITLE,
	projectExitGridColumns,
	RESTORE_DIALOG_CONTENT,
	RESTORE_DIALOG_TITLE,
} from './constants';
import { useFilterOptions } from './hooks/useFilterOptions';
import { useProjectExitsColumns } from './hooks/useProjectExitsColumns';
import { useProjectExitsFilters } from './hooks/useProjectExitsFilters';
import { useProjectExitsRows } from './hooks/useProjectExitsRows';
import { useProjectMenuOptions } from './hooks/useProjectMenuOptions';
import { getRowKey } from './utils';

import 'react-data-grid/lib/styles.css';

const PROJECT_INDEX_EDIT_COLUMN = 11;

export function ProjectExitsTab() {
	const { claims } = useAuth();
	const hasManageAllProjectExitsClaim = checkIsHasClaims(
		[UserClaims.ManageAllProjectExits],
		claims
	);

	const { filters, setFilters, isResetFiltersDisabled } =
		useProjectExitsFilters();
	const {
		id: projectExitToEditId,
		handleOpen: handleOpenProjectExit,
		handleClose: handleCloseProjectExit,
		isOpen: isProjectExitOpen,
	} = useShowModal();

	const {
		rowIdToDelete,
		isDeleteDialogOpen,
		isProjectDeleting,
		handleDelete,
		handleConfirmDelete,
		handleCloseDeleteDialog,

		rowIdToRestore,
		isRestoreDialogOpen,
		isProjectRestoring,
		handleRestore,
		handleConfirmRestore,
		handleCloseRestoreDialog,
	} = useProjectMenuOptions();

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
		initialOrder: SortDirection.DESC,
		initialOrderBy: projectExitGridColumns.closingDate.key,
	});
	const gridTemplateRef = useRef<DataGridHandle>(null);
	const { options, allDepartmentsLength } = useFilterOptions();

	const handlePageOpen = useOpenPopUpViaUrl(PopUpSearchParams.UserIdParam);

	const handleCrosslinkClick = useCallback(
		(row: ProjectExitsGridRow) => {
			handlePageOpen(row.userId.toString());
		},
		[handlePageOpen]
	);

	const handleEditProjectExit = useCallback(
		(row: ProjectExitsGridRow) => {
			handleOpenProjectExit(row.id);
		},
		[handleOpenProjectExit]
	);

	const { columns, projectExitsColumnsWidths } = useProjectExitsColumns({
		filters,
		setFilters,
		isResetFiltersDisabled,
		options,
		canManageProjectExits: hasManageAllProjectExitsClaim,
		onCellClick: handleCrosslinkClick,
		handleDelete,
		handleRestore,
		handleEdit: handleEditProjectExit,
	});

	const { isLoading, data, dataUpdatedAt, isFetching } = useGetProjectExits(
		page + 1,
		Number(rowsPerPage),
		order,
		orderBy,
		filters,
		allDepartmentsLength
	);

	const isDataLoading = isLoading || isFetching;

	const rows = useProjectExitsRows({
		items: data?.items,
		dataUpdatedAt,
		page,
		rowsPerPage,
		isDataLoading,
	});

	const overrideProjectsGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			projectExitsColumnsWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!rows.length,
			PROJECT_INDEX_EDIT_COLUMN,
			gridColumnTemplate
		);
	}, [rows.length, projectExitsColumnsWidths]);

	const projectGridPagination = useMemo(
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

	const projectExit = useMemo(
		() =>
			projectExitToEditId
				? data?.items.find((exit) => exit.id === projectExitToEditId)
				: undefined,
		[projectExitToEditId, data]
	);

	return (
		<PageWrapper>
			<PageHeader
				addButton={{
					onClick: handleOpenProjectExit,
					isHidden: !hasManageAllProjectExitsClaim,
				}}
			/>
			<Loader isLoading={isDataLoading}>
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...COMMON_GRID_PROPS}
						{...{ rows, columns, sortColumns }}
						ref={gridTemplateRef}
						defaultColumnOptions={{
							sortable: true,
						}}
						rowKeyGetter={getRowKey}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideProjectsGridStyles}
						paginationProps={projectGridPagination}
						renderers={noRowsGridFallback}
						hideVerticalScrollbar={
							rows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT
						}
						scrollToTopOnRowsChange={false}
					/>
				</DataGridHoverRowWrapper>
			</Loader>

			{isProjectExitOpen && (
				<ProjectExitForm
					onClose={handleCloseProjectExit}
					projectExit={projectExit}
				/>
			)}

			{isDeleteDialogOpen && rowIdToDelete && (
				<DeleteConfirmationDialog
					title={DELETE_DIALOG_TITLE}
					handleSubmit={handleConfirmDelete}
					handleClose={handleCloseDeleteDialog}
					content={
						<Typography variant="m-400">{DELETE_DIALOG_CONTENT}</Typography>
					}
					showConfirmationMessage={false}
					isLoading={isProjectDeleting}
				/>
			)}

			{isRestoreDialogOpen && rowIdToRestore && (
				<DeleteConfirmationDialog
					title={RESTORE_DIALOG_TITLE}
					handleSubmit={handleConfirmRestore}
					handleClose={handleCloseRestoreDialog}
					content={
						<Typography variant="m-400">{RESTORE_DIALOG_CONTENT}</Typography>
					}
					showConfirmationMessage={false}
					isLoading={isProjectRestoring}
					customTitle
				/>
			)}
		</PageWrapper>
	);
}
