import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TimeTrackItem } from '../../../../TimesheetEngineer/types';
import {
	PRIMARY_ROW_TOTAL_CELL_CLASS_NAME,
	HOVER_ROW_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '../../../constants';

import { minutesToHours } from '@/helpers/minutesToHours';
import { DEFAULT_TOTAL_TIME } from '@/pages/TimeSheet/components/TimesheetEngineer/constants';
import { getTotalColumnHoverIndex } from '@/pages/TimeSheet/constants';
import { getTotalPeriodTrackedTime } from '@/pages/TimeSheet/utils/getTotalPeriodTrackedTime';

import { PeriodTotalTimeWrapper, CellTextWrapper } from './styles';

interface PeriodTotalTimeCellProps {
	timeMap: Map<string, TimeTrackItem | undefined>;
	periodLength: number;
}

export function PeriodTotalTimeCell({
	timeMap,
	periodLength,
}: PeriodTotalTimeCellProps) {
	const theme = useTheme();
	const [totalPeriodFactTime, totalPeriodBillableTime] =
		getTotalPeriodTrackedTime(timeMap);

	const factHours = totalPeriodFactTime
		? minutesToHours(totalPeriodFactTime)
		: DEFAULT_TOTAL_TIME;

	const billableHours = totalPeriodBillableTime
		? minutesToHours(totalPeriodBillableTime)
		: DEFAULT_TOTAL_TIME;

	return (
		<PeriodTotalTimeWrapper
			periodLength={periodLength}
			className={`${PRIMARY_ROW_TOTAL_CELL_CLASS_NAME} ${HOVER_ROW_CELL_CLASS_NAME} ${HOVER_TOTAL_CELL_CLASS_NAME}`}
			data-hovercolindex={getTotalColumnHoverIndex(periodLength)}
		>
			<CellTextWrapper>
				<Typography
					align="center"
					variant="s-400"
					color={theme.palette['base-6']}
				>
					{factHours}
				</Typography>
				<Typography
					align="center"
					variant="s-400"
					color={theme.palette['base-6']}
				>
					{billableHours}
				</Typography>
			</CellTextWrapper>
		</PeriodTotalTimeWrapper>
	);
}
