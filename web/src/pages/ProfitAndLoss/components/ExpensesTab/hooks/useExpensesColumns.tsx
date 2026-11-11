import { EditIcon, BasketIcon } from '@mms/mms-ui-library/icons/base';
import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import { HeaderCellWithBYNSymbolRenderer } from '../../renderers/HeaderCellWithBYNSymbolRenderer';
import { MonthCellRenderer } from '../components/renderers/MonthCellRenderer';
import { TotalCellRenderer } from '../components/renderers/TotalCellRenderer';
import {
	EXPENSE_CELL_CLASS,
	expensesGridColumns,
	expensesGridColumnsWidths,
	expensesMenuOptionsValues,
} from '../constants';
import {
	ExpensesGridRow,
	ExpensesSummaryGridRow,
	UseExpensesColumnsProps,
} from '../types';

import {
	FreeTextAutoComplete,
	cellWithTooltipRenderer,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_HEADER_CLASS,
	EDIT_COLUMN_SUMMARY_CLASS,
	EDIT_COLUMN_CELL_CLASS,
} from '@/components/Grid/constants';
import { menuOptionsCellRenderer } from '@/components/Grid/renderers/cell/menuOptionsCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { months } from '@/constants/date';
import { arePrimitiveValuesEqual } from '@/helpers/areValuesEqual';

export const useExpensesColumns = ({
	filters,
	setFilters,
	isResetFiltersDisabled,
	monthColumnWidth,
	handleEdit,
	handleDelete,
	filterOptions,
}: UseExpensesColumnsProps): Column<
	ExpensesGridRow,
	ExpensesSummaryGridRow
>[] => {
	const monthColumns: Column<ExpensesGridRow, ExpensesSummaryGridRow>[] =
		useMemo(
			() =>
				months.map((month) => ({
					key: month,
					name: month,
					renderHeaderCell: HeaderCellWithBYNSymbolRenderer,
					renderCell: (props) =>
						MonthCellRenderer({ ...props, width: monthColumnWidth }),
					renderSummaryCell: (props) => TotalCellRenderer(props),
				})),
			[monthColumnWidth]
		);

	const columns: Column<ExpensesGridRow, ExpensesSummaryGridRow>[] = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...expensesGridColumns.expense,
				width: getColumnWidth(expensesGridColumnsWidths.expense).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<ExpensesGridRow, string>({
						...props,
						filter: filters.expense,
						setFilter: setFilters.expense as Dispatch<SetStateAction<string>>,
						isFilterApplied: filters.expense !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						renderModal: FreeTextAutoComplete,
						defaultValue: '',
						autocompleteOptions: filterOptions.expenses,
					}),
				renderCell: cellWithTooltipRenderer,
				cellClass: EXPENSE_CELL_CLASS,
			}) as Column<ExpensesGridRow, ExpensesSummaryGridRow>,
			...monthColumns,
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isResetFiltersDisabled,
				renderCell: (props) =>
					menuOptionsCellRenderer(props, {
						options: (row) => [
							{
								value: expensesMenuOptionsValues.edit,
								addon: <EditIcon />,
								onClick: () => handleEdit(row),
							},
							{
								value: expensesMenuOptionsValues.delete,
								addon: <BasketIcon />,
								onClick: () => handleDelete(row),
							},
						],
					}),
				clearFilter: setFilters.reset,
				frozen: true,
				sortable: false,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
				summaryCellClass: EDIT_COLUMN_SUMMARY_CLASS,
			}) as Column<ExpensesGridRow, ExpensesSummaryGridRow>,
		],
		[
			filters,
			setFilters,
			isResetFiltersDisabled,
			handleEdit,
			handleDelete,
			monthColumns,
			filterOptions,
		]
	);

	return columns;
};
