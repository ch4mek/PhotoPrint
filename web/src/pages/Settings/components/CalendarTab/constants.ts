import {
	CalendarMonthTime,
	WorkingHoursCalendarRow,
	WorkingHoursCalendarRowType,
} from '@/types/settings';

import { CalendarGridColumnWidths } from './types';

export const NEW_COUNTRY_ROW: WorkingHoursCalendarRow = {
	type: WorkingHoursCalendarRowType.NewCountryRow,
	countryName: '',
	countryId: 0,
	timeMap: new Map(),
};

export const ADD_COUNTRY_ROW: WorkingHoursCalendarRow = {
	...NEW_COUNTRY_ROW,
	type: WorkingHoursCalendarRowType.AddCountryButtonRow,
};

export const CALENDAR_TOTAL_COLUMN_HEADER_CLASS =
	'calendar-total-column-header';
export const CALENDAR_TOTAL_COLUMN_CELL_CLASS = 'calendar-total-column-cell';
export const CALENDAR_COUNTRY_COLUMN_HEADER_CLASS =
	'calendar-country-column-header';
export const CALENDAR_COUNTRY_COLUMN_CELL_CLASS =
	'calendar-country-column-cell';
export const ADD_BUTTON_CELL_CLASS_NAME = 'add-button-cell';

export const desktopCalendarGridWidths: CalendarGridColumnWidths = {
	countryName: 216,
	month: 80,
	total: 81,
};

export const DEFAULT_MONTH_TIME: CalendarMonthTime = {
	hours: 0,
	id: 0,
	isLoading: false,
	hasError: false,
};

export const COUNTRY_NAME_COLUMN_INDEX = 1;
export const TOTAL_COLUMN_INDEX = 15;

export const CALENDAR_SKELETON_ROW_COUNT = 2;

export const CALENDAR_SKELETON_ROW: WorkingHoursCalendarRow = {
	countryId: 0,
	countryName: '',
	timeMap: new Map(),
	type: WorkingHoursCalendarRowType.CountryRow,
	disabled: false,
};

export const CALENDAR_ADD_BUTTON_TOOLTIP_MESSAGE =
	'First you need to enter country name';

export const COUNTRY_NAME_FREEZE_BREAKPOINT = 577;
export const TOTAL_FREEZE_BREAKPOINT = 641;
