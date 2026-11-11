import styled from '@emotion/styled';

import { ThemeMode } from '@/theme';

export const CommentToolbarWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.s,

	minHeight: 'auto',
	margin: 0,
	border: 'none',
	padding: `${theme.spaces.s}px ${theme.spaces.m}px`,
	backgroundColor:
		theme.mode === ThemeMode.LIGHT
			? theme.palette['background-base-2']
			: theme.palette['background-base-3'],
}));
