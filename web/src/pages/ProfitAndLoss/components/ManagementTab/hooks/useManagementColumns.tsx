import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { createAmountWithCurrencyCellRenderer } from '../../renderers/AmountWithCurrencyCellRenderer';
import { HeaderCellWithBYNSymbolRenderer } from '../../renderers/HeaderCellWithBYNSymbolRenderer';
import { ProjectCellRenderer } from '../../renderers/ProjectCellRenderer';
import { KpiCellRenderer } from '../components/renderers/KpiCellRenderer';
import { createPriceCellRenderer } from '../components/renderers/PriceCellRenderer';
import {
	createSummaryPriceCellRenderer,
	SummaryExpensesProfitCellRenderer,
	SummaryKpiCellRenderer,
} from '../components/renderers/summaryCellRenderers';
import {
	getManagementGridColumns,
	getManagementGridColumnsWidths,
	MANAGEMENT_SUMMARY_LEFT_CELL_CLASS,
	managementGridRowFields,
	managementIncomeAmountCellFields,
	managementRateAmountCellFields,
} from '../constants';
import { ManagementGridRow, UseManagementColumnsProps } from '../types';

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
	RATES_RATE_COLUMN_HEADER_CLASS,
	RIGHT_ALIGN_CELL_CLASS,
} from '@/pages/ProfitAndLoss/constants';
import { getChildGroupCellClass } from '@/pages/ProfitAndLoss/utils/getChildGroupCellClass';
import { getParentRateColumnCellClass } from '@/pages/ProfitAndLoss/utils/getParentRateColumnCellClass';

export const useManagementColumns = ({
	filters,
	setFilters,
	isResetFiltersDisabled,
	filterOptions,
	isDesktopWidth,
	onCellClick,
}: UseManagementColumnsProps): Column<ManagementGridRow>[] => {
	const managementGridColumns = useMemo(
		() => getManagementGridColumns(isDesktopWidth),
		[isDesktopWidth]
	);

	const managementGridColumnsWidths = useMemo(
		() => getManagementGridColumnsWidths(isDesktopWidth),
		[isDesktopWidth]
	);

	const columns: Column<ManagementGridRow>[] = useMemo(
		() => [
			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.expand,
				width: getColumnWidth(managementGridColumnsWidths.expand).width,
				renderCell: (props) =>
					ExpandCellRenderer<ManagementGridRow>(props, {
						shouldRender: (row) => row.isParent,
						getIsDisabled: (_row, hasChildren) => !hasChildren,
					}),
				renderSummaryCell: SummaryExpensesProfitCellRenderer,
				summaryCellClass: MANAGEMENT_SUMMARY_LEFT_CELL_CLASS,
				cellClass: getChildGroupCellClass,
			}),

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.id,
				width: getColumnWidth(managementGridColumnsWidths.id).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string>({
						...props,
						filter: filters.id,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.id !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: '',
					}),
				renderCell: (props) => {
					if (!props.row.isParent) {
						return null;
					}

					return cellWithTooltipRenderer(props, { isCopyEnabled: true });
				},
			}),

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.user,
				width: getColumnWidth(managementGridColumnsWidths.user).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string>({
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

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.department,
				width: getColumnWidth(managementGridColumnsWidths.department).width,
				cellClass: getChildGroupCellClass,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string[]>({
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

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.project,
				width: getColumnWidth(managementGridColumnsWidths.project).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string>({
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

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.client,
				width: getColumnWidth(managementGridColumnsWidths.client).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string>({
						...props,
						filter: filters.client,
						setFilter: setFilters.client as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.client !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextModal,
						defaultValue: initialProfitAndLossFilters.client,
					}),
			}),

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.legalEntity,
				width: getColumnWidth(managementGridColumnsWidths.legalEntity).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ManagementGridRow, string>({
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

			getColumnWithTooltipFactory<ManagementGridRow>({
				...managementGridColumns.monthHoursCount,
				width: getColumnWidth(managementGridColumnsWidths.monthHoursCount)
					.width,
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						fieldToDisplay: managementGridRowFields.monthHoursCount,
					}),
			}),

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.rate,
				width: getColumnWidth(managementGridColumnsWidths.rate).width,
				renderCell: createAmountWithCurrencyCellRenderer(
					managementRateAmountCellFields
				),
				headerCellClass: RATES_RATE_COLUMN_HEADER_CLASS,
				cellClass: getParentRateColumnCellClass,
			}),

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.income,
				width: getColumnWidth(managementGridColumnsWidths.income).width,
				renderCell: createAmountWithCurrencyCellRenderer(
					managementIncomeAmountCellFields
				),
				headerCellClass: RATES_RATE_COLUMN_HEADER_CLASS,
				cellClass: getParentRateColumnCellClass,
			}),

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.incomeBYN,
				width: getColumnWidth(managementGridColumnsWidths.incomeBYN).width,
				renderHeaderCell: HeaderCellWithBYNSymbolRenderer,
				renderCell: createPriceCellRenderer(managementGridRowFields.incomeBYN),
				renderSummaryCell: createSummaryPriceCellRenderer(
					managementGridRowFields.incomeBYN
				),
				cellClass: RIGHT_ALIGN_CELL_CLASS,
			}),

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.salary,
				width: getColumnWidth(managementGridColumnsWidths.salary).width,
				renderHeaderCell: HeaderCellWithBYNSymbolRenderer,
				renderCell: createPriceCellRenderer(managementGridRowFields.salary),
				renderSummaryCell: createSummaryPriceCellRenderer(
					managementGridRowFields.salary
				),
				cellClass: RIGHT_ALIGN_CELL_CLASS,
			}),

			getColumnFactory<ManagementGridRow>({
				...managementGridColumns.kpi,
				width: getColumnWidth(managementGridColumnsWidths.kpi).width,
				renderCell: KpiCellRenderer,
				renderSummaryCell: SummaryKpiCellRenderer,
			}),

			getEditColumnFactory<ManagementGridRow>({
				key: '',
				name: '',
				disabled: isResetFiltersDisabled,
				renderCell: () => null,
				renderSummaryCell: () => null,
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
			managementGridColumns,
			managementGridColumnsWidths,
			onCellClick,
		]
	) as Column<ManagementGridRow>[];

	return columns;
};
