import { DateFormat } from '@/constants/date';
import { formatDate } from '@/helpers/formatDate';

export const getAssignmentText = (dateFrom: string, dateTo: string | null) => {
	const formattedFromDate = formatDate(dateFrom, DateFormat.DD_MM_YYYY);

	if (dateTo) {
		const formattedToDate = formatDate(dateTo, DateFormat.DD_MM_YYYY);

		return `Assigned: ${formattedFromDate} - ${formattedToDate}`;
	}

	return `Assigned: ${formattedFromDate}`;
};
