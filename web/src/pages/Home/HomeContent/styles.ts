import styled from '@emotion/styled';

import { HOME_MOBILE_BREAKPOINT } from './constants';

const RIGHT_PADDING = 1;
const WIDGET_WIDTH = 441;

export const HomePageWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	padding: `${theme.spaces.m}px ${RIGHT_PADDING}px 0 0`,
	gap: theme.spaces.m,
	flex: 1,

	[theme.breakpoints.down(theme.breakpoints.desktopL)]: {
		paddingBottom: theme.spaces.m,
	},
}));

export const ProgrammerDaySectionWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: '100%',
}));

export const ProgrammerDayContentWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `minmax(0, 2fr) minmax(${WIDGET_WIDTH}px, 1fr)`,
	gap: theme.spaces.m,
	alignItems: 'stretch',
	width: '100%',

	[theme.breakpoints.down(theme.breakpoints.desktop)]: {
		gridTemplateColumns: '1fr',
	},
}));

export const ProgrammerDayLeaderboardWrapper = styled('div')({
	display: 'flex',
	minWidth: 0,
});

export const HomeContentWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(4, minmax(${WIDGET_WIDTH}px, 1fr))`,
	gridGap: theme.spaces.m,
	width: '100%',

	[theme.breakpoints.down(theme.breakpoints.desktopL)]: {
		gridTemplateColumns: `repeat(3, 1fr)`,
	},

	[theme.breakpoints.down(theme.breakpoints.desktop)]: {
		gridTemplateColumns: `repeat(2, 1fr)`,
	},

	[theme.breakpoints.down(HOME_MOBILE_BREAKPOINT)]: {
		justifyContent: 'center',
		gridTemplateColumns: `repeat(1, 1fr)`,
	},
}));
