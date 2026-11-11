import dayjs, { Dayjs } from '@/lib/dayjs';

export const getDateFormat = (date: Dayjs | string | null, format: string) =>
	date ? dayjs(date).format(format) : null;
