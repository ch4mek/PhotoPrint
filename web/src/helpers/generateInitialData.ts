import { months, quarters, TIME_PERIOD } from '@/constants/date';

export const generateInitialData = (
	title: keyof typeof TIME_PERIOD | string
) => {
	const date = new Date();
	switch (title) {
		case TIME_PERIOD.Year:
			return { year: date.getUTCFullYear() };
		case TIME_PERIOD.Month:
			return {
				year: date.getUTCFullYear(),
				month: months[date.getMonth()],
			};
		case TIME_PERIOD.Quarter:
			return {
				year: date.getUTCFullYear(),
				quarter: quarters[Math.floor(date.getMonth() / 3)],
			};
		default:
			return { year: date.getUTCFullYear() };
	}
};
