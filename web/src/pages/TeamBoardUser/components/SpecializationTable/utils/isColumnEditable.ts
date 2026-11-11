import dayjs from '@/lib/dayjs';

export const isColumnEditable = (isFirst: boolean, date: string) =>
	isFirst &&
	dayjs(date).isBetween(
		dayjs().startOf('day').subtract(3, 'day'),
		dayjs().startOf('day'),
		'day',
		'[]'
	);
