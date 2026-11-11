import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderSummaryCellProps } from 'react-data-grid';

import { getTotalKPIColor } from '../utils/getTotalKPIColor';

import { usePayloadContext } from '@/context/PayloadContext';
import { isCurrentYearMonth } from '@/helpers/date';
import { TimePart } from '@/pages/Payload/components/WorkloadCell/TimePart';
import { TypeOfTimePart } from '@/pages/Payload/components/WorkloadCell/TimePart/types';
import { MonthCellWrapper, PayloadSkeletonCell } from '@/pages/Payload/styles';
import { PayloadGridRow, TotalMonth } from '@/pages/Payload/types';

export const summaryMonthCellRenderer = ({
	row,
	column,
}: RenderSummaryCellProps<PayloadGridRow, PayloadGridRow>) => {
	const theme = useTheme();

	const hoverIndex = 'hoverIndex' in column ? column.hoverIndex : undefined;
	const { key } = column;

	const info = row[key.toLowerCase() as keyof PayloadGridRow] as TotalMonth;
	const { year } = usePayloadContext();
	const {
		totalActualFte,
		totalFactBillableInHours,
		totalKpi,
		totalPlannedFte,
		totalPlannedWorkloadInHours,
		monthOfYear,
		usersCount,
		totalTimePerUsers,
		color,
	} = info;

	const isActiveMonth = isCurrentYearMonth(monthOfYear, +year);

	return (
		<MonthCellWrapper
			data-hovercolindex={hoverIndex}
			data-activemonth={isActiveMonth}
			isTotal
		>
			<PayloadSkeletonCell visible={!!row.isShowSkeleton}>
				<TimePart
					type={TypeOfTimePart.Total}
					firstTime={totalActualFte}
					secondTime={totalPlannedFte}
				/>
			</PayloadSkeletonCell>
			<PayloadSkeletonCell visible={!!row.isShowSkeleton}>
				<TimePart
					type={TypeOfTimePart.Total}
					firstTime={totalFactBillableInHours}
					secondTime={totalPlannedWorkloadInHours}
				/>
			</PayloadSkeletonCell>
			<PayloadSkeletonCell visible={!!row.isShowSkeleton}>
				<Typography variant="xs-400">
					{totalTimePerUsers ?? '-'} ({usersCount ?? '-'})
				</Typography>
			</PayloadSkeletonCell>
			<PayloadSkeletonCell visible={!!row.isShowSkeleton}>
				<Typography variant="s-700" color={getTotalKPIColor(color, theme)}>
					{totalKpi}%
				</Typography>
			</PayloadSkeletonCell>
		</MonthCellWrapper>
	);
};
