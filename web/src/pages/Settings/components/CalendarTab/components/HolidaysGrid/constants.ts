import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { fromToDateObject } from '@/constants/defaultValues';
import { convertConstantToSelectionModalOption } from '@/helpers/convertToSelectionModalOptions';
import { CalendarExclusions } from '@/queries/calendar/types';

import { HolidaysTableFilters } from './types';

export const HOLIDAYS_CONTAINER_TITLE = 'Holidays and days off';
export const HOLIDAYS_POPUP_TITLE = 'Holiday and day off';
export const SORT_DATE = 'date';

export const holidaysColumns = {
	date: {
		key: 'date',
		name: 'Date',
		sortable: true,
	},
	newDate: {
		key: 'newDate',
		name: 'New workday date',
		sortable: true,
	},
	dateType: {
		key: 'dateType',
		name: 'Type',
		sortable: true,
	},
	name: {
		key: 'name',
		name: 'Name',
		sortable: true,
	},
	countryId: {
		key: 'countryId',
		name: 'Country',
		sortable: true,
	},
};

export const holidaysGridColumnWidths = {
	date: {
		fixed: false,
		width: 100,
		maxWidth: 155,
	},
	newDate: {
		fixed: true,
		width: 155,
	},
	dateType: {
		fixed: false,
		width: 79,
		maxWidth: 350,
	},
	name: {
		fixed: false,
		width: 260,
	},
	countryId: {
		fixed: false,
		width: 97,
		maxWidth: 350,
	},
};

export const DAY_OFF_LABLE = 'Day off';
export const HOLIDAY_LABLE = 'Holiday';

export const dateTypesOptions: Array<SelectionModalOption> = [
	{ id: CalendarExclusions.DayOff, description: DAY_OFF_LABLE },
	convertConstantToSelectionModalOption(CalendarExclusions.Holiday),
];

export const defaultHolidaysFilters: HolidaysTableFilters = {
	dateType: [],
	countryId: [],
	newDate: fromToDateObject,
	date: fromToDateObject,
	name: '',
};

export const defaultHolidaysFiltersValues: HolidaysTableFilters = {
	dateType: [],
	countryId: [],
	newDate: fromToDateObject,
	date: fromToDateObject,
	name: '',
};
