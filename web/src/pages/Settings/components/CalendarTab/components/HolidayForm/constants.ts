import { CalendarExclusions } from '@/queries/calendar/types';

export const HOLIDAY_FORM_FIELDS = {
	countryId: {
		name: 'countryId',
		label: 'Country',
	},
	name: {
		name: 'name',
		label: 'Name',
	},
	dateType: {
		name: 'dateType',
		label: 'Day type:',
	},
	newDate: {
		name: 'newDate',
		label: 'New workday date',
	},
	date: {
		name: 'date',
		label: 'Date',
	},
};

export const DATE_TYPE_RADIO_BUTTONS_NAME = 'handleDateType';

export const DEFAULT_HOLIDAY_VALUES = {
	countryId: null,
	dateType: CalendarExclusions.Holiday,
	date: null,
	newDate: null,
	name: '',
};

export const THE_DATE_TEXT = 'The date';
export const NEW_WORKDAY_DATE_TEXT = 'New workday date';
export const THE_DATE_AND_WORKDAY_DATE_TEXT = 'Date and new workday date';
export const FORM_NAME = 'Settings holiday form';
