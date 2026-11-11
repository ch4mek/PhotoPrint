import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { popUpBreakpoints } from '@/constants/popUp';

import { RATE_HISTORY_PROJECT_COLOR_CIRCLE_SIZE } from './constants';

export const RateHistoryPopupContent = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.m,
	width: '100%',
	minWidth: 0,
}));

export const RateHistoryInfoRow = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'row',
	gap: theme.spaces.l,
	width: '100%',
	minWidth: 0,

	[theme.breakpoints.down(popUpBreakpoints.mobileL)]: {
		flexDirection: 'column',
		gap: theme.spaces.s,
	},
}));

export const RateHistoryInfoItem = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	flex: 1,
	minWidth: 0,
	maxWidth: '100%',
	overflow: 'hidden',
	gap: theme.spaces.xs,
}));

export const RateHistoryInfoLabel = styled(Typography)(({ theme }) => ({
	flexShrink: 0,
	color: theme.palette['text-gray'],
	...theme.typography.body['s-400'],
}));

export const RateHistoryInfoValueWrapper = styled('div')({
	flex: 1,
	minWidth: 0,
	maxWidth: '100%',
	overflow: 'hidden',
});

export const RateHistoryInfoValue = styled('span')(({ theme }) => ({
	display: 'block',
	width: '100%',
	maxWidth: '100%',
	overflow: 'hidden',
	textOverflow: 'ellipsis',
	whiteSpace: 'nowrap',
	...theme.typography.body['m-400'],
}));

export const RateHistoryProjectValue = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	flex: 1,
	minWidth: 0,
	maxWidth: '100%',
	overflow: 'hidden',
	gap: theme.spaces.xs,
}));

export const RateHistoryProjectColorCircle = styled('div')<{ color: string }>(
	({ color }) => ({
		flexShrink: 0,
		width: RATE_HISTORY_PROJECT_COLOR_CIRCLE_SIZE,
		height: RATE_HISTORY_PROJECT_COLOR_CIRCLE_SIZE,
		minWidth: RATE_HISTORY_PROJECT_COLOR_CIRCLE_SIZE,
		minHeight: RATE_HISTORY_PROJECT_COLOR_CIRCLE_SIZE,
		background: color,
		borderRadius: BorderRadiuses.xxl,
	})
);
