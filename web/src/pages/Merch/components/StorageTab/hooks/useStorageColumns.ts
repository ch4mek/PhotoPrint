import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_MERCH_STORAGE_GRID_FILTERS,
	STORAGE_TAB_COLUMNS_WIDTH,
	STORAGE_TAB_GRID_COLUMNS,
} from '../constants';
import { ColoredTransactionRenderCell } from '../renderers/ColoredTransactionRenderCell/ColoredTransactionRenderCell';
import { defaultCellRenderer } from '../renderers/DefaultRenderCell/defaultRenderCell';
import { ItemCellWithPhoto } from '../renderers/ItemCellWithPhoto/ItemCellWithPhoto';
import { ItemStorageCell } from '../renderers/ItemStorageCell/ItemStorageCell';
import { MinAmountCell } from '../renderers/MinAmountCell/MinAmountCell';
import { SizeCell } from '../renderers/SizeCell/SizeCell';
import { StatusCell } from '../renderers/StatusCellRender/StatusCellRender';
import { MerchStorageGridRow, StorageTabColumnsProps } from '../types';

import {
	FreeTextAutoComplete,
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

export function useStorageColumns({
	filters,
	setFilters,
	allMerchStorages,
	allMerchStatuses,
	allMerchStorageNames: allMerchElementsNames,
}: StorageTabColumnsProps) {
	const {
		descriptionFilter,
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		supplierFilter,
		totalAmountFilter,
	} = filters;

	const isResetFiltersDisabled = (
		Object.keys(filters) as Array<keyof typeof filters>
	).every((key) => {
		if (Array.isArray(filters[key])) {
			return areArraysEqualByPrimitiveItems(
				filters[key] as string[],
				DEFAULT_MERCH_STORAGE_GRID_FILTERS[key] as string[]
			);
		}

		return arePrimitiveValuesEqual(
			filters[key] as string,
			DEFAULT_MERCH_STORAGE_GRID_FILTERS[key] as string
		);
	});

	const handleClear = useCallback(() => {
		setFilters.clear();
	}, [setFilters]);

	return useMemo(
		() =>
			[
				{
					...STORAGE_TAB_GRID_COLUMNS.id,
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.id).width,
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
					...STORAGE_TAB_GRID_COLUMNS.name,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: nameFilter,
							setFilter: setFilters?.name as Dispatch<SetStateAction<string>>,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: allMerchElementsNames,
							isFilterApplied: nameFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.name).width,
					renderCell: (props) => ItemCellWithPhoto(props),
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.storage,
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
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.storage).width,
					renderCell: ItemStorageCell,
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.size,
					renderHeaderCell: (props) =>
						headerCellRenderer({
							...props,
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.size).width,
					renderCell: SizeCell,
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.totalAmount,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							renderModal: FromToNumberModal,
							isFilterApplied: !areFromToCompatibleObjectsEqual(
								totalAmountFilter,
								fromToCurrencyObject
							),
							filter: totalAmountFilter || '',
							defaultValue:
								DEFAULT_MERCH_STORAGE_GRID_FILTERS.totalAmountFilter,
							areValuesEquals: areFromToCompatibleObjectsEqual,
							setFilter: setFilters?.totalAmount as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							disableFilterIconActiveStateAsDefault: true,
							...props,
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.totalAmount).width,
					renderCell: MinAmountCell,
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.status,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: statusFilter,
							setFilter: setFilters?.status as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: DEFAULT_MERCH_STORAGE_GRID_FILTERS.statusFilter,
							options: allMerchStatuses,
							renderModal: SelectionModal,
							isFilterApplied: statusFilter.length > 0,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.status).width,
					renderCell: StatusCell,
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.supplier,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: supplierFilter,
							setFilter: setFilters?.supplier as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: supplierFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.supplier).width,
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
				},
				{
					...STORAGE_TAB_GRID_COLUMNS.description,
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
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(STORAGE_TAB_COLUMNS_WIDTH.description).width,
				},
				getEditColumnFactory({
					key: '',
					name: '',
					sortable: false,
					frozen: true,
					summaryCellClass: EDIT_COLUMN_SUMMARY_CLASS,
					disabled: isResetFiltersDisabled,
					clearFilter: handleClear,
					renderCell: ColoredTransactionRenderCell,
				}),
			] as Array<Column<MerchStorageGridRow>>,
		[filters, allMerchElementsNames, allMerchStatuses, allMerchStorages]
	);
}
