import { css } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';

const UPCOMING_EVENT_ITEM_MAX_HEIGHT = 92;

export const UpcomingEventItemWrapper = styled('li')<{
	isDesktopView?: boolean;
}>(({ isDesktopView = true }) => ({
	width: '100%',
	minWidth: 0,
	listStyle: 'none',
	height: isDesktopView ? 'fit-content' : UPCOMING_EVENT_ITEM_MAX_HEIGHT,
	maxHeight: isDesktopView ? '100%' : UPCOMING_EVENT_ITEM_MAX_HEIGHT,
}));

export const EventItem = styled('article')<{
	isDesktopView?: boolean;
}>(({ theme, isDesktopView = true }) => ({
	display: 'flex',
	flexDirection: 'row',
	alignItems: 'flex-start',
	gap: theme.spaces.s,
	width: '100%',
	minWidth: 0,
	padding: theme.spaces.s,

	height: 'fit-content',
	maxHeight: isDesktopView ? UPCOMING_EVENT_ITEM_MAX_HEIGHT : 'none',

	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-base-1']
			: theme.palette['background-base-2'],
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	cursor: 'pointer',
	transition: 'background-color 0.2s ease-in-out',
	'&:hover': {
		backgroundColor: theme.palette['hover-base-2'],
	},
}));

export const UpcomingEventContent = styled('div')(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	color: theme.palette['text-black'],
	flex: 1,
	minWidth: 0,
}));

export const EventTitleTypography = styled(Typography)(
	() => css`
		text-overflow: ellipsis;
		white-space: nowrap;
		overflow: hidden;
		max-width: 100%;
		width: 100%;
	`
);
