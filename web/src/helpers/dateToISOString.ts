import { DATE_FORMAT } from '@/constants/date';
import dayjs, { ConfigType } from '@/lib/dayjs';

export function isNeedZero(number: string | number) {
	if (Number(number) < 10) {
		return `0${Number(number)}`;
	}

	return number;
}

export const dateToString = (date: Date | string, period: string) =>
	period === 'This Month'
		? `${(date as string).split(',')[2].slice(1, 5)}-${isNeedZero(
				dayjs(date).month() + 1
		  )}-${isNeedZero((date as string).split(',')[1].slice(-2))}T00:00:00.000Z`
		: `${(date as Date).getFullYear()}-${isNeedZero(
				(date as Date).getMonth() + 1
		  )}-${isNeedZero((date as Date).getDate())}T00:00:00.000Z`;

export const formatDateString = (
	dateISO: ConfigType,
	nullDateOutput = '-',
	to: string = DATE_FORMAT
) => (dateISO === null ? nullDateOutput : dayjs(dateISO).format(to));
