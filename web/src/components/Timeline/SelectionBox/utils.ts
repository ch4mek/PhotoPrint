import { CELL_WIDTH, MIDNIGHT_HOURS, TimelinePeriod } from '../constants';

export const calculateSelectedRangeStyle = (from: number, to: number) => {
	const isHasFullDayDuraton = from === MIDNIGHT_HOURS && to === MIDNIGHT_HOURS;
	const posX = from * CELL_WIDTH;

	const width =
		(isHasFullDayDuraton ? TimelinePeriod.FULL_DAY : to - from) * CELL_WIDTH;

	return {
		left: posX,
		width,
	};
};
