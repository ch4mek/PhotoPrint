import { BookedDataTime } from '../types';

import { timeUnits } from '@/constants/date';

export const convertTimeToCellIndex = (
	{ hours, minutes }: BookedDataTime,
	baseHour: number
) => {
	const cellIndex = hours - baseHour + minutes / timeUnits.MINUTES_IN_HOUR;

	return cellIndex;
};
