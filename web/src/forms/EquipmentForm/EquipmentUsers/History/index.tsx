import { DataGrid } from '@mms/mms-ui-library/components/Grids';
import { PopUp } from '@mms/mms-ui-library/components/PopUp';
import { Tabs } from '@mms/mms-ui-library/components/Tabs';
import { useQueryClient } from '@tanstack/react-query';
import { useCallback, useMemo, useState } from 'react';

import { HistoryTabs, HistoryTabsData } from '../../constants';

import { DataGridHoverRowWrapper } from '@/components/Grid/components';
import { noRowsGridFallback } from '@/constants/noRowsGridFallback';
import {
	ASSIGNMENTS_HISTORY_QUERY_KEY,
	EquipmentQueriesKeys,
} from '@/constants/queriesKeys';
import { SortDirection } from '@/constants/sort';
import { useToast } from '@/context/Toast';
import { createGridPagination } from '@/helpers/createGridPagination';
import { usePageWithSearch } from '@/hooks/usePageWithSearch';
import { useTableState } from '@/pages/hooks/useTableState';
import { useDeleteLastEquipmentHistoryItem } from '@/queries/equipment/useDeleteLastEquipmentHistoryItem';
import { useGetEquipmentHistory } from '@/queries/equipment/useGetEquipmentHistory';

import {
	APPROVE_MESSAGE,
	HISTORY_UPDATING_POPUP_TITLE,
	ROW_HEIGHT,
} from './constants';
import { useGetEquipmentHistoryColumns } from './hooks/useGetEquipmentHistoryColumns';
import { useGetEquipmentHistoryRows } from './hooks/useGetEquipmentHistoryRows';
import { HistoryPositionContainer, TabsHeaderContainer } from './styles';
import type { EquipmentHistoryProps } from './types';
import { overrideEquipmentGridStyles } from './utils/overrideEquipmentGridStyles';

