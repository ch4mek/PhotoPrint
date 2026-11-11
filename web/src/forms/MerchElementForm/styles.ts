import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { IconSize } from '@/constants/iconSizes';

const HINT_MAX_WIDTH = 120;
const INFO_ICON_SIZE = 14;
const MERCH_DESKTOP_BREAKPOINT = 832;

export const TwoInputsRowWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	columnGap: theme.spaces.s,
	rowGap: theme.spaces.l,
	gridTemplateColumns: `1fr 1fr`,

	[theme.breakpoints.down(MERCH_DESKTOP_BREAKPOINT)]: {
		gridTemplateColumns: `1fr`,
	},
}));

export const HintWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	maxWidth: HINT_MAX_WIDTH,
	alignItems: 'center',
}));

export const SizesSection = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
}));

export const generateCircleInfoStylesConfig = (theme: Theme) => ({
	width: IconSize.MS,
	height: IconSize.MS,
	justifyContent: 'center',

	'& > svg': {
		width: INFO_ICON_SIZE,
		height: INFO_ICON_SIZE,
		color: theme.palette['graphics-light-gray'],
	},
});
