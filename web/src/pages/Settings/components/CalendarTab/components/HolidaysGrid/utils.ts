import { areArraysEqualByPrimitiveItems } from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import { CalendarExclusions } from '@/queries/calendar/types';

import {
	DAY_OFF_LABLE,
	defaultHolidaysFilters,
	HOLIDAY_LABLE,
} from './constants';
import { HolidaysFilters } from './types';

export const isResetDisabled = (filters: HolidaysFilters) => {
	const keys = Object.keys(filters) as Array<keyof typeof filters>;

	const isDisabled = keys.every((key) => {
		const currentField = filters[key];
		const defaultField = defaultHolidaysFilters[key];

		if (Array.isArray(currentField) && Array.isArray(defaultField)) {
			return areArraysEqualByPrimitiveItems(currentField, defaultField);
		}

		return currentField === defaultField;
	});

	return isDisabled;
};

export const getDayjsDate = (date: string | null) =>
	date !== null ? dayjs(date) : null;

export const getCalendarExclusionsTitle = (dateType: CalendarExclusions) =>
	dateType === CalendarExclusions.DayOff ? DAY_OFF_LABLE : HOLIDAY_LABLE;
