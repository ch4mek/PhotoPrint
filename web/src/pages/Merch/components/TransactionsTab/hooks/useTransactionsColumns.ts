import { Dispatch, SetStateAction, useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS,
	TRANSACTIONS_TAB_COLUMNS_WIDTH,
	TRANSACTIONS_TAB_GRID_COLUMNS,
} from '../constants';
import { AmountCell } from '../renderers/AmountCell/AmountCell';
import { DefaultCellRenderer } from '../renderers/DefaultCellRender/DefaultCellRender';
import { IdRenderCell } from '../renderers/IdRenderCell/IdRenderCell';
import { ItemCellWithPhoto } from '../renderers/ItemCellWithPhoto/ItemCellWithPhoto';
import { SizeCell } from '../renderers/SizeCell/SizeCell';
import {
	MerchTransactionsGridRow,
	TransactionsTabColumnsProps,
} from '../types';

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
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { FromToNumberModal } from '@/components/Grid/modals/fromToNumberModalRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export function useTransactionsColumns({
	filters,
	setFilters,
	allMerchTransactionsNames,
	allMerchTransactionsCreators,
	allMerchSenders,
	allMerchStorages,
	allMerchTransactionTypes,
	allRecipients,
}: TransactionsTabColumnsProps) {
	const {
		amountFilter,
		creatorFilter,
		dateFilter,
		notesFilter,
		recipientFilter,
		senderFilter,
		transactionTypeFilter,
		idFilter,
		nameFilter,
		storageFilter,
	} = filters;

	const isResetFiltersDisabled = (
		Object.keys(filters) as Array<keyof typeof filters>
	).every((key) => {
		if (Array.isArray(filters[key])) {
			return areArraysEqualByPrimitiveItems(
				filters[key] as string[],
				DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS[key] as string[]
			);
		}

		return arePrimitiveValuesEqual(
			filters[key] as string,
			DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS[key] as string
		);
	});

	const handleClear = useCallback(() => {
		setFilters.clear();
	}, [setFilters]);

	return useMemo(
		() =>
			[
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.id,
					renderCell: IdRenderCell,
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.id).width,
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
					...TRANSACTIONS_TAB_GRID_COLUMNS.date,
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.date).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: dateFilter,
							setFilter: setFilters?.date as Dispatch<
								SetStateAction<FromToProps<Date>>
							>,
							defaultValue: DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS.dateFilter,
							renderModal: FromToDateModal,
							isFilterApplied: !areFromToCompatibleObjectsEqual(
								dateFilter,
								fromToDateObject
							),
							areValuesEquals: areFromToCompatibleObjectsEqual,
						}),
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.transactionType,
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.transactionType)
						.width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							options: allMerchTransactionTypes,
							filter: transactionTypeFilter,
							setFilter: setFilters?.transactionType as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue:
								DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS.transactionTypeFilter,
							renderModal: SelectionModal,
							isFilterApplied: !!transactionTypeFilter.length,
							areValuesEquals: areArraysEqualByPrimitiveItems,
						}),
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.name,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: nameFilter,
							setFilter: setFilters?.name as Dispatch<SetStateAction<string>>,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: allMerchTransactionsNames,
							isFilterApplied: nameFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.name).width,
					renderCell: (props) => ItemCellWithPhoto(props),
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.size,
					renderHeaderCell: (props) =>
						headerCellRenderer({
							...props,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.size).width,
					renderCell: SizeCell,
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.amount,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							renderModal: FromToNumberModal,
							isFilterApplied: !areFromToCompatibleObjectsEqual(
								amountFilter,
								fromToCurrencyObject
							),
							filter: amountFilter || '',
							defaultValue:
								DEFAULT_MERCH_TRANSACTIONS_GRID_FILTERS.amountFilter,
							areValuesEquals: areFromToCompatibleObjectsEqual,
							setFilter: setFilters?.amount as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							disableFilterIconActiveStateAsDefault: true,
							...props,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.amount).width,
					renderCell: AmountCell,
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.storage,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: storageFilter,
							setFilter: setFilters?.storage as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: [],
							options: allMerchStorages,
							renderModal: SelectionModal,
							isFilterApplied: storageFilter.length > 0,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.storage).width,
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
					...TRANSACTIONS_TAB_GRID_COLUMNS.sender,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: senderFilter,
							setFilter: setFilters?.sender as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: [],
							options: allMerchSenders,
							renderModal: SelectionModal,
							isFilterApplied: senderFilter.length > 0,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.sender).width,
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
					...TRANSACTIONS_TAB_GRID_COLUMNS.creator,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: creatorFilter,
							setFilter: setFilters?.creator as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							isFilterApplied: creatorFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							autocompleteOptions: allMerchTransactionsCreators,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.creator).width,
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
					...TRANSACTIONS_TAB_GRID_COLUMNS.recipient,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							options: allRecipients,
							filter: recipientFilter,
							setFilter: setFilters?.recipient as Dispatch<
								SetStateAction<string[]>
							>,
							defaultValue: [],
							renderModal: SelectionModal,
							isFilterApplied: !!recipientFilter.length,
							areValuesEquals: areArraysEqualByPrimitiveItems,
						}),
					renderCell: (props) =>
						DefaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.recipient).width,
				},
				{
					...TRANSACTIONS_TAB_GRID_COLUMNS.notes,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: notesFilter,
							setFilter: setFilters?.notes as Dispatch<SetStateAction<string>>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: notesFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					width: getColumnWidth(TRANSACTIONS_TAB_COLUMNS_WIDTH.notes).width,
					renderCell: (props) =>
						DefaultCellRenderer({
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
					clearFilter: handleClear,
				}),
			] as Array<Column<MerchTransactionsGridRow>>,
		[
			filters,
			allMerchTransactionsNames,
			allMerchTransactionsCreators,
			allMerchSenders,
			allMerchStorages,
			allRecipients,
		]
	);
}
