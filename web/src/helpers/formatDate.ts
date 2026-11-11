import { DateFormat } from '@/constants/date';
import dayjs, { Dayjs } from '@/lib/dayjs';

export const formatDate = (
	date: Date | Dayjs | string,
	dateFormat: DateFormat
) => dayjs(date).format(dateFormat);
