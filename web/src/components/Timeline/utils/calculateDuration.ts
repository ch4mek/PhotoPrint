import { BookedDataTime } from '../types';

import { convertTimeToCellIndex } from './convertTimeToCellIndex';

export function calculateDuration(
	fromDateTime: BookedDataTime,
	toDateTime: BookedDataTime,
	baseHour: number
): number {
	const fromCellIndex = convertTimeToCellIndex(fromDateTime, baseHour);
	const toCellIndex = convertTimeToCellIndex(toDateTime, baseHour);

	return toCellIndex - fromCellIndex;
}
