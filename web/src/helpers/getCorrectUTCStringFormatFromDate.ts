import { parseDayjsDateToUTCString } from '@/helpers/date';
import dayjs from '@/lib/dayjs';

export const getCorrectUTCStringFormatFromDate = (date: Date) =>
	parseDayjsDateToUTCString(dayjs(date));
