import styled from '@emotion/styled';

const INFO_TOOLTIP_WIDTH = 215;
const INFO_TOOLTIP_WIDTH_MOBILE = 175;

export const InfoTooltipWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	width: `${INFO_TOOLTIP_WIDTH}px`,

	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		width: `${INFO_TOOLTIP_WIDTH_MOBILE}px`,
	},
}));

export const InfoItem = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
}));
