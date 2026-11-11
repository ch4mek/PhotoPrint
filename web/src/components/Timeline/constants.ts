import { LocationTypeId } from '@/types/configuration';

import { TimezoneObject, Timezone } from './types';

export const CELL_WIDTH = 106;
export const CELL_HEIGHT = 54;

export const TABLE_HEADER_HEIGHT = 30;
export const TIME_LABELS_HEIGHT = 24;

export const BOOKED_LABEL_MARGIN = 4;

export const BUSSINESS_HOURS_START = 8;
const BUSINESS_HOURS_END = 20;
export const ALL_HOURS_START = 0;
const ALL_HOURS_END = 24;

export const MIDNIGHT_HOURS = 0;

export const BUSINESS_HOURS_CELLS_AMOUNT =
	BUSINESS_HOURS_END - BUSSINESS_HOURS_START;
export const ALL_HOURS_CELLS_AMOUNT = ALL_HOURS_END - ALL_HOURS_START;

export const MINUTES_IN_HOUR = 60;
export const SECONDS_IN_MINUTE = 60;
export const MODULUS_HOURS_DIVIDER = 25;

export enum TimelinePeriod {
	HOUR_PERIOD = 1,
	HALF_AND_QUARTER_PERIOD = 0.75,
	HALF_PERIOD = 0.5,
	QUARTER_PERIOD = 0.25,
	FULL_DAY = 24,
}

export const DEFAULT_TIMEZONE: TimezoneObject = {
	timezoneDisplayName: LocationTypeId.Minsk,
	timezoneOffset: '+3:00',
	timezone: Timezone.MINSK,
};
