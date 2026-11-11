import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_MERCH_ELEMENTS_GRID_FILTERS,
	ELEMENT_TAB_COLUMNS_WIDTH,
	ELEMENTS_TAB_GRID_COLUMNS,
} from '../constants';
import { defaultCellRenderer } from '../renderers/DefaultRenderCell/defaultRenderCell';
import { ItemCellWithPhoto } from '../renderers/ItemCellWithPhoto/ItemCellWithPhoto';
import { itemStorageCell } from '../renderers/ItemStorageCell/ItemStorageCell';
import { MinAmountCell } from '../renderers/MinAmountCell/MinAmountCell';
import { sizeCell } from '../renderers/SizeCell/SizeCell';
import { ElementsTabColumnsProps, MerchElementsGridRow } from '../types';

import {
	editCellRenderer,
	FreeTextAutoComplete,
	FreeTextModal,
	getEditColumnFactory,
	headerCellRenderer,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { EDIT_COLUMN_SUMMARY_CLASS } from '@/components/Grid/constants';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { MerchStatuses } from '@/pages/Merch/constants';
import { UserClaims } from '@/types/claims';

export function useElementsColumns({
	filters,
	setFilters,
	allMerchStorages,
	allMerchStatuses,
	allMerchElementsNames,
}: ElementsTabColumnsProps) {
	const { claims } = useAuth();
	const canManageItems = checkIsHasClaims(
		[UserClaims.ManageItemsAndKits],
		claims
	);

	const {
		descriptionFilter,
		idFilter,
		nameFilter,
		statusFilter,
		storageFilter,
		supplierFilter,
	} = filters;

	const isResetFiltersDisabled = (
		Object.keys(filters) as Array<keyof typeof filters>
	).every((key) => {
		if (Array.isArray(filters[key])) {
			return areArraysEqualByPrimitiveItems(
				filters[key] as string[],
				DEFAULT_MERCH_ELEMENTS_GRID_FILTERS[key] as string[]
			);
		}

		return arePrimitiveValuesEqual(
			filters[key] as string,
			DEFAULT_MERCH_ELEMENTS_GRID_FILTERS[key] as string
		);
	});

	const handleClear = useCallback(() => {
		setFilters.clear();
	}, [setFilters]);

	return useMemo(
		() =>
			[
				{
					...ELEMENTS_TAB_GRID_COLUMNS.id,
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.id).width,
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
					...ELEMENTS_TAB_GRID_COLUMNS.name,
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
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.name).width,
					renderCell: (props) => ItemCellWithPhoto(props),
				},
				{
					...ELEMENTS_TAB_GRID_COLUMNS.storage,
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
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.storage).width,
					renderCell: itemStorageCell,
				},
				{
					...ELEMENTS_TAB_GRID_COLUMNS.size,
					renderHeaderCell: (props) =>
						headerCellRenderer({
							...props,
						}),
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.size).width,
					renderCell: sizeCell,
				},
				{
					...ELEMENTS_TAB_GRID_COLUMNS.minAmount,
					renderHeaderCell: (props) =>
						headerCellRenderer({
							...props,
						}),
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.minAmount).width,
					renderCell: MinAmountCell,
				},
				{
					...ELEMENTS_TAB_GRID_COLUMNS.status,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: statusFilter,
							setFilter: setFilters?.status as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: [MerchStatuses.available],
							options: allMerchStatuses,
							renderModal: SelectionModal,
							isFilterApplied: statusFilter.length > 0,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.status).width,
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
					...ELEMENTS_TAB_GRID_COLUMNS.supplier,
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
					width: getColumnWidth(ELEMENT_TAB_COLUMNS_WIDTH.supplier).width,
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
					...ELEMENTS_TAB_GRID_COLUMNS.description,
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
				},
				getEditColumnFactory({
					key: '',
					name: '',
					sortable: false,
					frozen: true,
					summaryCellClass: EDIT_COLUMN_SUMMARY_CLASS,
					disabled: isResetFiltersDisabled,
					renderCell: (props) =>
						editCellRenderer(props, { isDisabled: !canManageItems }),
					clearFilter: handleClear,
				}),
			] as Array<Column<MerchElementsGridRow>>,
		[
			filters,
			allMerchElementsNames,
			allMerchStatuses,
			allMerchStorages,
			canManageItems,
		]
	);
}
