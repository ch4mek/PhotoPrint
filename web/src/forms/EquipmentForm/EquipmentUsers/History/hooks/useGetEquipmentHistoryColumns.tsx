import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import { COLUMNS_AMOUNT, HISTORY_COLUMNS } from '../constants';
import {
	nameCellRenderer,
	historyCellRenderer,
	deleteCellRenderer,
} from '../renderers';
import { EquipmentHistoryRow } from '../types';

import {
	headerCellRenderer,
	getEditColumnFactory,
	getColumnFactory,
} from '@/components/Grid';
import {
	EDIT_COLUMN_HEADER_CLASS,
	EDIT_COLUMN_CELL_CLASS,
} from '@/components/Grid/constants';
import { HistoryTabs } from '@/forms/EquipmentForm/constants';

export const useGetEquipmentHistoryColumns = (
	handleDelete: (equipmentHistoryId: number) => void,
	activeTab: HistoryTabs,
	status: string
) => {
	const historyColumns: Array<Column<EquipmentHistoryRow>> = useMemo(
		() => [
			getColumnFactory({
				...HISTORY_COLUMNS.userName,
				renderHeaderCell: headerCellRenderer,
				renderCell: (props) => nameCellRenderer(props, activeTab),
			}),
			getColumnFactory({
				...HISTORY_COLUMNS.departmentId,
				renderHeaderCell: headerCellRenderer,
				renderCell: (props) => nameCellRenderer(props, activeTab),
			}),
			getColumnFactory({
				...HISTORY_COLUMNS.issueDate,
				renderHeaderCell: headerCellRenderer,
				renderCell: (props) => historyCellRenderer(props, activeTab),
			}),
			getColumnFactory({
				...HISTORY_COLUMNS.returnDate,
				renderHeaderCell: headerCellRenderer,
				renderCell: (props) => historyCellRenderer(props, activeTab),
			}),
			getColumnFactory({
				...HISTORY_COLUMNS.assignmentApprover,
				renderHeaderCell: headerCellRenderer,
				renderCell: (props) => nameCellRenderer(props, activeTab),
			}),
		],
		[activeTab]
	);

	const deleteHistoryItemColumn: Column<EquipmentHistoryRow> = useMemo(
		() =>
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: true,
				displayFilterButton: false,
				renderCell: (props) =>
					deleteCellRenderer(props, activeTab, handleDelete, status),
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		[activeTab, handleDelete]
	);

	if (
		activeTab === HistoryTabs.HISTORY &&
		historyColumns.length === COLUMNS_AMOUNT
	) {
		historyColumns.push(deleteHistoryItemColumn);
	}

	return { historyColumns };
};
