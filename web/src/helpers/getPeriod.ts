import {
	CURRENT_YEAR,
	MAX_DATE_DAY,
	MAX_DATE_MONTH,
	MIN_DATE_DAY,
	MIN_DATE_MONTH,
	months,
} from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const getPeriod = (
	year = CURRENT_YEAR,
	periodMonths?: (typeof months)[number][]
) => {
	if (periodMonths?.length === 0) {
		return [
			dayjs(new Date(year, MIN_DATE_MONTH, MIN_DATE_DAY)),
			dayjs(new Date(year, MAX_DATE_MONTH, MAX_DATE_DAY)),
		];
	}

	if (periodMonths?.length === 1) {
		const monthIndex = months.indexOf(periodMonths[0]);
		const lastDay = dayjs(new Date(year, monthIndex + 1, 0)).date();

		return [
			dayjs(new Date(year, monthIndex, 1)),
			dayjs(new Date(year, monthIndex, lastDay)),
		];
	}

	return periodMonths?.map((month, index) => {
		const monthIndex = months.indexOf(month);
		const lastDay = dayjs(new Date(year, monthIndex + 1, 0)).date();

		return dayjs(
			new Date(
				year ?? CURRENT_YEAR,
				monthIndex,
				index === 0 ? MIN_DATE_DAY : lastDay
			)
		);
	});
};
