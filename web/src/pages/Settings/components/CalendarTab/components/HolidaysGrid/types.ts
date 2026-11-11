import { Dispatch, SetStateAction } from 'react';

import { FromToProps, GridCellDefaultProps } from '@/components/Grid';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { GridColumnWidth } from '@/types/grid';

export type HolidaysTableFilters = HolidaysFilters;

export enum HolidaysFilterActions {
	RESET = 'reset',
	HOLIDAY_NAME = 'name',
	HOLIDAY_TYPE = 'dateType',
	COUNTRY_ID = 'countryId',
	DATE = 'date',
	NEW_DATE = 'newDate',
}

export type HolidaysAction = {
	type: HolidaysFilterActions;
	payload?: string | string[] | HolidaysFilters | FromToProps<Date>;
};

export interface HolidaysFilters {
	[HolidaysFilterActions.DATE]: FromToProps<Date>;
	[HolidaysFilterActions.NEW_DATE]: FromToProps<Date>;
	[HolidaysFilterActions.HOLIDAY_TYPE]: string[];
	[HolidaysFilterActions.HOLIDAY_NAME]: string;
	[HolidaysFilterActions.COUNTRY_ID]: string[];
}

export type HolidayGridRow = GridCellDefaultProps &
	Record<keyof HolidaysFilters, string>;

export interface HolidaysFiltersOptions {
	dateTypes: Array<SelectionModalOption>;
	countries: Array<SelectionModalOption>;
}

export type HolidaysFiltersHandlers = {
	[K in HolidaysFilterActions]: K extends keyof HolidaysFilters
		? Dispatch<SetStateAction<HolidaysFilters[K]>>
		: VoidFunction;
};

export type HolidaysGridColumnWidths = Record<
	keyof HolidaysFilters,
	GridColumnWidth
>;
