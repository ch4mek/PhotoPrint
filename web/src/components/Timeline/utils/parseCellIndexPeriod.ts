import { SelectedTimePeriod } from '../types';

import { convertCellIndexToTime } from './convertCellIndexToTime';

export const parseCellIndexPeriod = (
	cellIndexPeriod: SelectedTimePeriod<number>,
	baseHour: number
) => {
	const { from, to, isOverlapped } = cellIndexPeriod;

	const parsedPeriod = {
		from: convertCellIndexToTime(from, baseHour),
		to: convertCellIndexToTime(to, baseHour),
		isOverlapped,
	};

	return parsedPeriod;
};
