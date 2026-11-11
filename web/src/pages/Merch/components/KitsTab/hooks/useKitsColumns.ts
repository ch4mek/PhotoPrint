import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { ColoredPickerRenderCell } from '../../StorageTab/renderers/ColoredPickerRenderCell/ColoredPickerRenderCell';
import {
	DEFAULT_MERCH_KITS_GRID_FILTERS,
	KITS_TAB_COLUMNS_WIDTH,
	KITS_TAB_GRID_COLUMNS,
} from '../constants';
import { DefaultCellRenderer } from '../renderers/DefaultCellRender/defaultCellRender';
import { ItemCellWithPhoto } from '../renderers/ItemCellWithPhoto/ItemCellWithPhoto';
import { ItemStorageCell } from '../renderers/ItemStorageCell/ItemStorageCell';
import { PossibleKitsCell } from '../renderers/PossibleKitCell/PossibleKitCell';
import { SizeCell } from '../renderers/sizeCell/sizeCell';
import { StatusCell } from '../renderers/StatusCellRender/StatusCellRender';
import { KitsTabColumnsProps, MerchKitsGridRow } from '../types';

import {
	FreeTextModal,
	FromToProps,
	getEditColumnFactory,
	headerCellRenderer,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { EDIT_COLUMN_SUMMARY_CLASS } from '@/components/Grid/constants';
import { FromToNumberModal } from '@/components/Grid/modals/fromToNumberModalRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { fromToCurrencyObject } from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export function useKitsColumns({
	filters,
	setFilters,
	allMerchStorages,
	allMerchStatuses,
	allMerchStorageNames: allMerchElementsNames,
}: KitsTabColumnsProps) {
	const {
		descriptionFilter,
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		compoundFilter,
		possibleKitsFilter,
	} = filters;

	const isResetFiltersDisabled =
		JSON.stringify(filters) === JSON.stringify(DEFAULT_MERCH_KITS_GRID_FILTERS);

	const handleClear = useCallback(() => {
		setFilters.clear();
	}, [setFilters]);

	return useMemo(
		() =>
			[
				{
					...KITS_TAB_GRID_COLUMNS.id,
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.id).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: idFilter,
							setFilter: setFilters?.id as Dispatch<SetStateAction<string>>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: idFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
				},
				{
					...KITS_TAB_GRID_COLUMNS.name,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: nameFilter,
							setFilter: setFilters?.name as Dispatch<SetStateAction<string>>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: nameFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.name).width,
					renderCell: ItemCellWithPhoto,
				},
				{
					...KITS_TAB_GRID_COLUMNS.compound,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: compoundFilter,
							setFilter: setFilters?.compound as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: compoundFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.compound).width,
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
				},
				{
					...KITS_TAB_GRID_COLUMNS.storage,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: storageFilter,
							setFilter: setFilters?.storage as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: [],
							renderModal: SelectionModal,
							options: allMerchStorages,
							isFilterApplied: !!storageFilter.length,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.storage).width,
					renderCell: ItemStorageCell,
				},
				{
					...KITS_TAB_GRID_COLUMNS.size,
					renderHeaderCell: (props) =>
						headerCellRenderer({
							...props,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.size).width,
					renderCell: SizeCell,
				},
				{
					...KITS_TAB_GRID_COLUMNS.possibleKits,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							renderModal: FromToNumberModal,
							isFilterApplied: !areFromToCompatibleObjectsEqual(
								possibleKitsFilter,
								fromToCurrencyObject
							),
							filter: possibleKitsFilter || '',
							defaultValue: DEFAULT_MERCH_KITS_GRID_FILTERS.possibleKitsFilter,
							areValuesEquals: areFromToCompatibleObjectsEqual,
							setFilter: setFilters?.possibleKits as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							disableFilterIconActiveStateAsDefault: true,
							...props,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.possibleKits).width,
					renderCell: PossibleKitsCell,
				},
				{
					...KITS_TAB_GRID_COLUMNS.status,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: statusFilter,
							setFilter: setFilters?.status as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: DEFAULT_MERCH_KITS_GRID_FILTERS.statusFilter,
							options: allMerchStatuses,
							renderModal: SelectionModal,
							isFilterApplied: !!statusFilter.length,
							areValuesEquals: areArraysEqualByPrimitiveItems,
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.status).width,
					renderCell: StatusCell,
				},
				{
					...KITS_TAB_GRID_COLUMNS.description,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: descriptionFilter,
							setFilter: setFilters?.description as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: descriptionFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(KITS_TAB_COLUMNS_WIDTH.description).width,
				},
				getEditColumnFactory({
					key: '',
					name: '',
					sortable: false,
					frozen: true,
					summaryCellClass: EDIT_COLUMN_SUMMARY_CLASS,
					disabled: isResetFiltersDisabled,
					clearFilter: handleClear,
					renderCell: ColoredPickerRenderCell,
				}),
			] as Array<Column<MerchKitsGridRow>>,
		[filters, allMerchElementsNames, allMerchStatuses, allMerchStorages]
	);
}
