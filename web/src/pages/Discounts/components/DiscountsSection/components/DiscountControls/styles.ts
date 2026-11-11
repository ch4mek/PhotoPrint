import styled from '@emotion/styled';
import { CSSProperties } from 'react';

import { MOBILE_DISCOUNT_BREAKPOINT } from '@/pages/Discounts/constants';

const SEARCH_WIDTH = 274;
const SEARCH_SMALL_WIDTH = 232;

export const DiscountControlsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	width: '100%',

	gap: theme.spaces.m,
	padding: theme.spaces.m,
}));

export const SearchInputWrapper = styled('div')(({ theme }) => ({
	width: SEARCH_WIDTH,

	[theme.breakpoints.down(theme.breakpoints.desktopL)]: {
		width: SEARCH_SMALL_WIDTH,
	},

	[theme.breakpoints.down(MOBILE_DISCOUNT_BREAKPOINT)]: {
		flex: 1,
	},
}));

export const ControlRow = styled('div')<{ reverseWrap?: boolean }>(
	({ theme, reverseWrap }) => ({
		display: 'flex',
		justifyContent: 'space-between',
		gap: theme.spaces.s,
		width: '100%',

		flexWrap: reverseWrap ? 'wrap-reverse' : 'wrap',
	})
);

export const getScrollableControlStyles = () =>
	({
		maxWidth: '100%',

		overflowX: 'auto',
		scrollbarWidth: 'none',

		msOverflowStyle: 'none',
		'::-webkit-scrollbar': {
			display: 'none',
		},
	} as CSSProperties);
