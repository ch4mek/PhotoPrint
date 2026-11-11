import { RenderSummaryCellProps } from 'react-data-grid';

import { ATTENDANCE_TOTAL_GRID_COLUMNS } from '../constants';
import { SummaryRowContainer, SummaryLabel, SummaryValue } from '../styles';
import { AttendanceTotalGridRow } from '../types';
import { isAttendanceTotalRow } from '../utils/isTotalRow';

export const summaryTimeCellRenderer = (
	props: RenderSummaryCellProps<unknown, AttendanceTotalGridRow>
) => {
	const { row, column } = props;

	if (
		column.key === ATTENDANCE_TOTAL_GRID_COLUMNS.time.key &&
		isAttendanceTotalRow(row)
	) {
		return (
			<SummaryRowContainer>
				<SummaryLabel variant="xs-400">Total</SummaryLabel>
				<SummaryValue variant="xs-400">{row.totalTime}</SummaryValue>
			</SummaryRowContainer>
		);
	}

	return null;
};
