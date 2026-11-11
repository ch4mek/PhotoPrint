import { Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

const getTextColor = (theme: Theme, isPrimary: boolean) =>
	isPrimary ? theme.palette['text-accent'] : theme.palette['text-black'];

export const CellWrapper = styled.div(({ theme }) => ({
	height: '100%',
	width: '100%',
	textOverflow: 'ellipsis',
	overflow: 'hidden',
	display: 'flex',
	justifyContent: 'space-between',
	alignItems: 'center',
	gap: `${theme.spaces.s}px`,
}));

export const ButtonsWrapper = styled('div')(({ theme }) => ({
	display: 'flex',
	gap: theme.spaces.xs,
	alignItems: 'center',

	'& > button': {
		height: SMALL_ICON_SIZE,
		width: SMALL_ICON_SIZE,
	},
}));

export const CellTypography = styled(Typography)<{ isPrimary: boolean }>(
	({ isPrimary, theme }) => ({
		color: getTextColor(theme, isPrimary),
		overflow: 'hidden',
		textOverflow: 'ellipsis',
		lineHeight: `${DEFAULT_GRID_ROW_HEIGHT}px`,
	})
);
