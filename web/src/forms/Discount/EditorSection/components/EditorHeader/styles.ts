import { Theme } from '@emotion/react';
import styled from '@emotion/styled';

import { discountBreakpoints } from '@/constants/discounts';

const MAX_GROUP_WIDTH = 120;
const MAX_FIRST_GROUP_WIDTH = 88;

export const getHeadingToolbarStyles = (theme: Theme) =>
	({
		minHeight: 'auto',
		padding: `${theme.spaces.m}px 0 ${theme.spaces.xs}px`,
		margin: 0,
		backgroundColor: 'transparent',
		justifyContent: 'space-between',
		flexWrap: 'wrap',
		border: 'none',

		[theme.breakpoints.down(discountBreakpoints.desktopM)]: {
			padding: 0,
		},
	} as const);

export const ToolbarButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.m,
	justifyContent: 'space-between',
	flex: 1,

	'& > div': {
		maxWidth: MAX_GROUP_WIDTH,
	},

	'& > div:first-of-type': {
		maxWidth: MAX_FIRST_GROUP_WIDTH,
	},

	[theme.breakpoints.down(discountBreakpoints.desktopM)]: {
		gap: theme.spaces.s,
		flex: 'none',
	},

	[theme.breakpoints.down(discountBreakpoints.desktopS)]: {
		gap: theme.spaces.xs,
		flex: 'none',
	},
}));
