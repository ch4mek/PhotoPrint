import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { useCallback, useMemo } from 'react';

import { DeleteConfirmationDialog } from '@/components/DeleteConfirmationDialog';
import {
	getGridColumnTemplateFromColumnWidths,
	overrideGridStylesWithEditColumn,
} from '@/components/Grid';
import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { COMMON_GRID_PROPS } from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { TabHeader } from '@/components/TabHeader';
import { TabTableWrapper } from '@/components/TabWrapper';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	JIRA_SETTING_DELETED,
	NotificationType,
} from '@/constants/notifications';
import { SortDirection } from '@/constants/sort';
import { useAuth } from '@/context/AuthContext';
import { useToast } from '@/context/Toast';
import { JiraSettingsForm } from '@/forms/JiraSettingsForm';
import { createGridPagination } from '@/helpers/createGridPagination';
import { createToast } from '@/helpers/createToast';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useDeleteJiraSettings } from '@/queries/jira/useDeleteJiraSettings';
import { useGetUserJiraSettings } from '@/queries/jira/useGetUserJiraSettings';
import { JiraSettingsGridRow } from '@/types/jira';

import {
	ADD_BUTTON_TITLE,
	DELETE_DIALOG_CONTENT,
	DELETE_DIALOG_TITLE,
	DEFAULT_JIRA_SETTINGS_ORDER_BY_FIELD,
	jiraSettingsColumnsWidths,
	JIRA_SETTINGS_EDIT_COLUMN_WIDTH,
} from './constants';
import { useJiraSettingsColumns } from './hooks/useJiraSettingsColumns';
import { useJiraSettingsFilters } from './hooks/useJiraSettingsFilters';
import { useJiraSettingsRows } from './hooks/useJiraSettingsRows';
import { buildJiraSettingsOrderBy, getRowKey } from './utils';

export function JiraSettingsTab() {
	const { id } = useAuth();
	const toast = useToast();
	const {
		handleOpen: handleJiraSettingsOpen,
		isOpen: isJiraSettingsOpen,
		handleClose: handleJiraSettingsClose,
		id: jiraSettingsId,
	} = useShowModal<number>();

	const {
		handleOpen: handleDeleteJiraSettingsOpen,
		isOpen: isDeleteJiraSettingsOpen,
		handleClose: handleDeleteJiraSettingsClose,
		id: jiraSettingsIdToDelete,
	} = useShowModal<number>();

	const { filters, setFilters, isResetFiltersDisabled } =
		useJiraSettingsFilters();
	const { mutateAsync: deleteJiraSettings, isPending: isJiraSettingsDeleting } =
		useDeleteJiraSettings({
			onSuccess: () => {
				toast.open(createToast(NotificationType.SUCCESS, JIRA_SETTING_DELETED));
				handleDeleteJiraSettingsClose();
			},
		});

	const {
		rowsPerPage,
		handleOnPerPageChange,
		page,
		setPage,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		initialOrderBy: DEFAULT_JIRA_SETTINGS_ORDER_BY_FIELD,
		initialOrder: SortDirection.DESC,
		resetPageDeps: [filters],
	});

	const orderBy = buildJiraSettingsOrderBy(sortColumns);

	const { data, dataUpdatedAt, isLoading } = useGetUserJiraSettings({
		userId: id,
		top: rowsPerPage,
		skip: page * rowsPerPage,
		filters,
		orderBy,
	});

	const rows = useJiraSettingsRows({
		items: data?.items,
		dataUpdatedAt,
		page,
		rowsPerPage,
	});

	const handleEditJiraSettings = useCallback(
		(row: JiraSettingsGridRow) => {
			handleJiraSettingsOpen(row.id);
		},
		[handleJiraSettingsOpen]
	);

	const handleDeleteJiraSettings = useCallback(
		(row: JiraSettingsGridRow) => {
			handleDeleteJiraSettingsOpen(row.id);
		},
		[handleDeleteJiraSettingsOpen]
	);

	const columns = useJiraSettingsColumns({
		filters,
		isResetFiltersDisabled,
		setFilters,
		handleEdit: handleEditJiraSettings,
		handleDelete: handleDeleteJiraSettings,
	});

	const overrideJiraSettingsGridStyles = useMemo(() => {
		const gridColumnTemplate = getGridColumnTemplateFromColumnWidths(
			jiraSettingsColumnsWidths,
			true
		);

		return overrideGridStylesWithEditColumn(
			!!rows.length,
			columns.length + 1,
			gridColumnTemplate,
			JIRA_SETTINGS_EDIT_COLUMN_WIDTH
		);
	}, [rows.length, jiraSettingsColumnsWidths, columns.length]);

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
		[rows.length, page, handleOnPerPageChange, rowsPerPage, data?.total]
	);

	const jiraSettingsToEdit = useMemo(
		() => data?.items.find((item) => item.id === jiraSettingsId),
		[data, jiraSettingsId]
	);

	const handleDeleteJiraSettingsSubmit = useCallback(async () => {
		if (!jiraSettingsIdToDelete) {
			return;
		}

		await deleteJiraSettings(jiraSettingsIdToDelete);
	}, [jiraSettingsIdToDelete, deleteJiraSettings]);

	return (
		<TabTableWrapper>
			<TabHeader
				onAddClick={handleJiraSettingsOpen}
				buttonTitle={ADD_BUTTON_TITLE}
			/>

			<Loader {...{ isLoading }} size="small">
				<DataGridHoverRowWrapper defineHeight={false} flexGrow>
					<DataGrid
						{...COMMON_GRID_PROPS}
						{...{ columns, rows, sortColumns, paginationProps }}
						overrideDataGridStyles={overrideJiraSettingsGridStyles}
						onSortColumnsChange={handleSortColumnChange}
						renderers={noRowsGridFallback}
						rowKeyGetter={getRowKey}
					/>
				</DataGridHoverRowWrapper>
			</Loader>

			{isJiraSettingsOpen && (
				<JiraSettingsForm
					onClose={handleJiraSettingsClose}
					jiraSettings={jiraSettingsToEdit}
				/>
			)}

			{isDeleteJiraSettingsOpen && jiraSettingsIdToDelete && (
				<DeleteConfirmationDialog
					title={DELETE_DIALOG_TITLE}
					handleSubmit={handleDeleteJiraSettingsSubmit}
					handleClose={handleDeleteJiraSettingsClose}
					content={DELETE_DIALOG_CONTENT}
					showConfirmationMessage={false}
					isLoading={isJiraSettingsDeleting}
				/>
			)}
		</TabTableWrapper>
	);
}
