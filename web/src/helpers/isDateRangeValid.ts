import dayjs from '@/lib/dayjs';

export const isDateRangeValid = (
	startDate?: Date,
	endDate?: Date,
	allowEquals = false
) => {
	if (startDate && endDate) {
		const start = dayjs(startDate);
		const end = dayjs(endDate);

		return allowEquals
			? start.isSameOrBefore(end, 'day')
			: start.isBefore(end, 'day');
	}

	return true;
};
