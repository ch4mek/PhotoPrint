import { MODULUS_HOURS_DIVIDER } from '../constants';

import { timeUnits } from '@/constants/date';

export const convertCellIndexToTime = (cellIndex: number, baseHour: number) => {
	const interval = timeUnits.MINUTES_IN_HOUR;

	const timeInMinutes = (cellIndex * interval) / 2;
	const totalMinutes = baseHour * interval + timeInMinutes;
	const hours = Math.floor(totalMinutes / interval) % MODULUS_HOURS_DIVIDER;
	const minutes = totalMinutes % interval;

	const now = new Date();
	const date = new Date(
		now.getFullYear(),
		now.getMonth(),
		now.getDate(),
		hours,
		minutes
	);

	return date;
};
