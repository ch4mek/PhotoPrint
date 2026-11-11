import styled from '@emotion/styled';
import { Scrollbar } from '@mms/mms-ui-library/components/Scrollbar';

import { zIndexes } from '@/constants/zIndexes';
import { WIKI_PAGE_CLASS_NAME } from '@/pages/constants';

import { closedDrawerWidth } from './config';
import {
	HEADER_HEIGHT,
	SCROLL_BOTTOM_MARGIN,
	TOP_POSITION_LOAD_WRAPPER,
} from './constants';
import { MainContentProps } from './types';

export const AppWrapper = styled('div')(({ theme }) => ({
	height: '100%',
	paddingLeft: closedDrawerWidth,
	paddingTop: HEADER_HEIGHT,
	backgroundColor: theme.palette['background-base-1'],

	[`&:has(.${WIKI_PAGE_CLASS_NAME})`]: {
		paddingLeft: 0,
	},

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		paddingLeft: 0,
	},
}));

export const MainContent = styled('main')<MainContentProps>(
	({ theme, withPaddings }) => ({
		padding: withPaddings
			? `0px ${theme.spaces.m}px ${theme.spaces.m}px ${theme.spaces.m}px`
			: 0,
		position: 'relative',
		flexGrow: 1,
		height: '100%',

		[`&:has(.${WIKI_PAGE_CLASS_NAME})`]: {
			padding: 0,
		},
	})
);

export const CustomScroll = styled(Scrollbar)({
	width: '100%',
	height: '100%',
	scrollMarginBottom: SCROLL_BOTTOM_MARGIN,
	overflow: 'auto',
});

export const LoaderWrapper = styled('div')<{ topPosition?: number }>(
	({ topPosition = TOP_POSITION_LOAD_WRAPPER }) => ({
		position: 'absolute',
		width: '100%',
		left: 0,
		zIndex: zIndexes.linearLoader,
		top: topPosition,
	})
);
