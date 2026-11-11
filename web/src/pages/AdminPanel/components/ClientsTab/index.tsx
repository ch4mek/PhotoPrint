import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useEffect, useMemo, useState } from 'react';
import { RowsChangeData } from 'react-data-grid';

import {
	FromToProps,
	getGridColumnTemplateFromColumnWidths,
} from '@/components/Grid';
import {
	CLIENTS_PROJECT_GRID_ROW_HEIGHT,
	COMMON_GRID_PROPS,
	DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT,
} from '@/components/Grid/constants';
import { Loader } from '@/components/Loader';
import { PageHeader } from '@/components/PageHeader';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import { ClientQuerieKeys } from '@/constants/queriesKeys';
import { isChiefRole, isHeadOrGroupManager } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import { useConfiguration } from '@/context/Configuration';
import { useToast } from '@/context/Toast';
import { ClientForm } from '@/forms/ClientForm';
import { ClientFormSubmitValues } from '@/forms/ClientForm/types';
import { createGridPagination } from '@/helpers/createGridPagination';
import { getRandomColor } from '@/helpers/getRandomColor';
import { updateGridItems } from '@/helpers/updateGridItems';
import {
	IS_ACTIVE_DEFAULT_VALUE,
	TRACKED_TIME_DEFAULT_VALUE,
} from '@/pages/constants';
import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { PageWrapper } from '@/pages/styles';
import { useClients } from '@/queries/useClients';
import { useCreateClient } from '@/queries/useCreateClient';
import { useUpdateClient } from '@/queries/useUpdateClient';

import {
	CHIEF_ROLES_CLIENT_GRID_WIDTH,
	ClientsGridRowType,
	HEAD_CLIENT_GRID_WIDTH,
} from './constants';
import { useClientsColumns } from './hooks/useClientsHeaders';
import { useClientsRows } from './hooks/useClientsValues';
import {
	clientsGridStyles,
	ClientHeaderWrapper,
	PageHeaderWrapper,
	ClientGridRowHoverWrapper,
} from './styles';
import { ClientsGridRow } from './types';

import 'react-data-grid/lib/styles.css';

const getRowKey = (row: ClientsGridRow) => row.id;

