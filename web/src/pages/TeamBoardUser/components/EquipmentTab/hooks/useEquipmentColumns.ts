import { SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	defaultUserEquipmentFilters,
	equipmentGridColumns,
	equipmentGridColumnWidths,
} from '../constants';
import {
	UserEquipmentFiltersHandlers,
	UserEquipmentFilters,
	UserEquipmentFiltersOptions,
} from '../types';
import { isResetDisabled } from '../utils';

import {
	cellRenderer,
	FreeTextModal,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { CellWithTooltipAlternativeRenderer } from '@/components/Grid/renderers/cell/cellWithTooltipAlternativeRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { UserEquipmentGridRow } from '@/types/equipment';
import { TooltipContent } from '@/types/hooks';

export function useEquipmentsColumns(
	filters: UserEquipmentFilters,
	setFilters: UserEquipmentFiltersHandlers,
	filtersOptions: UserEquipmentFiltersOptions,
	setTooltipContent: (value: SetStateAction<TooltipContent>) => void
) {
	const columns: Column<UserEquipmentGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...equipmentGridColumns.equipmentName,
				width: getColumnWidth(equipmentGridColumnWidths.equipmentName).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentName,
						setFilter: setFilters.equipmentName,
						renderModal: FreeTextModal,
						defaultValue: defaultUserEquipmentFilters.equipmentName,
						isFilterApplied: Boolean(filters.equipmentName),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.equipmentTypeName,
				width: getColumnWidth(equipmentGridColumnWidths.equipmentTypeName)
					.width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.equipmentType,
						setFilter: setFilters.equipmentType,
						renderModal: SelectionModal,
						options: filtersOptions.equipmentTypes,
						defaultValue: defaultUserEquipmentFilters.equipmentType,
						isFilterApplied: filters.equipmentType.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.serialNumber,
				width: getColumnWidth(equipmentGridColumnWidths.serialNumber).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.serialNumber,
						setFilter: setFilters.serialNumber,
						renderModal: FreeTextModal,
						defaultValue: defaultUserEquipmentFilters.serialNumber,
						isFilterApplied: Boolean(filters.serialNumber),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.inventoryNumber,
				width: getColumnWidth(equipmentGridColumnWidths.inventoryNumber).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.inventoryNumber,
						setFilter: setFilters.inventoryNumber,
						renderModal: FreeTextModal,
						defaultValue: defaultUserEquipmentFilters.inventoryNumber,
						isFilterApplied: Boolean(filters.inventoryNumber),
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.issueDate,
				width: getColumnWidth(equipmentGridColumnWidths.issueDate).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.issueDate,
						setFilter: setFilters.issueDate,
						defaultValue: defaultUserEquipmentFilters.issueDate,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.issueDate?.from || !!filters.issueDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.returnDate,
				width: getColumnWidth(equipmentGridColumnWidths.returnDate).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.returnDate,
						setFilter: setFilters.returnDate,
						defaultValue: defaultUserEquipmentFilters.returnDate,
						renderModal: FromToDateModal,
						isFilterApplied:
							!!filters.returnDate?.from || !!filters.returnDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: cellRenderer,
			}),
			getColumnWithTooltipFactory({
				...equipmentGridColumns.isAssigned,
				width: getColumnWidth(equipmentGridColumnWidths.isAssigned).width,
				sortable: true,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.isAssigned,
						isFilterApplied: filters.isAssigned.length > 0,
						setFilter: setFilters.isAssigned,
						renderModal: SelectionModal,
						options: filtersOptions.assignStatuses,
						defaultValue: defaultUserEquipmentFilters.isAssigned,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					CellWithTooltipAlternativeRenderer(props, setTooltipContent, true),
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetDisabled(filters),
				clearFilter: setFilters.reset,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				frozen: true,
			}),
		],
		[filters]
	);

	return columns;
}
