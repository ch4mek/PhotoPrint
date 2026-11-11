import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { BarChart } from 'recharts';

import { HEIGHT_OF_CHART } from '../constants';

export const BarChartCustom = styled(BarChart)(({ theme }) => ({
	margin: 0,
	'.recharts-cartesian-axis-line': {
		display: 'none',
	},
	'.recharts-cartesian-axis-tick-line': {
		display: 'none',
	},
	'.recharts-layer.recharts-cartesian-axis > g > g > text': {
		fill: theme.palette['text-gray'],
		lineHeight: theme.typography.sizes[1],
		fontSize: theme.typography.pxToRem(10),
		fontWeight: 500,
	},
	'.recharts-tooltip-label': {
		color: theme.palette['text-black'],
	},
	position: 'relative',
}));

export const ChartWrapper = styled('div')(({ theme }) => ({
	overflowY: 'hidden',
	height: `${HEIGHT_OF_CHART}px`,
	...getScrollbarStyles(theme, 3, 'horizontal'),
}));
