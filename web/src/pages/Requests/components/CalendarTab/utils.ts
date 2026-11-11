import { months } from '@/constants/date';

import { AvailableMonths, CalendarDate, MonthYearDate } from './types';

export const formatMonthYearToString = (date: CalendarDate): MonthYearDate =>
	`${months[date.month]}, ${date.year}`;

export const parseStringToMonthYear = (value: MonthYearDate): CalendarDate => {
	const [monthName, yearString] = value.split(', ');

	const monthIndex = months.indexOf(monthName as AvailableMonths);

	return {
		month: monthIndex,
		year: Number(yearString),
	};
};

export const getNextMonth = ({ month, year }: CalendarDate): CalendarDate => {
	if (month === 11) {
		return {
			month: 0,
			year: year + 1,
		};
	}

	return {
		month: month + 1,
		year,
	};
};

export const getPrevMonth = ({ month, year }: CalendarDate): CalendarDate => {
	if (month === 0) {
		return {
			month: 11,
			year: year - 1,
		};
	}

	return {
		month: month - 1,
		year,
	};
};

export const getNextYearDate = () => {
	const date = new Date();
	date.setFullYear(date.getFullYear() + 1);

	return date;
};
