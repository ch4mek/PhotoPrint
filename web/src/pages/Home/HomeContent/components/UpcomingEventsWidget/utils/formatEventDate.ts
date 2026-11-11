import { formatDateString } from '@/helpers/dateToISOString';
import { UpcomingEvent, UpcomingEventType } from '@/types/home';

export const formatEventDate = (event: UpcomingEvent): string => {
	const { type, startDate, endDate, inquiryType } = event;

	if (type === UpcomingEventType.Absence && inquiryType && endDate) {
		return `${formatDateString(startDate, '')} - ${formatDateString(
			endDate,
			''
		)}`;
	}

	return formatDateString(startDate, '');
};
