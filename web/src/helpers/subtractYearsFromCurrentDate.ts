import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export function subtractPeriodFromCurrentDate(
	period: 'years' | 'months' | 'days',
	amount: number,
	format: string = YYYY_MM_DD_DAYJS_DATE_FORMAT
) {
	const subtractedDate = dayjs().subtract(amount, period);

	const formattedDate = subtractedDate.format(format);

	return formattedDate;
}
