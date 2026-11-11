import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { BarChart } from 'recharts';

const OUTER_BUG_FIX_BOX_PADDING_BOTTOM = 224;

export const BarChartCustom = styled(BarChart)(({ theme }) => ({
	margin: 0,
	'.recharts-cartesian-axis-line': {
		display: 'none',
	},
	'.recharts-cartesian-axis-tick-line': {
		display: 'none',
	},
	'.recharts-layer.recharts-cartesian-axis > g > g > text': {
		lineHeight: theme.typography.sizes[1],
		fontSize: theme.typography.pxToRem(10),
		fontWeight: 500,
	},
	'.recharts-tooltip-label': {
		color: theme.palette.primary.main,
	},
}));

export const OuterBugFixBox = styled('div')({
	position: 'relative',
	width: '100%',
	paddingBottom: OUTER_BUG_FIX_BOX_PADDING_BOTTOM,
});

export const InnerBugFixBox = styled('div')(
	() => css`
		position: absolute;
		left: 0;
		right: 0;
		bottom: 0;
		top: 0;
	`
);
