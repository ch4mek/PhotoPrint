import { RenderCellProps } from 'react-data-grid';

import { MIN_WORKING_TIME_IN_MINUTES } from '../constants';
import { TimeCellWrapper } from '../styles';
import { AttendanceTotalGridRow } from '../types';
import { convertTimeToMinutes } from '../utils/timeConverter';

export const timeCellRenderer = ({
	row,
}: RenderCellProps<AttendanceTotalGridRow, unknown>) => {
	const time = row.time;
	const isMinWorkingTime =
		convertTimeToMinutes(time) >= MIN_WORKING_TIME_IN_MINUTES;

	return <TimeCellWrapper {...{ isMinWorkingTime }}>{time}</TimeCellWrapper>;
};
