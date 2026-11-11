import { generateYearsCompanyExistence } from '@/helpers/date';
import dayjs from '@/lib/dayjs';

export const date = new Date();
export const currentDate = dayjs(date);
export const CURRENT_YEAR = date.getFullYear();

export const COMPANY_FOUNDATION_DATE = 2020;

export const MONTH_IN_YEAR = 12;

export const PERIOD = [
	'Jan',
	'Feb',
	'Mar',
	'Apr',
	'May',
	'Jun',
	'Jul',
	'Aug',
	'Sep',
	'Oct',
	'Nov',
	'Dec',
];

export const TIME_PERIOD = {
	Year: 'Year',
	Month: 'Month',
	Quarter: 'Quarter',
};

export const quarters = [
	'January-March',
	'April-June',
	'July-September',
	'October-December',
];

export const quartersShorthands: Record<string, string> = {
	'Jan-Mar': 'January-March',
	'Apr-Jun': 'April-June',
	'Jul-Sep': 'July-September',
	'Oct-Dec': 'October-December',
};

export const months = [
	'January',
	'February',
	'March',
	'April',
	'May',
	'June',
	'July',
	'August',
	'September',
	'October',
	'November',
	'December',
];

export const dateNavigatorSelectors = [
	{
		title: TIME_PERIOD.Year,
		items: generateYearsCompanyExistence(),
	},
	{
		title: TIME_PERIOD.Month,
		items: months,
	},
	{
		title: TIME_PERIOD.Quarter,
		items: quarters,
	},
];

export const MIN_SELECT_YEAR = 1900;
export const MAX_SELECT_YEAR = 2100;

export const MIN_DATE_MONTH = 0;
export const MIN_DATE_DAY = 1;
export const MIN_DATE_YEAR = 2000;
export const MAX_DATE_MONTH = 11;
export const MAX_DATE_DAY = 31;
export const MAX_DATE_YEAR = 2120;

export const YYYY_MM_DD_DAYJS_DATE_FORMAT = 'YYYY-MM-DD';
export const DATE_SEPARATOR = '.';
export const DATE_FORMAT = 'DD.MM.YYYY';
export const TIME_FORMAT = 'HH:mm';
const FULL_DATETIME_FORMAT = 'DD MMM YYYY, HH:mm';
const ISO_DATETIME_FORMAT = 'YYYY-MM-DDTHH:mm:ss';
export const FULL_MONTH_DATE_TYPE = 'D MMMM YYYY';
const ISO_FORMAT = 'YYYY-MM-DD[T]HH:mm:ss.SSS[Z]';
export const SHORT_MONTH_DATE_FORMAT = 'D MMM YYYY';
export const FULL_MONTH_WITHOUT_DATE_TYPE = 'MMMM YYYY';

export const COMMENT_TIMESTAMP = 'DD MMM YYYY, HH:mm';

export enum DateFormat {
	DD_MM_YYYY = DATE_FORMAT,
	YYYY_MM_DD = YYYY_MM_DD_DAYJS_DATE_FORMAT,
	ISO = ISO_FORMAT,
	ISO_DATETIME = ISO_DATETIME_FORMAT,
	FULL_DATETIME = FULL_DATETIME_FORMAT,
}

export const timeUnits = {
	MILLISECONDS_IN_SECOND: 1000,
	SECONDS_IN_MINUTE: 60,
	MINUTES_IN_HOUR: 60,
	HOURS_IN_DAY: 24,
	MILLISECONDS_IN_MINUTE: 1000 * 60,
	MILLISECONDS_IN_HOUR: 1000 * 60 * 60,
	MILLISECONDS_IN_DAY: 1000 * 60 * 60 * 24,
};

export const MIN_FULL_DATE = dayjs('01-01-2020', DATE_FORMAT);

export const MIN_PICKER_DATE = dayjs([
	MIN_DATE_YEAR,
	MIN_DATE_MONTH,
	MIN_DATE_DAY,
]);

export const MAX_PICKER_DATE = dayjs([
	MAX_DATE_YEAR,
	MAX_DATE_MONTH,
	MAX_DATE_DAY,
]);
