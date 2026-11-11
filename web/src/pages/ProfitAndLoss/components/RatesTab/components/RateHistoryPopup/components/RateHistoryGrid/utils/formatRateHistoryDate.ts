import { months } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const formatRateHistoryDate = (date: string) => {
	const parsedDate = dayjs(date);

	return `${months[parsedDate.month()]} ${parsedDate.year()}`;
};