export function ClientsTab() {
	const [dataGridRows, setDataGridRows] = useState<ClientsGridRow[]>([]);
	const queryClient = useQueryClient();
	const toast = useToast();
	const { role } = useAuth();
	const { id, handleOpen, handleClose, isOpen } = useShowModal<number>();
	const {
		configurableValues: { countries },
	} = useConfiguration();

	const [clientNameFilter, setClientNameFilter] = useState('');
	const [registeredLocationFilter, setRegisteredLocationFilter] = useState<
		string[]
	>([]);
	const [notesFilter, setNotesFilter] = useState('');
	const [factTrackedTimeFilter, setFactTrackedTimeFilter] = useState<
		FromToProps<string>
	>(TRACKED_TIME_DEFAULT_VALUE);
	const [billableTrackedTimeFilter, setBillableTrackedTimeFilter] = useState<
		FromToProps<string>
	>(TRACKED_TIME_DEFAULT_VALUE);
	const [idFilter, setIdFilter] = useState('');
	const [statusFilter, setStatusFilter] = useState<string[]>(
		IS_ACTIVE_DEFAULT_VALUE
	);

	const {
		page,
		setPage,
		rowsPerPage,
		setRowsPerPage,
		order,
		orderBy,
		sortColumns,
		handleSortColumnChange,
	} = useTableState({
		resetPageDeps: [
			clientNameFilter,
			registeredLocationFilter,
			notesFilter,
			factTrackedTimeFilter,
			billableTrackedTimeFilter,
			idFilter,
			statusFilter,
		],
	});

	const displayTrackedColumn = useMemo(() => isChiefRole(role), [role]);

	const columnsWidths = useMemo(
		() =>
			displayTrackedColumn
				? CHIEF_ROLES_CLIENT_GRID_WIDTH
				: HEAD_CLIENT_GRID_WIDTH,
		[displayTrackedColumn]
	);

	const clientColumns = useClientsColumns({
		clientNameFilter,
		setClientNameFilter,
		registeredLocationFilter,
		setRegisteredLocationFilter,
		factTrackedTimeFilter,
		setFactTrackedTimeFilter,
		billableTrackedTimeFilter,
		setBillableTrackedTimeFilter,
		notesFilter,
		setNotesFilter,
		idFilter,
		setIdFilter,
		statusFilter,
		setStatusFilter,
		columnsWidths,
	});

	const { isLoading, isFetching, data, dataUpdatedAt, refetch } = useClients(
		page + 1,
		Number(rowsPerPage),
		order,
		orderBy,
		clientNameFilter,
		notesFilter,
		registeredLocationFilter,
		idFilter,
		statusFilter,
		countries,
		factTrackedTimeFilter,
		billableTrackedTimeFilter
	);

	const defaultValues = useMemo(
		() => ({
			name: '',
			legalEntityName: '',
			comments: '',
			registeredLocation: '',
			color: getRandomColor(),
			saleManager: null,
		}),
		[isOpen]
	);

	const getRowHeight = useMemo(
		() => (row: ClientsGridRow) =>
			row.projectItemsRowHeight ??
			(row.type === ClientsGridRowType.CLIENT
				? DEFAULT_GRID_ROW_HEIGHT
				: CLIENTS_PROJECT_GRID_ROW_HEIGHT),
		[]
	);

	const handleEdit = (clientId?: number) => () => {
		handleOpen(clientId);
	};

	const createClientMutation = useCreateClient({
		onSuccess: (res) => {
			if (res) {
				handleClose();
			}

			refetch();
		},
	});

	const clientRows = useClientsRows(
		data,
		dataUpdatedAt,
		page,
		rowsPerPage,
		isFetching,
		isLoading,
		handleEdit,
		role
	);

	const updateClientMutation = useUpdateClient(
		{
			onSuccess: (updatedClientItem) => {
				if (updatedClientItem) {
					handleClose();
				}

				queryClient.setQueryData(
					[
						ClientQuerieKeys.clients,
						page,
						rowsPerPage,
						order,
						orderBy,
						clientNameFilter,
						registeredLocationFilter,
						notesFilter,
						factTrackedTimeFilter.from,
						factTrackedTimeFilter.to,
					],
					(prev: unknown) => {
						updateGridItems(prev, updatedClientItem, orderBy, order);
						refetch();
					}
				);
			},
		},
		toast
	);

	useEffect(() => {
		setDataGridRows(clientRows);
	}, [clientRows]);

	const onRowsChange = (
		newRows: ClientsGridRow[],
		{ indexes }: RowsChangeData<ClientsGridRow>
	) => {
		const row = newRows[indexes[0]];

		if (!row.disabled) {
			if (row.expanded) {
				newRows.splice(indexes[0] + 1, 0, {
					disabled: false,
					id: -row.id,
					showCell: false,
					parentId: row.id,
					type: ClientsGridRowType.PROJECT,
					comments: '',
					name: '',
					registeredLocation: '',
					color: '',
					showSkeleton: false,
					role,
					isActive: undefined,
					factTrackedTimeInMinutes: 0,
					billableTrackedTimeInMinutes: 0,
				});
			} else {
				const rowNextFromChanged = newRows[indexes[0] + 1];

				if (rowNextFromChanged) {
					if (rowNextFromChanged.type === 'PROJECT') {
						newRows.splice(indexes[0] + 1, 1);
					}
				}
			}
		}
		setDataGridRows(newRows);
	};
	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);
			setPage(0);
		},
		[rowsPerPage]
	);

	const overrideClientGridStyles = useMemo(
		() =>
			clientsGridStyles(
				!!dataGridRows.length,
				displayTrackedColumn,
				getGridColumnTemplateFromColumnWidths(columnsWidths, true)
			),
		[dataGridRows.length]
	);

	const clientsGridPagination = useMemo(
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

	const handleSubmit = useCallback(
		async (values: ClientFormSubmitValues) => {
			if (id) {
				await updateClientMutation.mutateAsync({
					...values,
					id,
				});

				return;
			}

			await createClientMutation.mutateAsync(values);
		},
		[id, updateClientMutation, createClientMutation]
	);

	return (
		<PageWrapper>
			<ClientHeaderWrapper>
				<PageHeaderWrapper>
					<PageHeader
						addButton={{
							onClick: handleOpen,
							isHidden: isHeadOrGroupManager(role),
						}}
					/>
				</PageHeaderWrapper>
			</ClientHeaderWrapper>
			<Loader isLoading={isLoading || isFetching}>
				<ClientGridRowHoverWrapper>
					<DataGrid
						{...COMMON_GRID_PROPS}
						rowHeight={getRowHeight}
						columns={clientColumns}
						rows={dataGridRows}
						onRowsChange={onRowsChange}
						rowKeyGetter={getRowKey}
						scrollToTopOnRowsChange={false}
						enableVirtualization={false}
						sortColumns={sortColumns}
						onSortColumnsChange={handleSortColumnChange}
						overrideDataGridStyles={overrideClientGridStyles}
						paginationProps={clientsGridPagination}
						hideVerticalScrollbar={
							dataGridRows.length < DEFAULT_GRID_FIT_SCREEN_ROWS_AMOUNT &&
							(isLoading || isFetching)
						}
						renderers={noRowsGridFallback}
					/>
				</ClientGridRowHoverWrapper>
			</Loader>

			{isOpen && (
				<ClientForm
					initialValues={defaultValues}
					onSubmit={handleSubmit}
					{...{ handleClose, id, refetch }}
				/>
			)}
		</PageWrapper>
	);
}
