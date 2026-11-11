import { CalendarHighlightedDays, HighlightedDays } from '@/context/types';
import dayjs, { ConfigType, Dayjs } from '@/lib/dayjs';
import { getBlockedMonthDays } from '@/pages/Booking/utils';

export const getNewHighlightedDays = (
	highlightedDays: HighlightedDays,
	lastDay: Dayjs | null,
	startOfMonth: ConfigType,
	endOfMonth: ConfigType
): Record<keyof CalendarHighlightedDays, number[]> => {
	const result: CalendarHighlightedDays = {
		bookedList: [],
		yourList: [],
		blockedList: [],
	};
	Object.keys(highlightedDays).forEach((key) => {
		if (key === 'blockStartDate') {
			const blockStartDate = highlightedDays.blockStartDate;

			if (
				blockStartDate === null ||
				lastDay?.isBefore(blockStartDate, 'month')
			) {
				result.blockedList = [];

				return;
			}

			const blockedDays = getBlockedMonthDays();
			result.blockedList = lastDay?.isSame(blockStartDate, 'month')
				? blockedDays.filter((day) => day >= blockStartDate.date())
				: blockedDays;
		} else {
			result[key as keyof CalendarHighlightedDays] = highlightedDays[
				key as keyof Omit<HighlightedDays, 'blockStartDate'>
			]
				.filter((day: ConfigType) =>
					dayjs(day).isBetween(startOfMonth, endOfMonth, 'day', '[]')
				)
				.map((day: ConfigType) => dayjs(day).date());
		}
	});

	return result;
};
