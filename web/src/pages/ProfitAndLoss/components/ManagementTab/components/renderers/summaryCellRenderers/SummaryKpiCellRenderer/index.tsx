import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderSummaryCellProps } from 'react-data-grid';

import { SummaryCellContent } from '../styles';

import { getTotalKPIColor } from '@/pages/Payload/utils/getTotalKPIColor';
import { managementGridColumns } from '@/pages/ProfitAndLoss/components/ManagementTab/constants';
import { ManagementGridRow } from '@/pages/ProfitAndLoss/components/ManagementTab/types';
import { checkIsManagementSummaryRow } from '@/pages/ProfitAndLoss/components/ManagementTab/utils';

export function SummaryKpiCellRenderer({
	row,
	column,
}: RenderSummaryCellProps<unknown, ManagementGridRow>) {
	const theme = useTheme();

	if (
		column.key !== managementGridColumns.kpi.key ||
		!checkIsManagementSummaryRow(row)
	) {
		return null;
	}

	const { kpi, kpiColor } = row;

	if (kpi === null) {
		return (
			<SummaryCellContent>
				<Typography variant="xs-400">-</Typography>
			</SummaryCellContent>
		);
	}

	const color = kpiColor ? getTotalKPIColor(kpiColor, theme) : undefined;

	return (
		<SummaryCellContent>
			<Typography variant="xs-400" {...(color && { color })}>
				{kpi}
			</Typography>
		</SummaryCellContent>
	);
}
