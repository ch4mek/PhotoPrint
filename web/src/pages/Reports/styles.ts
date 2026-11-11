import { css } from '@emotion/react';
import styled from '@emotion/styled';

import {
	COLLAPSIBLE_TABLE_GRID_TEMPLATE_COLUMNS,
	COLLAPSIBLE_TABLE_GRID_TEMPLATE_ROWS,
} from '../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { PIE_CHART_WRAPPER_HEIGHT } from '@/constants/pieChart';

import { CONTROLLERS_BREAKPOINT } from './constants';

export const PageWrapper = styled('div')(
	() => css`
		display: flex;
		flex-direction: column;
		width: 100%;
		height: 100%;
	`
);

export const HeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',

	[theme.breakpoints.up(CONTROLLERS_BREAKPOINT)]: {
		display: 'block',
		marginBottom: theme.spaces.m,
	},
}));

export const StatisticsBlockWrapper = styled('div')(({ theme }) => ({
	padding: `${theme.spaces.m}px ${theme.spaces.m}px`,
	paddingBottom: 0,
	position: 'relative',

	borderRadius: `0 0 ${BorderRadiuses.m}px ${BorderRadiuses.m}px`,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderTop: 'none',
	backgroundColor: theme.palette['background-white'],
}));

export const ReportsListContainer = styled('div')(({ theme }) => ({
	display: 'grid',
	width: '100%',
	height: '100%',
	gridTemplateColumns: COLLAPSIBLE_TABLE_GRID_TEMPLATE_COLUMNS,
	gridTemplateRows: COLLAPSIBLE_TABLE_GRID_TEMPLATE_ROWS,
	gap: theme.spaces.m,
	gridAutoRows: '100%',
	marginTop: theme.spaces.m,
	paddingBottom: theme.spaces.m,

	[theme.breakpoints.down(theme.breakpoints.desktopS)]: {
		gridTemplateColumns: '100%',
		display: 'flex',
		flexDirection: 'column',
		gap: theme.spaces.m,
	},
}));

export const Warning = styled('h3')(({ theme }) => ({
	textAlign: 'center',
	color: theme.palette['text-black'],
	...theme.typography.headlines.small,
}));

export const ReportsListWrapper = styled('div')(({ theme }) => ({
	flexGrow: '1',

	[theme.breakpoints.down(theme.breakpoints.desktopS)]: {
		minHeight: 'auto',
	},
}));

export const ActivityTableWrapper = styled('div')(({ theme }) => ({
	flex: '1 1 auto',

	[theme.breakpoints.up(theme.breakpoints.desktopXS)]: {
		minHeight: PIE_CHART_WRAPPER_HEIGHT,
	},
}));
