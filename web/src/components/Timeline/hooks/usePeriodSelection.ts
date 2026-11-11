import { MouseEvent } from 'react';

import { ALL_HOURS_CELLS_AMOUNT, CELL_WIDTH } from '../constants';
import { BookedData, Section, SelectTimePeriod } from '../types';
import { calculateDuration } from '../utils/calculateDuration';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';

const HALF_HOURS_IN_CELL = 2;

export const usePeriodSelection = (
	section: Section,
	selectionPeriod: number,
	onPeriodSelect: SelectTimePeriod<number>,
	isPeriodSelectionDisabled: boolean,
	bookedData: BookedData[],
	baseHour: number
) => {
	const handleMouseDown = (e: MouseEvent<HTMLDivElement>) => {
		e.preventDefault();

		const offsetX = e.pageX - e.currentTarget.getBoundingClientRect().left;
		const clickedCellIndex = (offsetX / CELL_WIDTH) * HALF_HOURS_IN_CELL;

		const bookedItem = bookedData.find((booking) => {
			const startCellIndex =
				convertTimeToCellIndex(booking.fromDateTime, baseHour) *
				HALF_HOURS_IN_CELL;
			const duration =
				calculateDuration(booking.fromDateTime, booking.toDateTime, baseHour) *
				HALF_HOURS_IN_CELL;
			const endCellIndex = startCellIndex + duration;

			return (
				clickedCellIndex >= startCellIndex && clickedCellIndex < endCellIndex
			);
		});

		if (bookedItem) {
			const startCellIndex =
				convertTimeToCellIndex(bookedItem.fromDateTime, baseHour) *
				HALF_HOURS_IN_CELL;
			const duration =
				calculateDuration(
					bookedItem.fromDateTime,
					bookedItem.toDateTime,
					baseHour
				) * HALF_HOURS_IN_CELL;
			const endCellIndex = startCellIndex + duration;

			onPeriodSelect(section, startCellIndex, endCellIndex);
		} else {
			if (isPeriodSelectionDisabled) {
				return;
			}

			const roundedCellIndex = Math.floor(clickedCellIndex);
			onPeriodSelect(
				section,
				roundedCellIndex,
				Math.min(
					roundedCellIndex + selectionPeriod,
					ALL_HOURS_CELLS_AMOUNT * HALF_HOURS_IN_CELL
				)
			);
		}
	};

	return {
		handleMouseDown,
	};
};
