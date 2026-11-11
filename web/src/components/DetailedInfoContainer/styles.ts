import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';

import { scrollbarWidths } from '@/constants/commonStyles';

import { DETAILED_INFO_RIGHT_PADDING, INFO_SCROLL_HEIGHTS } from './constants';

export const DetailedInfoList = styled('ul')(({ theme }) => ({
	margin: 0,
	padding: 0,
	listStyle: 'none',

	display: 'grid',
	alignItems: 'start',
	gridTemplateColumns: 'repeat(2, 1fr)',
	gridTemplateRows: 'auto',
	gap: theme.spaces.s,

	[theme.breakpoints.down(theme.breakpoints.tablet)]: {
		gridTemplateColumns: 'repeat(1, 1fr)',
	},
	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		display: 'flex',
		paddingLeft: theme.spaces.m,
	},
}));

export const DetailedInfoScrollWrapper = styled('div')<{
	scroll: 'horizontal' | 'vertical';
}>(({ theme, scroll }) => ({
	padding: 0,
	paddingRight: DETAILED_INFO_RIGHT_PADDING,
	height: INFO_SCROLL_HEIGHTS.desktop,
	scrollbarGutter: 'stable',
	color: theme.palette['text-black'],
	...getScrollbarStyles(theme, scrollbarWidths.s, scroll),

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		flexGrow: 1,
		height: INFO_SCROLL_HEIGHTS.desktopS,
		padding: `0 ${DETAILED_INFO_RIGHT_PADDING}px ${theme.spaces.m}px 0`,
	},
	[theme.breakpoints.down(theme.breakpoints.tabletXS)]: {
		flexGrow: 1,
		height: INFO_SCROLL_HEIGHTS.tablet,
	},

	[theme.breakpoints.up(theme.breakpoints.desktopXS)]: {
		display: 'none',
	},
}));
