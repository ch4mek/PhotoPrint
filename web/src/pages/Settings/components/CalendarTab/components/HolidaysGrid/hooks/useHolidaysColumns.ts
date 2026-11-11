import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	defaultHolidaysFiltersValues,
	holidaysColumns,
	holidaysGridColumnWidths,
} from '../constants';
import {
	HolidayGridRow,
	HolidaysFilters,
	HolidaysFiltersHandlers,
	HolidaysFiltersOptions,
} from '../types';
import { isResetDisabled } from '../utils';

import {
	FreeTextAutoComplete,
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
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';

export function useHolidaysColumns(
	filters: HolidaysFilters,
	setFilters: HolidaysFiltersHandlers,
	filtersOptions: HolidaysFiltersOptions,
	holidayNamesOptions: string[]
) {
	const columns: Array<Column<HolidayGridRow>> = useMemo(
		() => [
			getColumnWithTooltipFactory({
				...holidaysColumns.date,
				width: getColumnWidth(holidaysGridColumnWidths.date).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.date,
						setFilter: setFilters.date,
						defaultValue: defaultHolidaysFiltersValues.date,
						isFilterApplied: !!filters.date?.from || !!filters.date?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromToDateModal,
					}),
			}),
			getColumnWithTooltipFactory({
				...holidaysColumns.newDate,
				width: getColumnWidth(holidaysGridColumnWidths.newDate).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.newDate,
						setFilter: setFilters.newDate,
						defaultValue: defaultHolidaysFiltersValues.newDate,
						isFilterApplied: !!filters.newDate?.from || !!filters.newDate?.to,
						areValuesEquals: areFromToCompatibleObjectsEqual,
						renderModal: FromToDateModal,
					}),
			}),
			getColumnWithTooltipFactory({
				...holidaysColumns.dateType,
				width: getColumnWidth(holidaysGridColumnWidths.dateType).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.dateType,
						setFilter: setFilters.dateType,
						defaultValue: defaultHolidaysFiltersValues.dateType,
						renderModal: SelectionModal,
						options: filtersOptions.dateTypes,
						isFilterApplied: filters.dateType.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
			}),
			getColumnWithTooltipFactory({
				...holidaysColumns.name,
				width: getColumnWidth(holidaysGridColumnWidths.name).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.name,
						setFilter: setFilters.name,
						defaultValue: defaultHolidaysFiltersValues.name,
						renderModal: FreeTextAutoComplete,
						isFilterApplied: !!filters.name,
						disableFilterIconActiveStateAsDefault: true,
						autocompleteOptions: holidayNamesOptions,
						areValuesEquals: arePrimitiveValuesEqual,
					}),
			}),
			getColumnWithTooltipFactory({
				...holidaysColumns.countryId,
				width: getColumnWidth(holidaysGridColumnWidths.countryId).width,

				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.countryId,
						setFilter: setFilters.countryId,
						defaultValue: defaultHolidaysFiltersValues.countryId,
						renderModal: SelectionModal,
						options: filtersOptions.countries,
						isFilterApplied: filters.countryId.length > 0,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						disableFilterIconActiveStateAsDefault: true,
					}),
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
		[filters, holidayNamesOptions]
	);

	return columns;
}
