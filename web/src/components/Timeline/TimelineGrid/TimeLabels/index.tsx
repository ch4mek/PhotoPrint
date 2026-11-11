import { Typography } from '@mms/mms-ui-library/components/Typography';
import { useMemo } from 'react';

import {
	ALL_HOURS_CELLS_AMOUNT,
	ALL_HOURS_START,
	BUSINESS_HOURS_CELLS_AMOUNT,
	BUSSINESS_HOURS_START,
} from '../../constants';
import { useTimelineContext } from '../../TimelineContext';
import { generateTimeLabels } from '../../utils/generateTimeLabels';

import { TimeLabelsWrapper } from './styles';

export function TimeLabels() {
	const { isWorkingHoursOnly, cellsAmount } = useTimelineContext();

	const timeLabels = useMemo(
		() =>
			generateTimeLabels(
				isWorkingHoursOnly ? BUSSINESS_HOURS_START : ALL_HOURS_START,
				isWorkingHoursOnly
					? BUSINESS_HOURS_CELLS_AMOUNT
					: ALL_HOURS_CELLS_AMOUNT
			),
		[isWorkingHoursOnly]
	);

	return (
		<TimeLabelsWrapper cellsAmount={cellsAmount}>
			{timeLabels.map((time) => (
				<Typography key={time} variant="s-400">
					{time}
				</Typography>
			))}
		</TimeLabelsWrapper>
	);
}
