/* eslint-disable consistent-return */
// TODO: functions which have complex logic with date should'n be in @/helpers

import { RangeTimePeriod, TimePeriod } from '../types';

import {
	months,
	date,
	COMPANY_FOUNDATION_DATE,
	quartersShorthands,
	DATE_FORMAT,
	currentDate,
} from '@/constants/date';
import dayjs, { ConfigType, Dayjs } from '@/lib/dayjs';

export const generateYearsCompanyExistence = () => {
	const arr = [];

	for (let i = date.getUTCFullYear(); i >= COMPANY_FOUNDATION_DATE; i -= 1) {
		arr.push(i);
	}

	return arr;
};

const getQuarterDependOnMonth = (month: string) => {
	switch (month) {
		case 'January':
		case 'February':
		case 'March':
			return 'January-March';
		case 'April':
		case 'May':
		case 'June':
			return 'April-June';
		case 'July':
		case 'August':
		case 'September':
			return 'July-September';
		case 'October':
		case 'November':
		case 'December':
			return 'October-December';
		default:
			break;
	}
};

export const generateShorthandOfQuarter = (quarter?: string) => {
	if (!quarter) {
		return '';
	}
	const [startMonth, endMonth] = quarter.split('-');

	return `${startMonth.slice(0, 3)}-${endMonth.slice(0, 3)}, `;
};

export const generateAvailableRangeFromCompanyFoundation = (
	range: RangeTimePeriod,
	toDate?: Date
) => {
	const options = [];
	const calculatedDate = toDate || new Date();
	const companyFoundationDate = new Date(COMPANY_FOUNDATION_DATE, 0, 1);

	while (calculatedDate >= companyFoundationDate) {
		const indexOfMonth = calculatedDate.getMonth();
		const year = calculatedDate.getFullYear();
		const month = months[indexOfMonth];

		if (range === RangeTimePeriod.Month) {
			options.push(`${month}, ${year}`);
		} else {
			options.push(
				`${generateShorthandOfQuarter(getQuarterDependOnMonth(month))}${year}`
			);
		}
		calculatedDate.setDate(1);
		calculatedDate.setMonth(indexOfMonth - range);
	}

	return options;
};

export const changeSelectDate = (title: string, value: string) => {
	switch (title) {
		case TimePeriod.Year:
			return {
				year: parseInt(value, 10),
			};
		case TimePeriod.Month: {
			const [month, year] = value.split(', ');

			return {
				year,
				month,
			};
		}
		case TimePeriod.Quarter: {
			const [quarterShorthand, year] = value.split(', ');

			return {
				year,
				quarter: quartersShorthands[quarterShorthand],
			};
		}
		default:
			break;
	}
};

export const generateQuarter = (quarter: string) => {
	switch (quarter) {
		case 'January-March':
			return ['January', 'February', 'March'];
		case 'April-June':
			return ['April', 'May', 'June'];
		case 'July-September':
			return ['July', 'August', 'September'];
		case 'October-December':
			return ['October', 'November', 'December'];
		default:
			break;
	}
};

export const generateMonths = (array: (typeof months)[number][]) =>
	array.map((item) => months.lastIndexOf(item) + 1);

export const parseDayjsDateToUTCString = (dateDayjs?: Dayjs | null) =>
	dateDayjs ? `${dayjs.utc(dateDayjs).local().format().slice(0, -6)}Z` : null;

export const generateDateOptions = (title: TimePeriod) => {
	switch (title) {
		case TimePeriod.Year:
			return generateYearsCompanyExistence().map((year) => year.toString());
		case TimePeriod.Month:
			return generateAvailableRangeFromCompanyFoundation(RangeTimePeriod.Month);
		case TimePeriod.Quarter:
			return generateAvailableRangeFromCompanyFoundation(
				RangeTimePeriod.Quarter
			);
		default:
			return generateAvailableRangeFromCompanyFoundation(RangeTimePeriod.Month);
	}
};

export const isDateMoreThanCurrent = (targetDate: Date) => {
	const currentDateFromDayStart = dayjs().startOf('day');
	const targetDateFromDayStart = dayjs(targetDate, DATE_FORMAT, true).startOf(
		'day'
	);

	return currentDateFromDayStart.isSameOrBefore(targetDateFromDayStart);
};

export function isCurrentYearMonth(month: number, year: number): boolean {
	return currentDate.month() + 1 === month && currentDate.year() === year;
}

export function isFutureMonth(month: number, year: number): boolean {
	const inputDate = dayjs({ year, month: month - 1 });

	return inputDate.isAfter(currentDate);
}

export function generateYearsWithFutureYearOptions() {
	const currentYear = new Date().getFullYear();
	const years = generateYearsCompanyExistence();

	return [currentYear + 1, ...years].map(Number);
}

export const formatUtcToLocal = (
	dateUtc: ConfigType,
	nullDateOutput = '-',
	format: string = DATE_FORMAT
) =>
	dateUtc == null ? nullDateOutput : dayjs.utc(dateUtc).local().format(format);

export const formatUtcTime = (utcTime?: string) => utcTime?.slice(0, 5);
