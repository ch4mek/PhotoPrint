import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { PieChartTooltipPosition } from './types';

const RIGHT_POSITION_TOP_PADDING = 28;
const RIGHT_POSITION_BOTTOM_PADDING = 27;
const INFO_WRAPPER_HEIGHT = 47;

const wrapperWithTooltipTopPositionStyles = (width: number) => css`
	width: ${width}px;
`;

const wrapperWithTooltipRightPositionStyles = (theme: Theme) => css`
	width: 100%;
	display: flex;
	align-content: center;
	justify-content: center;
	position: relative;
	padding-top: ${RIGHT_POSITION_TOP_PADDING}px;
	padding-bottom: ${RIGHT_POSITION_BOTTOM_PADDING}px;

	${theme.breakpoints.down(theme.breakpoints.desktopXS)} {
		padding: 0;
	}
`;

export const Wrapper = styled('div')<{
	width: number;
	tooltipPosition: PieChartTooltipPosition;
}>(({ theme, tooltipPosition, width }) =>
	tooltipPosition === 'top'
		? wrapperWithTooltipTopPositionStyles(width).styles
		: wrapperWithTooltipRightPositionStyles(theme)
);

export const PieChartWrapper = styled('div')<{ width: number; height: number }>(
	({ width, height }) => ({
		width: `${width}px`,
		height: `${height}px`,

		'& *': {
			outline: 'none',
		},
	})
);

const infoWrapperWithTooltipTopPositionStyles = (theme: Theme) => css`
	margin-bottom: ${theme.spaces.s}px;
	display: flex;
	align-items: center;
	flex-direction: column;
	row-gap: ${theme.spaces.s};
	height: ${INFO_WRAPPER_HEIGHT}px;
`;

const infoWapperWithTooltipRightPositionStyles = (
	theme: Theme,
	chartWidth: number
) => css`
	margin-bottom: ${theme.spaces.s}px;
	display: flex;
	align-items: flex-start;
	flex-direction: column;
	row-gap: ${theme.spaces.s}px;
	height: ${INFO_WRAPPER_HEIGHT}px;
	position: absolute;
	top: 50%;
	transform: translateY(-50%);
	left: calc(50% + ${Math.floor(chartWidth / 2)}px + ${theme.spaces.m}px);
	max-width: calc(
		50% - ${Math.floor(chartWidth / 2)}px - ${theme.spaces.m * 2}px
	);
`;

export const InfoWrapper = styled('div')<{
	chartWidth: number;
	tooltipPosition: PieChartTooltipPosition;
}>(({ theme, chartWidth, tooltipPosition }) =>
	tooltipPosition === 'top'
		? infoWrapperWithTooltipTopPositionStyles(theme).styles
		: infoWapperWithTooltipRightPositionStyles(theme, chartWidth).styles
);

export const Name = styled(Typography)(({ theme }) => ({
	color: theme.palette['text-black'],
	letterSpacing: '0.02em',
	textOverflow: 'ellipsis',
	textAlign: 'center',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	width: '100%',
	position: 'relative',
	zIndex: 999,
}));
