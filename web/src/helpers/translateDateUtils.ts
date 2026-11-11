import { MONTH_IN_YEAR } from '@/constants/date';

const monthsToYears = (months: number) => Math.floor(months / MONTH_IN_YEAR);
const getRemainingMonths = (months: number) => months % MONTH_IN_YEAR;

export const monthsToYearsAndMonthsObject = (months: number) => ({
	years: monthsToYears(months),
	months: getRemainingMonths(months),
});
