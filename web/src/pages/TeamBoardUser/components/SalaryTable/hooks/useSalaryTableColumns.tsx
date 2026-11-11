import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	SALARY_COLUMN_KEYS,
	SALARY_COLUMN_NAMES,
	SALARY_TABLE_COLUMNS_WIDTHS,
} from '../constants';
import { SalaryFilterSetter, SalaryRow, SalaryTableFilters } from '../types';

import {
	FromToProps,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { FromToPriceModal } from '@/components/Grid/modals/fromToPriceModalRenderer';
import { SalaryCell } from '@/components/SalaryCell';
import {
	fromToCurrencyObject,
	fromToDateObject,
} from '@/constants/defaultValues';
import { areFromToCompatibleObjectsEqual } from '@/helpers/areValuesEqual';
import { TeamboardTableMode } from '@/types/teamboard';

export const useSalaryTableColumns = (
	mode: TeamboardTableMode,
	filters?: SalaryTableFilters,
	areFiltersApplied?: boolean,
	filterSetter?: SalaryFilterSetter,
	resetFilters?: () => void
): Array<Column<SalaryRow>> => {
	const isTabMode = mode === TeamboardTableMode.Tab;

	return useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: SALARY_COLUMN_KEYS.date,
				name: SALARY_COLUMN_NAMES.date,
				width: SALARY_TABLE_COLUMNS_WIDTHS.date,
				renderHeaderCell:
					isTabMode && filterSetter && filters
						? (props) =>
								headerCellWithFilterRenderer<SalaryRow, FromToProps<Date>>({
									...props,
									filter: filters?.date || fromToDateObject,
									setFilter: filterSetter?.date as React.Dispatch<
										React.SetStateAction<FromToProps<Date>>
									>,
									defaultValue: fromToDateObject,
									renderModal: FromToDateModal,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filters.date,
										fromToDateObject
									),
									areValuesEquals: areFromToCompatibleObjectsEqual,
								})
						: undefined,
				renderCell: ({ row: { salaryFrom, isCurrent } }) => (
					<SalaryCell {...{ isCurrent }}>{salaryFrom}</SalaryCell>
				),
				sortable: true,
			}),
			getColumnWithTooltipFactory({
				key: SALARY_COLUMN_KEYS.salaryUsd,
				name: SALARY_COLUMN_NAMES.salaryUsd,
				width: SALARY_TABLE_COLUMNS_WIDTHS.salaryUsd,
				renderHeaderCell:
					isTabMode && filterSetter && filters
						? (props) =>
								headerCellWithFilterRenderer<SalaryRow, FromToProps<string>>({
									...props,
									filter: filters.salaryUsd,
									setFilter: filterSetter.salaryUsd as React.Dispatch<
										React.SetStateAction<FromToProps<string>>
									>,
									renderModal: (modalProps) =>
										FromToPriceModal({
											...modalProps,
											includeDecimalPart: false,
										}),
									areValuesEquals: areFromToCompatibleObjectsEqual,
									defaultValue: fromToCurrencyObject,
									isFilterApplied: !areFromToCompatibleObjectsEqual(
										filters.salaryUsd,
										fromToCurrencyObject
									),
								})
						: undefined,
				renderCell: ({ row: { salaryUsd, isCurrent } }) => (
					<SalaryCell {...{ isCurrent }}>{salaryUsd}</SalaryCell>
				),
				sortable: true,
			}),
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: areFiltersApplied !== undefined ? !areFiltersApplied : false,
				displayFilterButton: isTabMode,
				clearFilter: resetFilters || undefined,
				sortable: false,
				frozen: true,
			}),
		],
		[isTabMode, areFiltersApplied, filters]
	);
};
