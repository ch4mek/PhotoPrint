import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { createAmountWithCurrencyCellRenderer } from '../../renderers/AmountWithCurrencyCellRenderer';
import { ProjectCellRenderer } from '../../renderers/ProjectCellRenderer';
import { RateHistoryCellRenderer } from '../components/renderers/RateHistoryCellRenderer';
import { getRatesGridColumnsWidths, ratesGridColumns } from '../constants';
import { RatesGridRow, UseRatesColumnsProps } from '../types';

import {
	ExpandCellRenderer,
	FreeTextAutoComplete,
	FreeTextModal,
	SelectionModal,
	cellWithTooltipRenderer,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import {
	initialProfitAndLossFilters,
	profitAndLossRateAmountCellFields,
	RATES_RATE_COLUMN_HEADER_CLASS,
} from '@/pages/ProfitAndLoss/constants';
import { getChildGroupCellClass } from '@/pages/ProfitAndLoss/utils/getChildGroupCellClass';

export const useRatesColumns = ({
	filters,
	setFilters,
	isResetFiltersDisabled,
	filterOptions,
	isDesktopWidth,
	onCellClick,
}: UseRatesColumnsProps): Column<RatesGridRow>[] => {
	const ratesGridColumnsWidths = useMemo(
		() => getRatesGridColumnsWidths(isDesktopWidth),
		[isDesktopWidth]
	);

	const columns: Column<RatesGridRow>[] = useMemo(
		() => [
			getColumnFactory<RatesGridRow>({
				...ratesGridColumns.expand,
				width: getColumnWidth(ratesGridColumnsWidths.expand).width,
				renderCell: (props) =>
					ExpandCellRenderer<RatesGridRow>(props, {
						shouldRender: (row) => row.isParent,
						getIsDisabled: (_row, hasChildren) => !hasChildren,
					}),
				cellClass: getChildGroupCellClass,
			}),

			getColumnWithTooltipFactory<RatesGridRow>({
				...ratesGridColumns.id,
				width: getColumnWidth(ratesGridColumnsWidths.id).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string>({
						...props,
						filter: filters.id,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.id !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialProfitAndLossFilters.id,
					}),
				renderCell: (props) => {
					if (!props.row.isParent) {
						return null;
					}

					return cellWithTooltipRenderer(props, { isCopyEnabled: true });
				},
			}),

			getColumnWithTooltipFactory<RatesGridRow>({
				...ratesGridColumns.user,
				width: getColumnWidth(ratesGridColumnsWidths.user).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string>({
						...props,
						filter: filters.user,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.user !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextAutoComplete,
						defaultValue: initialProfitAndLossFilters.user,
						autocompleteOptions: filterOptions.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) => {
					if (!props.row.isParent) {
						return null;
					}

					return cellWithTooltipRenderer(props, {
						onCellClick,
						isClickable: !!props.row.canViewPersonalInfo,
					});
				},
			}),

			getColumnWithTooltipFactory<RatesGridRow>({
				...ratesGridColumns.department,
				width: getColumnWidth(ratesGridColumnsWidths.department).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string[]>({
						...props,
						filter: filters.department,
						setFilter: setFilters.department as Dispatch<
							SetStateAction<string[]>
						>,
						isFilterApplied: filters.department.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						renderModal: SelectionModal,
						options: filterOptions.departments,
						defaultValue: initialProfitAndLossFilters.department,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) => {
					if (!props.row.isParent) {
						return null;
					}

					return cellWithTooltipRenderer(props);
				},
			}),

			getColumnFactory<RatesGridRow>({
				...ratesGridColumns.project,
				width: getColumnWidth(ratesGridColumnsWidths.project).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string>({
						...props,
						filter: filters.project,
						setFilter: setFilters.project as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.project !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialProfitAndLossFilters.project,
					}),
				renderCell: ProjectCellRenderer,
			}),

			getColumnWithTooltipFactory<RatesGridRow>({
				...ratesGridColumns.client,
				width: getColumnWidth(ratesGridColumnsWidths.client).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string>({
						...props,
						filter: filters.client,
						setFilter: setFilters.client as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.client !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialProfitAndLossFilters.client,
					}),
			}),

			getColumnWithTooltipFactory<RatesGridRow>({
				...ratesGridColumns.legalEntity,
				width: getColumnWidth(ratesGridColumnsWidths.legalEntity).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<RatesGridRow, string>({
						...props,
						filter: filters.legalEntity,
						setFilter: setFilters.legalEntity as Dispatch<
							SetStateAction<string>
						>,
						isFilterApplied: filters.legalEntity !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialProfitAndLossFilters.legalEntity,
					}),
			}),

			getColumnFactory<RatesGridRow>({
				...ratesGridColumns.rate,
				width: getColumnWidth(ratesGridColumnsWidths.rate).width,
				renderCell: createAmountWithCurrencyCellRenderer(
					profitAndLossRateAmountCellFields
				),
				headerCellClass: RATES_RATE_COLUMN_HEADER_CLASS,
			}),

			getEditColumnFactory<RatesGridRow>({
				key: '',
				name: '',
				disabled: isResetFiltersDisabled,
				renderCell: RateHistoryCellRenderer,
				clearFilter: setFilters.reset,
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: (row) => getChildGroupCellClass(row, EDIT_COLUMN_CELL_CLASS),
			}),
		],
		[
			filters,
			isResetFiltersDisabled,
			filterOptions,
			onCellClick,
			ratesGridColumnsWidths,
		]
	) as Column<RatesGridRow>[];

	return columns;
};
