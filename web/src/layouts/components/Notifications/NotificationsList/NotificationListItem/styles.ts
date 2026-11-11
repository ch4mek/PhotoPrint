import { css } from '@emotion/react';
import styled from '@emotion/styled';

import { NOTIFICATION_OPACITY } from '../constants';

import { DEFAULT_OPACITY } from '@/constants/opacities';

export const NotificationListItemWrapper = styled('div')(({ theme }) => ({
	paddingBottom: theme.spaces.m,
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
	display: 'flex',
	alignItems: 'flex-start',
	justifyContent: 'space-between',
	gap: theme.spaces.xs,
}));

export const NotificationListItemContentWrapper = styled('div')<{
	isRead: boolean;
}>(
	({ isRead, theme }) => css`
		color: ${theme.palette['text-black']};
		opacity: ${isRead ? NOTIFICATION_OPACITY : DEFAULT_OPACITY};
		max-width: calc(100% - ${theme.spaces.l + theme.spaces.xs}px);
	`
);
