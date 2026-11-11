import { useTheme } from '@emotion/react';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { RenderCellProps } from 'react-data-grid';

import { getTotalKPIColor } from '@/pages/Payload/utils/getTotalKPIColor';
import { ManagementGridRow } from '@/pages/ProfitAndLoss/components/ManagementTab/types';

import { KpiCellContent } from './styles';

export function KpiCellRenderer({ row }: RenderCellProps<ManagementGridRow>) {
	const theme = useTheme();
	const { kpi, kpiColor, disabled, isParent } = row;

	if (!isParent) {
		return null;
	}

	if (kpi === null) {
		return (
			<KpiCellContent {...{ disabled }}>
				<Typography variant="m-400">-</Typography>
			</KpiCellContent>
		);
	}

	const color = kpiColor ? getTotalKPIColor(kpiColor, theme) : undefined;

	return (
		<KpiCellContent {...{ disabled }}>
			<Typography variant="m-400" {...(color && { color })}>
				{kpi}
			</Typography>
		</KpiCellContent>
	);
}
