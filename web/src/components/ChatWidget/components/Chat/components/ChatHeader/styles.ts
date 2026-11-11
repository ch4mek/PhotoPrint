import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { ThemeMode } from '@/theme';

export const ChatHeaderWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,
	alignItems: 'start',

	width: '100%',
	padding: `${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.m}px`,

	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-base-1']
			: theme.palette['background-base-2'],
	borderBottom: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: `${BorderRadiuses.m}px ${BorderRadiuses.m}px 0 0`,

	[theme.breakpoints.down(theme.breakpoints.desktopXS)]: {
		borderRadius: 0,
	},
}));
