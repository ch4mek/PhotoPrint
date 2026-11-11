import { memo, useEffect, useState } from 'react';

import { CELL_WIDTH } from '../constants';
import { useTimelineContext } from '../TimelineContext';
import { convertTimeToCellIndex } from '../utils/convertTimeToCellIndex';

import { timeUnits } from '@/constants/date';
import dayjs from '@/lib/dayjs';

import { CurrentTimeIndicatorLine } from './styles';

function CurrentTimeIndicatorInner() {
	const { baseHour, cellsAmount, timezone } = useTimelineContext();

	const getTimeObject = () => dayjs().tz(timezone.timezone).toObject();

	const [dateObjectWithTimezone, setDateObjectWithTimezone] = useState(
		getTimeObject()
	);

	const { hours, minutes } = dateObjectWithTimezone;

	const cellIndex = convertTimeToCellIndex(
		{
			hours,
			minutes,
		},
		baseHour
	);

	const IS_CURRENT_TIME_INDICATOR_VISIBLE = cellIndex < cellsAmount;

	useEffect(() => {
		if (IS_CURRENT_TIME_INDICATOR_VISIBLE) {
			const intervalId = setInterval(() => {
				const updatedDate = getTimeObject();
				setDateObjectWithTimezone(updatedDate);
			}, timeUnits.MILLISECONDS_IN_MINUTE);

			return () => clearInterval(intervalId);
		}
	}, [timezone.timezone]);

	if (!IS_CURRENT_TIME_INDICATOR_VISIBLE) {
		return null;
	}

	return <CurrentTimeIndicatorLine posX={cellIndex * CELL_WIDTH} />;
}

export const CurrentTimeIndicator = memo(CurrentTimeIndicatorInner);
