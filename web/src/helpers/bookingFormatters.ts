import i18next, { TFunction } from 'i18next';

import {
	ALL_DAY_TIME_HH_MM,
	BOOKING_ITEM_TIME_FORMAT,
	BOOKING_ITEM_TIME_FORMAT_WITH_COMMA,
	RESPONSE_ALL_DAY_END_TIME_FORMAT,
	RESPONSE_ALL_DAY_START_TIME_FORMAT,
	RESPONSE_TIME_FORMAT,
	UTC_RESPONSE_TIME_FORMAT,
	VIEW_TIME_FORMAT,
} from '@/constants/booking';
import { DateFormat } from '@/constants/date';
import { capitalizeString } from '@/helpers/capitalizeString';
import { getDateLocale } from '@/i18n/locale';
import dayjs, { Dayjs } from '@/lib/dayjs';

import { calculateTimeDifferenceInMinutes } from './bookingTime';
import { minsToHrsAndMinsObject } from './translateTimeUtils';

type BookingDisplayDateFormat =
	| typeof BOOKING_ITEM_TIME_FORMAT
	| typeof BOOKING_ITEM_TIME_FORMAT_WITH_COMMA;

type DateParts = {
	day: string;
	month: string;
	year: string;
};

export const formatDateParts = (date: Date, locale: string): DateParts => {
	const formatter = new Intl.DateTimeFormat(locale, {
		day: '2-digit',
		month: 'short',
		year: 'numeric',
	});

	const parts = formatter.formatToParts(date);
	const get = (type: Intl.DateTimeFormatPartTypes) =>
		parts.find((part) => part.type === type)?.value ?? '';

	let month = get('month');

	if (locale.startsWith('ru')) {
		month = capitalizeString(month);
	}

	return {
		day: get('day'),
		month,
		year: get('year'),
	};
};

const formatPartsToString = (
	{ day, month, year }: DateParts,
	format: BookingDisplayDateFormat
) => {
	if (format === BOOKING_ITEM_TIME_FORMAT_WITH_COMMA) {
		return `${day} ${month}, ${year}`;
	}

	return `${day} ${month} ${year}`;
};

export const formatDisplayDate = (
	date: Date | Dayjs | string,
	format: BookingDisplayDateFormat,
	language = i18next.language
) => {
	const locale = getDateLocale(language);
	const parts = formatDateParts(dayjs(date).utc(false).toDate(), locale);

	return formatPartsToString(parts, format);
};

export const getFormattedTime = (
	hours: string | number,
	minutes: string | number,
	inUtc: boolean = false
) => {
	const time = `${String(hours).padStart(2, '0')}:${String(minutes).padStart(
		2,
		'0'
	)}`;

	return inUtc
		? dayjs.utc(time, VIEW_TIME_FORMAT).format(DateFormat.ISO_DATETIME)
		: time;
};

export const getFormattedTimePeriod = (
	fromTime: string,
	toTime: string,
	calculateHours: boolean,
	t: TFunction<'common'>
) => {
	const fromTimeDate = dayjs(fromTime);
	let toTimeDate = dayjs(toTime);

	const isToTimeEndOfDay =
		toTimeDate.format(RESPONSE_TIME_FORMAT) ===
		RESPONSE_ALL_DAY_END_TIME_FORMAT;

	if (
		fromTimeDate.format(RESPONSE_TIME_FORMAT) ===
			RESPONSE_ALL_DAY_START_TIME_FORMAT &&
		isToTimeEndOfDay
	) {
		return t('allDay');
	}

	if (isToTimeEndOfDay) {
		toTimeDate = toTimeDate.add(1, 'seconds');
	}

	if (!calculateHours) {
		return `${fromTimeDate.format(VIEW_TIME_FORMAT)} - ${toTimeDate.format(
			VIEW_TIME_FORMAT
		)}`;
	}

	const { hours: differenceHours, minutes: differenceMinutes } =
		minsToHrsAndMinsObject(
			calculateTimeDifferenceInMinutes(fromTimeDate, toTimeDate)
		);

	const differenceMinutesString = differenceMinutes
		? `${differenceMinutes} ${t('time.minutes')}`
		: '';
	const differenceHoursString = differenceHours
		? `${differenceHours} ${t('time.hours')}`
		: '';
	const hasSpace = differenceHoursString && differenceMinutesString;

	return `${fromTimeDate.format(VIEW_TIME_FORMAT)} - ${toTimeDate.format(
		VIEW_TIME_FORMAT
	)} (${differenceHoursString}${
		hasSpace ? ' ' : ''
	}${differenceMinutesString})`;
};

export const getFormattedDateTimePeriodWithUTC = (
	fromTime: string,
	toTime: string,
	t: TFunction<'common'>
) => {
	const formattedFrom = dayjs(fromTime).format(VIEW_TIME_FORMAT);
	const formattedTo = dayjs(toTime).format(VIEW_TIME_FORMAT);

	if (
		formattedFrom === ALL_DAY_TIME_HH_MM.from &&
		formattedTo === ALL_DAY_TIME_HH_MM.to
	) {
		return t('allDay');
	}

	const fromTimeDate = dayjs(fromTime);
	let toTimeDate = dayjs(toTime);

	if (
		toTimeDate.format(UTC_RESPONSE_TIME_FORMAT) ===
		RESPONSE_ALL_DAY_END_TIME_FORMAT
	) {
		toTimeDate = toTimeDate.add(1, 'second');
	}

	return `${fromTimeDate.format(VIEW_TIME_FORMAT)} - ${toTimeDate.format(
		VIEW_TIME_FORMAT
	)}`;
};
