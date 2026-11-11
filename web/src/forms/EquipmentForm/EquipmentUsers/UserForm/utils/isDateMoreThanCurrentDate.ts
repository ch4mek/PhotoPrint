import dayjs from '@/lib/dayjs';

export const isDateMoreThanCurrentDate = (targetDate: Date) => {
	const currentDateFromDayStart = dayjs().startOf('day');
	const targetDateFromDayStart = dayjs(targetDate).startOf('day');

	return currentDateFromDayStart.isBefore(targetDateFromDayStart);
};
