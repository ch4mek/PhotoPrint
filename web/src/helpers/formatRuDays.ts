import { capitalizeString } from '@/helpers/capitalizeString';

const formatLocalizedDatePart = (
	date: Date,
	locale: string,
	options: Intl.DateTimeFormatOptions
) => {
	const value = date.toLocaleDateString(locale, options);

	return locale.startsWith('ru') ? capitalizeString(value) : value;
};

export const formatWeekdayShort = (date: Date, locale: string) =>
	formatLocalizedDatePart(date, locale, { weekday: 'short' });

export const formatMonthShort = (date: Date, locale: string) =>
	formatLocalizedDatePart(date, locale, { month: 'short' });

export const formatMonthLong = (month: number, year: number, locale: string) =>
	formatLocalizedDatePart(new Date(year, month - 1), locale, { month: 'long' });
