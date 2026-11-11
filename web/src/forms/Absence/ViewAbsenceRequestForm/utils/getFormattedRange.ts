import { SHORT_MONTH_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const getFormattedRange = (dateFrom: string, dateTo: string | null) => {
	const start = dayjs(dateFrom).format(SHORT_MONTH_DATE_FORMAT);

	if (!dateTo || dateFrom === dateTo) {
		return start;
	}

	const end = dayjs(dateTo).format(SHORT_MONTH_DATE_FORMAT);

	return `${start} - ${end}`;
};
