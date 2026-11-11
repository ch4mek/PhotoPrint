import dayjs from '@/lib/dayjs';

export const getCalendarTimelineItemWidth = (
	startDate: string,
	endDate: string,
	cellWidth: number
) => {
	const days = dayjs(endDate).diff(dayjs(startDate), 'day') + 1;

	return days * cellWidth;
};
