import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { ActionButton } from '@mms/mms-ui-library/components/ActionButton';

import { BorderRadiuses } from '@/constants/borderRadiuses';

import {
	NOTIFICATION_BADGE_RIGHT,
	NOTIFICATION_BADGE_SIZE,
	NOTIFICATION_BADGE_TOP,
} from './constants';

export const Badge = styled('div')<{ showBadge: boolean }>(
	({ theme, showBadge }) => css`
		border-radius: ${BorderRadiuses.half};
		height: ${NOTIFICATION_BADGE_SIZE}px;
		width: ${NOTIFICATION_BADGE_SIZE}px;
		box-sizing: content-box;
		border: 2px solid ${theme.palette['background-primary-3']};
		position: absolute;
		background: ${theme.palette['graphics-accent']};
		top: ${NOTIFICATION_BADGE_TOP}px;
		right: ${NOTIFICATION_BADGE_RIGHT}px;
		transition: transform 0.4s;
		transform: scale(${showBadge ? 1 : 0});
	`
);

export const NotificationsButtonWrapper = styled(ActionButton)({
	position: 'relative',
});
