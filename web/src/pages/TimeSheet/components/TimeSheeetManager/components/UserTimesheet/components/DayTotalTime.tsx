import { useTheme } from '@emotion/react';
import React from 'react';

import { DEFAULT_TOTAL_TIME } from '../../../../TimesheetEngineer/constants';
import { TimeTrackItem } from '../../../../TimesheetEngineer/types';
import { HOVER_ROW_CELL_CLASS_NAME } from '../../../constants';

import { minutesToHours } from '@/helpers/minutesToHours';
import { getTimesheetColumnClassname } from '@/pages/TimeSheet/utils/getTimesheetColumnClassname';

import { DayTotalTimeTypography, DayTotalTimeWrapper } from './styles';

interface DayTotalTimeProps {
	timeMap: Map<string, TimeTrackItem | undefined>;
	day: string;
	column: number;
	displayRightBorder: boolean;
	ariaColIndex: number;
}

function DayTotalTimeCellInner({
	timeMap,
	day,
	column,
	displayRightBorder,
	ariaColIndex,
}: DayTotalTimeProps) {
	const theme = useTheme();
	const trackItem = timeMap.get(day);

	const factHours = trackItem?.factTrackedInMinutes
		? minutesToHours(trackItem?.factTrackedInMinutes)
		: DEFAULT_TOTAL_TIME;

	const billableHours = trackItem?.billableTrackedInMinutes
		? minutesToHours(trackItem?.billableTrackedInMinutes)
		: DEFAULT_TOTAL_TIME;

	return (
		<DayTotalTimeWrapper
			className={`${HOVER_ROW_CELL_CLASS_NAME} ${getTimesheetColumnClassname(
				column,
				true
			)}`}
			data-hovercolindex={ariaColIndex}
			column={column}
			displayRightBorder={displayRightBorder}
		>
			<DayTotalTimeTypography
				align="center"
				trackedInMinutes={trackItem?.factTrackedInMinutes}
				variant="s-400"
				color={theme.palette['base-4']}
			>
				{factHours}
			</DayTotalTimeTypography>
			<DayTotalTimeTypography
				align="center"
				trackedInMinutes={trackItem?.billableTrackedInMinutes ?? 0}
				variant="s-400"
				color={theme.palette['base-4']}
			>
				{billableHours}
			</DayTotalTimeTypography>
		</DayTotalTimeWrapper>
	);
}

export const DayTotalTimeCell = React.memo(DayTotalTimeCellInner);
