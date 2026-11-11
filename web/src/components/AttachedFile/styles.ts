import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';

const ATTACHED_FILE_WRAPPER_MAX_WIDTH = 371;

export const AttachedFileWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	padding: `${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.s}px ${theme.spaces.m}px`,
	border: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	flex: 1,
	maxWidth: ATTACHED_FILE_WRAPPER_MAX_WIDTH,
	transition: 'all 0.3s ease-in-out',
	gap: theme.spaces.xs,

	'& > svg': {
		flexShrink: 0,
	},

	'&:hover': {
		backgroundColor:
			theme.mode === ThemeMode.LIGHT
				? theme.palette['background-base-1']
				: theme.palette['background-base-2'],
	},
}));

export const AttachedFileTextWrapper = styled('div')(({ theme }) => ({
	display: 'inline-grid',
	flexDirection: 'column',
	gap: theme.spaces.xs,
	maxWidth: '100%',
}));

export const AttachedFileText = styled(Typography)(({ theme }) => ({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
	color: `${theme.palette['text-black']}`,
}));
