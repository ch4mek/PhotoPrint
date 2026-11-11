import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { getDataGridResetStyles } from '@/components/Grid/styles';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { popUpBreakpoints } from '@/constants/popUp';

import { HISTORY_GRID_CONTAINER_MAX_WIDTH } from './constants';

const ACTIVE_OPACITY = 1;
const INACTIVE_OPACITY = 0.6;

export const HistoryPositionContainer = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	border: `1px solid ${theme.palette['graphics-input-stroke']}`,
	borderBottom: 0,
	borderRadius: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,
	maxWidth: HISTORY_GRID_CONTAINER_MAX_WIDTH,
	height: '100%',

	[theme.breakpoints.down(popUpBreakpoints.desktopXS)]: {
		maxWidth: '100%',
	},

	backgroundColor: theme.palette['background-white'],

	gap: theme.spaces.m,
	...getDataGridResetStyles(true),
}));

export const TabsHeaderContainer = styled('div')(({ theme }) => ({
	borderBottom: `1px solid ${theme.palette['graphics-input-stroke']}`,
}));

export const HistoryCellWrapper = styled(Typography)<{
	isDeletedHistory: boolean;
}>(({ isDeletedHistory, theme }) => ({
	...theme.typography.body['m-400'],
	opacity: isDeletedHistory ? INACTIVE_OPACITY : ACTIVE_OPACITY,
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
}));
