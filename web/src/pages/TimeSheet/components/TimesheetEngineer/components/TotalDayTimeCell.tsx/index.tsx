import { RenderCellProps } from 'react-data-grid';

import { DEFAULT_TOTAL_TIME } from '../../constants';
import { TimeSheetRow } from '../../types';

import { minutesToHours } from '@/helpers/minutesToHours';

import { ColumnTotalTypography, DayTotalTimeColumnWrapper } from './styles';

export function TotalDayTimeCell(
	props: RenderCellProps<TimeSheetRow, unknown>
) {
	const {
		column: { key },
		row,
	} = props;

	const { timeMap, isDoubleTimeCell } = row;
	const trackItem = timeMap.get(key);

	const billableTotalTime = trackItem?.billableTrackedInMinutes ?? 0;

	if (!isDoubleTimeCell) {
		return (
			<DayTotalTimeColumnWrapper>
				<ColumnTotalTypography
					variant="m-400"
					trackedInMinutes={trackItem?.factTrackedInMinutes}
				>
					{minutesToHours(trackItem?.factTrackedInMinutes) ||
						DEFAULT_TOTAL_TIME}
				</ColumnTotalTypography>
			</DayTotalTimeColumnWrapper>
		);
	}

	return (
		<DayTotalTimeColumnWrapper>
			<ColumnTotalTypography
				variant="m-400"
				trackedInMinutes={trackItem?.factTrackedInMinutes}
			>
				{minutesToHours(trackItem?.factTrackedInMinutes) || DEFAULT_TOTAL_TIME}
			</ColumnTotalTypography>
			<ColumnTotalTypography
				variant="m-400"
				trackedInMinutes={billableTotalTime}
			>
				{minutesToHours(billableTotalTime) || DEFAULT_TOTAL_TIME}
			</ColumnTotalTypography>
		</DayTotalTimeColumnWrapper>
	);
}
