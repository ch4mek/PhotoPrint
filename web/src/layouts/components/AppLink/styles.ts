import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { LARGE_ICON_SIZE } from '@/constants/commonStyles';

import { APP_LINK_HEIGHT } from './constants';

export const AppLinkWrapper = styled.a(({ theme }) => ({
	display: 'flex',
	flexDirection: 'column',
	alignItems: 'center',
	justifyContent: 'space-between',
	padding: theme.spaces.s,
	height: APP_LINK_HEIGHT,
	minWidth: 0,
	width: '100%',
	borderRadius: BorderRadiuses.m,
	textDecoration: 'none',
	color: theme.palette['text-black'],

	'&:hover': {
		background: theme.palette['hover-base-2'],
	},
}));

export const LinkIconContainer = styled.div<{ color?: string }>(
	({ color }) => ({
		display: 'flex',
		alignItems: 'center',
		justifyContent: 'center',
		width: LARGE_ICON_SIZE,
		height: LARGE_ICON_SIZE,
		color,

		'& svg': {
			width: LARGE_ICON_SIZE,
			height: LARGE_ICON_SIZE,
		},
	})
);
