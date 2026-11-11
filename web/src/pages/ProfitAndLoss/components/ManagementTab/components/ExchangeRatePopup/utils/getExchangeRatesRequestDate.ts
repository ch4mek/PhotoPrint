import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs, { Dayjs } from '@/lib/dayjs';

export const getExchangeRatesRequestDate = (date: Dayjs) => {
	const isCurrentMonth = date.isSame(dayjs(), 'month');
	const requestDate = isCurrentMonth ? dayjs() : date.endOf('month');

	return requestDate.format(YYYY_MM_DD_DAYJS_DATE_FORMAT);
};
