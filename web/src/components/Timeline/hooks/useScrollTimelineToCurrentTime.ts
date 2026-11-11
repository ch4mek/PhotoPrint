import { RefObject, useEffect } from 'react';

import { BUSINESS_HOURS_CELLS_AMOUNT, CELL_WIDTH } from '../constants';
import { useTimelineContext } from '../TimelineContext';
import { SelectedTimePeriod, TimelineScroll } from '../types';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';
import { mapScrollPositionToScrollOffset } from '../utils/mapScrollPositionToScrollOffset';

import dayjs from '@/lib/dayjs';

export const useScrollTimelineToCurrentTime = (
	timelineRef: RefObject<HTMLDivElement | null>,
	selectedPeriod: SelectedTimePeriod<number> | null,
	scroll?: TimelineScroll
) => {
	const { isWorkingHoursOnly, baseHour, timezone } = useTimelineContext();

	const { hours, minutes } = dayjs().tz(timezone.timezone).toObject();

	const currentCellIndex = convertTimeToCellIndex({ hours, minutes }, baseHour);

	const IS_IN_WORKING_RANGE =
		!isWorkingHoursOnly || currentCellIndex < BUSINESS_HOURS_CELLS_AMOUNT;

	useEffect(() => {
		const SHOULD_SCROLL_TO_CURRENT_TIME =
			scroll && IS_IN_WORKING_RANGE && !selectedPeriod && timelineRef.current;

		if (SHOULD_SCROLL_TO_CURRENT_TIME && timelineRef.current) {
			const { position, behavior } = scroll;

			const timelineWidth = timelineRef.current.clientWidth;

			const scrollOffset =
				currentCellIndex * CELL_WIDTH -
				mapScrollPositionToScrollOffset(position || 'center', timelineWidth);

			timelineRef.current?.scrollTo({
				left: scrollOffset,
				behavior,
			});
		}
	}, [timelineRef.current]);
};
