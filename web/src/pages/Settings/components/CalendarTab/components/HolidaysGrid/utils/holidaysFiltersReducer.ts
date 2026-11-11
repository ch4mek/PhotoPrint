import { defaultHolidaysFiltersValues } from '../constants';
import {
	HolidaysAction,
	HolidaysFilterActions,
	HolidaysFilters,
} from '../types';

import { FromToProps } from '@/components/Grid';

export const holidaysFiltersReducer = (
	state: HolidaysFilters,
	{ type, payload }: HolidaysAction
): HolidaysFilters => {
	switch (type) {
		case HolidaysFilterActions.HOLIDAY_NAME:
			return { ...state, name: payload as string };
		case HolidaysFilterActions.HOLIDAY_TYPE:
			return { ...state, dateType: payload as string[] };
		case HolidaysFilterActions.COUNTRY_ID:
			return { ...state, countryId: payload as string[] };
		case HolidaysFilterActions.DATE:
			return { ...state, date: payload as FromToProps<Date> };
		case HolidaysFilterActions.NEW_DATE:
			return { ...state, newDate: payload as FromToProps<Date> };
		case HolidaysFilterActions.RESET:
			return { ...defaultHolidaysFiltersValues };
		default:
			return state;
	}
};
