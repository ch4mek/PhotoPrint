import * as yup from 'yup';

import {
	MIN_COUNTRY_CALENDAR_HOURS,
	MAX_COUNTRY_CALENDAR_HOURS,
} from '@/constants/settings';

export const hoursSchema = yup
	.number()
	.nullable()
	.transform((value) => (value === '' ? null : value))
	.min(MIN_COUNTRY_CALENDAR_HOURS)
	.max(MAX_COUNTRY_CALENDAR_HOURS);
