import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { getScrollbarStyles } from '@mms/mms-ui-library/components/Scrollbar';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { TabableContainer } from '@/components/TabableContainer';
import { BorderRadiuses } from '@/constants/borderRadiuses';
import { scrollbarWidths } from '@/constants/commonStyles';

import { NOTIFICATION_LIST_PARENT_CLASS } from './constants';

export const LOADER_HEIGHT = 85;

export const NotificationListWrapper = styled('div')<{ maxHeight: number }>(
	({ theme, maxHeight }) => css`
		max-height: ${maxHeight}px;
		display: flex;
		flex-direction: column;
		width: 100%;
		background: ${theme.palette['background-white']};
		border-radius: ${BorderRadiuses.m}px;
		border: 1px solid ${theme.palette['graphics-input-stroke']};
		box-shadow: ${theme.namedShadows.dropDownListWrapper};
	`
);

export const NotificationsListHeaderWrapper = styled('div')(
	({ theme }) => css`
		display: flex;
		justify-content: space-between;
		padding: ${theme.spaces.m}px;
		border-bottom: 1px solid ${theme.palette['graphics-input-stroke']};
	`
);

export const NotificationsListBodySpaceContainer = styled('div')(
	({ theme }) => css`
		padding: 0 ${theme.spaces.m}px;
		display: flex;
		flex-direction: column;
		gap: ${theme.spaces.m}px;
	`
);

export const ClearAllButton = styled(TabableContainer)(
	({ theme }) => css`
		color: ${theme.palette['text-accent']};
		cursor: pointer;
	`
);

export const ScrollbarWrapper = styled('div')(({ theme }) => ({
	...getScrollbarStyles(theme, scrollbarWidths.m, 'vertical'),
	padding: `${theme.spaces.m}px 0`,
	'&::-webkit-scrollbar-track': {
		backgroundColor: 'transparent',
	},

	[`&>.${NOTIFICATION_LIST_PARENT_CLASS}>div:last-child`]: {
		borderBottomWidth: 0,
		padding: 0,
	},
}));

export const AllClearDialogWrapper = styled('div')``;

export const HeaderTypography = styled(Typography)(
	({ theme }) => css`
		color: ${theme.palette['text-black']};
	`
);

export const NavigationNotificationWrapper = styled('div')({
	cursor: 'pointer',
});

export const UserRequestNotificationWrapper = styled('div')<{ color?: string }>(
	({ theme, color }) => ({
		cursor: 'pointer',
		display: 'flex',
		alignItems: 'center',
		gap: theme.spaces.xs,
		color,
	})
);

export const PreventOverflowTypography = styled(Typography)({
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
});
