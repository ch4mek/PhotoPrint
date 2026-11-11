import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { ThemeMode } from '@mms/mms-ui-library/theme';

import { BorderRadiuses } from '@/constants/borderRadiuses';

export const AttachedFileWrapper = styled('div')<{
	maxWidth?: number;
	minWidth?: number;
	isClickable: boolean;
}>(({ theme, maxWidth, minWidth, isClickable = false }) => ({
	display: 'flex',
	alignItems: 'center',
	justifyContent: 'space-between',
	padding: theme.spaces.s,
	outline: `1px solid ${theme.palette['graphics-divider-1']}`,
	borderRadius: BorderRadiuses.m,
	flex: 1,
	transition: 'all 0.3s ease-in-out',
	gap: theme.spaces.s,
	...(!Number.isNaN(maxWidth) && {
		maxWidth,
	}),
	...(!Number.isNaN(minWidth) && {
		minWidth,
	}),
	...(isClickable && {
		cursor: 'pointer',
	}),

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

export const AttachedFileTextWrapper = styled('div')({
	display: 'inline-grid',
	flexGrow: 1,
	flexDirection: 'column',
	maxWidth: '100%',
});

export const AttachedFileText = styled(Typography)({
	maxWidth: '100%',
	overflow: 'hidden',
	whiteSpace: 'nowrap',
	textOverflow: 'ellipsis',
});
