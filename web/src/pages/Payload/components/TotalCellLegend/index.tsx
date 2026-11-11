import { Typography } from '@mms/mms-ui-library/components/Typography';

import { CellWrapper } from '../../styles';

import {
	FACT_PLAN_TEXT,
	TOTAL_FTE_TEXT,
	TOTAL_KPI_TEXT,
	TOTAL_TIME_PER_USER_TEXT,
	TOTAL_TIME_TEXT,
} from './constants';
import { TotalCellLegendInfoPattern, TotalCellLegendWrapper } from './styles';

export function TotalCellLegend() {
	return (
		<CellWrapper>
			<TotalCellLegendWrapper>
				<Typography variant="xs-400">
					{TOTAL_FTE_TEXT}
					<TotalCellLegendInfoPattern>
						{FACT_PLAN_TEXT}
					</TotalCellLegendInfoPattern>
				</Typography>
				<Typography variant="xs-400">
					{TOTAL_TIME_TEXT}
					<TotalCellLegendInfoPattern>
						{FACT_PLAN_TEXT}
					</TotalCellLegendInfoPattern>
				</Typography>
				<Typography variant="xs-400">{TOTAL_TIME_PER_USER_TEXT}</Typography>
				<Typography variant="s-700">{TOTAL_KPI_TEXT}</Typography>
			</TotalCellLegendWrapper>
		</CellWrapper>
	);
}
