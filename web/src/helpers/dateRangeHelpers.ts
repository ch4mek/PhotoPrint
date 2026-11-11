import { getCorrectUTCStringFormatFromDate } from '@/helpers/getCorrectUTCStringFormatFromDate';
import dayjs from '@/lib/dayjs';

export const getStartOfDayUTC = (date: Date) =>
	getCorrectUTCStringFormatFromDate(dayjs(date).startOf('day').toDate());

export const getEndOfDayUTC = (date: Date) =>
	getCorrectUTCStringFormatFromDate(dayjs(date).endOf('day').toDate());
