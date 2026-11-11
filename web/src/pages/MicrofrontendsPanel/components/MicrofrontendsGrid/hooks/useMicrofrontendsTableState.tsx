import { useEffect, useMemo, useState } from 'react';
import { Column } from 'react-data-grid';

import {
	MICROFRONTENDS_TAB_GRID_COLUMNS,
	MICROFRONTENDS_TABLE_COLUMNS_WIDTH,
} from '../constants';
import { ActionsCell } from '../renderers/ActionsCell/ActionsCell';
import { ChipsCell } from '../renderers/ChipsCell/ChipsCell';
import { ClaimsCell } from '../renderers/ClaimsCell/ClaimsCell';
import { DefaultTextCell } from '../renderers/DefaultTextCell/DefaultTextCell';
import { MicrofrontendsGridRow } from '../types';
import { mapMicrofrontendsResponseToRows } from '../utils/mapMicrofrontendsResponseToRows';

import { useShowModal } from '@/pages/hooks/useShowModal';
import { useTableState } from '@/pages/hooks/useTableState';
import { useGetMicrofrontends } from '@/queries/microfrontends/useGetMicrofrontends';

export const useMicrofrontendsTableState = () => {
	const [rows, setRows] = useState<MicrofrontendsGridRow[]>([]);
	const { sortColumns, handleSortColumnChange } = useTableState();

	const {
		id: editId,
		handleOpen: handleEditOpen,
		handleClose: handleEditClose,
		isOpen: isEditOpen,
	} = useShowModal();

	const {
		id: deleteId,
		handleOpen: handleDeleteOpen,
		handleClose: handleDeleteClose,
		isOpen: isDeleteOpen,
	} = useShowModal();

	const handleEditClick = (itemId?: string | number) => () => {
		handleEditOpen(itemId);
	};

	const handleDeleteClick = (itemId?: string | number) => () => {
		handleDeleteOpen(itemId);
	};

	const { data, isFetching } = useGetMicrofrontends({
		onSuccess: () => {
			if (data) {
				setRows(
					mapMicrofrontendsResponseToRows(
						data,
						handleEditClick,
						handleDeleteClick
					)
				);
			}
		},
	});

	const columns = useMemo(
		() =>
			[
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.name,
					renderCell: (props) => (
						<DefaultTextCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.name.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.name.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.scope,
					renderCell: (props) => (
						<DefaultTextCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.scope.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.scope.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.modules,
					renderCell: (props) => (
						<ChipsCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.modules.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.modules.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.remoteEntries,
					renderCell: (props) => (
						<DefaultTextCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.remoteEntries.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.remoteEntries.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.routes,
					renderCell: (props) => (
						<ChipsCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.routes.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.routes.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.claims,
					renderCell: ClaimsCell,
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.claims.width,
				},
				{
					...MICROFRONTENDS_TAB_GRID_COLUMNS.version,
					renderCell: (props) => (
						<DefaultTextCell
							{...props}
							field={MICROFRONTENDS_TAB_GRID_COLUMNS.version.key}
						/>
					),
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.version.width,
				},
				{
					key: MICROFRONTENDS_TAB_GRID_COLUMNS.actions.key,
					name: MICROFRONTENDS_TAB_GRID_COLUMNS.actions.name,
					sortable: false,
					width: MICROFRONTENDS_TABLE_COLUMNS_WIDTH.actions.width,
					renderCell: ActionsCell,
				},
			] as Array<Column<MicrofrontendsGridRow>>,
		[]
	);

	const getRowHeight = useMemo(
		() => (row: MicrofrontendsGridRow) => row.itemRowHeight,
		[]
	);

	useEffect(() => {
		if (data) {
			setRows(
				mapMicrofrontendsResponseToRows(
					data,
					handleEditClick,
					handleDeleteClick
				)
			);
		}
	}, [data]);

	return {
		sortColumns,
		isFetching,
		columns,
		rows,
		setRows,
		handleSortColumnChange,
		getRowHeight,
		editId,
		handleEditOpen,
		handleEditClose,
		isEditOpen,
		deleteId,
		handleDeleteOpen,
		handleDeleteClose,
		isDeleteOpen,
	};
};
