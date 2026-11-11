import { months } from '@/constants/date';
import { generateAvailableRangeFromCompanyFoundation } from '@/helpers/date';
import dayjs, { Dayjs } from '@/lib/dayjs';
import { AvailableMonths } from '@/types/dates';
import { MonthYearOption } from '@/types/forms';
import { RangeTimePeriod } from '@/types/index';

export const getCurrentMonthDate = (): Dayjs => dayjs().startOf('month');

export const getMonthYearOptions = (): MonthYearOption[] =>
	generateAvailableRangeFromCompanyFoundation(
		RangeTimePeriod.Month
	) as MonthYearOption[];

export const formatMonthYearDate = (date: Dayjs): MonthYearOption =>
	`${months[date.month()]}, ${date.year()}`;

export const parseMonthYearOption = (value: MonthYearOption): Dayjs => {
	const [monthName, yearString] = value.split(', ');
	const monthIndex = months.indexOf(monthName as AvailableMonths);

	return dayjs(new Date(Number(yearString), monthIndex, 1));
};