export function EquipmentHistory({
	equipmentId,
	setHistoryItems,
	resetForm,
	status,
}: EquipmentHistoryProps) {
	const [activeTab, setActiveTab] = useState<HistoryTabs>(HistoryTabs.HISTORY);
	const [isConfirmPopupOpen, setIsConfirmPopupOpen] = useState(false);
	const [equipmentHistoryIdToDelete, setEquipmentHistoryIdToDelete] = useState<
		number | null
	>(null);

	const queryClient = useQueryClient();
	const toast = useToast();

	const { mutate: removeHistoryRowMutation } =
		useDeleteLastEquipmentHistoryItem(toast);

	const { page, setPage } = usePageWithSearch();
	const { page: deletedHistoryPage, setPage: setDeletedHistoryPage } =
		usePageWithSearch();
	const { rowsPerPage, setRowsPerPage } = useTableState();

	const isHistoryTabActive = activeTab === HistoryTabs.HISTORY;
	const isDeletedHistoryTabActive = activeTab === HistoryTabs.DELETED;

	const {
		data: historyData,
		isLoading: isHistoryDataLoading,
		isFetching: isHistoryDataFetching,
	} = useGetEquipmentHistory({
		toast,
		id: equipmentId,
		page: page + 1,
		rowsPerPage,
		isDeletedFilter: false,
		orderBy: SortDirection.DESC,
		enabled: isHistoryTabActive,
	});
	const {
		data: deletedHistoryData,
		isLoading: isDeletedHistoryDataLoading,
		isFetching: isDeletedHistoryDataFetching,
	} = useGetEquipmentHistory({
		toast,
		id: equipmentId,
		page: deletedHistoryPage + 1,
		rowsPerPage,
		isDeletedFilter: true,
		orderBy: SortDirection.DESC,
		enabled: isDeletedHistoryTabActive,
	});

	const showHistoryRowsSkeleton = isHistoryDataLoading || isHistoryDataFetching;
	const showDeletedHistoryRowsSkeleton =
		isDeletedHistoryDataLoading || isDeletedHistoryDataFetching;

	const handleDelete = useCallback((equipmentHistoryId: number) => {
		setEquipmentHistoryIdToDelete(equipmentHistoryId);
		setIsConfirmPopupOpen(true);
	}, []);

	const handleApply = useCallback(() => {
		if (!equipmentHistoryIdToDelete) {
			return;
		}

		setIsConfirmPopupOpen(false);
		removeHistoryRowMutation(
			{
				equipmentId,
				equipmentHistoryId: equipmentHistoryIdToDelete,
			},
			{
				onSuccess: () => {
					setHistoryItems((history) =>
						history.filter(
							(historyItem) =>
								historyItem.equipmentHistoryId !== equipmentHistoryIdToDelete
						)
					);
					setEquipmentHistoryIdToDelete(null);
					queryClient.refetchQueries({
						queryKey: [EquipmentQueriesKeys.history],
					});
					queryClient.invalidateQueries({
						queryKey: [EquipmentQueriesKeys.equipments],
					});
					queryClient.invalidateQueries({
						queryKey: [ASSIGNMENTS_HISTORY_QUERY_KEY],
					});
					resetForm();
				},
			}
		);
	}, [
		removeHistoryRowMutation,
		setHistoryItems,
		resetForm,
		equipmentHistoryIdToDelete,
	]);

	const toggleIsConfirmOpen = useCallback(
		() => setIsConfirmPopupOpen((prev) => !prev),
		[]
	);

	const handleChangeTab = useCallback(
		(newActiveTab: number) => setActiveTab(newActiveTab),
		[]
	);
	const { historyColumns } = useGetEquipmentHistoryColumns(
		handleDelete,
		activeTab,
		status
	);

	const historyRows = useGetEquipmentHistoryRows(
		historyData!,
		showHistoryRowsSkeleton
	);

	const deletedHistoryRows = useGetEquipmentHistoryRows(
		deletedHistoryData!,
		showDeletedHistoryRowsSkeleton
	);

	const handleOnPerPageChange = useCallback(
		(rowsPerPageCounter: number) => {
			setRowsPerPage(rowsPerPageCounter);

			if (isHistoryTabActive) {
				setPage(0);
			} else {
				setDeletedHistoryPage(0);
			}
		},
		[rowsPerPage, isHistoryTabActive]
	);

	const historyGridPagination = useMemo(
		() =>
			createGridPagination(
				historyRows.length,
				page,
				setPage,
				rowsPerPage,
				handleOnPerPageChange,
				historyData?.total
			),
		[historyRows.length, page, rowsPerPage, historyData?.total]
	);

	const deletedHistoryGridPagination = useMemo(
		() =>
			createGridPagination(
				deletedHistoryRows.length,
				deletedHistoryPage,
				setDeletedHistoryPage,
				rowsPerPage,
				handleOnPerPageChange,
				deletedHistoryData?.total
			),
		[
			deletedHistoryRows.length,
			deletedHistoryPage,
			rowsPerPage,
			deletedHistoryData?.total,
		]
	);

	const hasDataGridPagination = Boolean(
		isHistoryTabActive ? historyGridPagination : deletedHistoryGridPagination
	);
	const dataGridRows = useMemo(
		() => (isHistoryTabActive ? historyRows : deletedHistoryRows),
		[isHistoryTabActive, historyRows, deletedHistoryRows]
	);
	const dataGridPaginationProps = useMemo(
		() =>
			isHistoryTabActive ? historyGridPagination : deletedHistoryGridPagination,
		[isHistoryTabActive, historyGridPagination, deletedHistoryGridPagination]
	);

	return (
		<HistoryPositionContainer>
			<TabsHeaderContainer>
				<Tabs
					activeTab={activeTab}
					onChangeTab={handleChangeTab}
					data={HistoryTabsData}
				/>
			</TabsHeaderContainer>
			<DataGridHoverRowWrapper defineHeight={false}>
				<DataGrid
					rowHeight={ROW_HEIGHT}
					overrideDataGridStyles={overrideEquipmentGridStyles(
						hasDataGridPagination,
						isHistoryTabActive
					)}
					columns={historyColumns}
					rows={dataGridRows}
					renderers={noRowsGridFallback}
					paginationProps={dataGridPaginationProps}
					hideVerticalScrollbar={
						showDeletedHistoryRowsSkeleton || showHistoryRowsSkeleton
					}
				/>
			</DataGridHoverRowWrapper>

			{isConfirmPopupOpen && (
				<PopUp
					title={HISTORY_UPDATING_POPUP_TITLE}
					onClose={toggleIsConfirmOpen}
					type="approve"
					showCloseButton={false}
					controls={{
						negativeControl: { onClick: toggleIsConfirmOpen },
						positiveControl: { onClick: handleApply },
					}}
				>
					{APPROVE_MESSAGE}
				</PopUp>
			)}
		</HistoryPositionContainer>
	);
}
