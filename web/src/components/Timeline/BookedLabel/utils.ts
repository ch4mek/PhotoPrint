import { CELL_WIDTH, BOOKED_LABEL_MARGIN } from '../constants';
import { BookedDataTime } from '../types';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';

export const getXPos = (cellIndex: number) => cellIndex * CELL_WIDTH;

export const getWidth = (duration: number) =>
	duration * CELL_WIDTH - BOOKED_LABEL_MARGIN * 2;

export const getVisibleBookingRange = (
	fromDateTime: BookedDataTime,
	toDateTime: BookedDataTime,
	baseHour: number,
	cellsAmount: number,
	shouldClipToVisibleRange: boolean
) => {
	const startCellIndex = convertTimeToCellIndex(fromDateTime, baseHour);
	const endCellIndex = convertTimeToCellIndex(toDateTime, baseHour);

	if (!shouldClipToVisibleRange) {
		return {
			startCellIndex,
			duration: endCellIndex - startCellIndex,
			isClippedOnRight: false,
		};
	}

	const visibleStartCellIndex = Math.max(startCellIndex, 0);
	const visibleEndCellIndex = Math.min(endCellIndex, cellsAmount);

	return {
		startCellIndex: visibleStartCellIndex,
		duration: visibleEndCellIndex - visibleStartCellIndex,
		isClippedOnRight: endCellIndex > cellsAmount,
	};
};
