import styled from '@emotion/styled';

const DASHBOARD_MOBILE_BREAKPOINT = 930;
const WIDGET_WIDTH = 441;

export const DashboardContentContainer = styled('div')({
	width: '100%',
	height: '100%',
});

export const DashboardContentWrapper = styled('div')(({ theme }) => ({
	display: 'grid',
	gridTemplateColumns: `repeat(4, minmax(${WIDGET_WIDTH}px, 1fr))`,
	gridGap: theme.spaces.m,
	width: '100%',
	paddingBottom: theme.spaces.m,

	[theme.breakpoints.down(theme.breakpoints.desktopL)]: {
		gridTemplateColumns: `repeat(3, 1fr)`,
	},

	[theme.breakpoints.down(theme.breakpoints.desktop)]: {
		gridTemplateColumns: `repeat(2, 1fr)`,
	},

	[theme.breakpoints.down(DASHBOARD_MOBILE_BREAKPOINT)]: {
		justifyContent: 'center',
		gridTemplateColumns: `repeat(1, 1fr)`,
	},
}));
