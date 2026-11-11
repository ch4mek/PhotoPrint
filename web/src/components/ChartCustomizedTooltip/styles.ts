import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import {
	CHART_TOOLTIP_ARROW_OFFSET_X,
	CHART_TOOLTIP_ARROW_OFFSET_Y,
	CHART_TOOLTIP_ARROW_ROTATE_DEG,
	CHART_TOOLTIP_ARROW_SIZE,
	CHART_TOOLTIP_MAX_WIDTH,
} from './constants';
import { BaseCustomizedTooltipWrapperProps } from './types';

export const Wrapper = styled('div')<BaseCustomizedTooltipWrapperProps>(
	({ theme, arrowPlacement: tooltipPlacement }) => ({
		backgroundColor: theme.palette['graphics-gray'],
		boxShadow: theme.namedShadows.chartTooltip,
		borderWidth: 0,
		borderRadius: BorderRadiuses.m,
		padding: theme.spaces.s,
		outline: 'none',
		position: 'relative',
		maxWidth: CHART_TOOLTIP_MAX_WIDTH,

		'&:before': {
			content: '""',
			position: 'absolute',
			transform: `rotate(${CHART_TOOLTIP_ARROW_ROTATE_DEG})`,
			width: CHART_TOOLTIP_ARROW_SIZE,
			height: CHART_TOOLTIP_ARROW_SIZE,
			backgroundColor: theme.palette['graphics-gray'],
			top: CHART_TOOLTIP_ARROW_OFFSET_Y,
			[tooltipPlacement]: CHART_TOOLTIP_ARROW_OFFSET_X,
		},
	})
);

export const TextLinePeriod = styled('div')(({ theme }) => ({
	color: theme.palette['text-white'],
	marginBottom: theme.spacing(0.5),
	whiteSpace: 'wrap',
	...theme.typography.body['m-600'],
}));

export const TextLineHours = styled('div')(({ theme }) => ({
	color: theme.palette['text-white'],
	...theme.typography.body['s-400'],
}));
