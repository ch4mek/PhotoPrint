import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';
import { DEFAULT_GRID_ROW_HEIGHT } from '@mms/mms-ui-library/constants/grid';

import { ChevronButton } from '@/components/ChevronButton';
import { SMALL_ICON_SIZE } from '@/constants/commonStyles';

const getTextColor = (theme: Theme, isPrimary: boolean) =>
	isPrimary ? theme.palette['text-accent'] : theme.palette['text-black'];

export const CellWrapper = styled.div(
	({ theme }) => css`
		height: 100%;
		width: 100%;
		text-overflow: ellipsis;
		overflow: hidden;
		display: flex;
		justify-content: space-between;
		align-items: center;
		gap: ${theme.spaces.s}px;
	`
);

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
	({ isPrimary, theme }) => css`
		color: ${getTextColor(theme, isPrimary)};
		overflow: hidden;
		text-overflow: ellipsis;
		line-height: ${DEFAULT_GRID_ROW_HEIGHT}px;
	`
);

export const PrimarySpecializationChevronButton = styled(ChevronButton)<{
	isPrimary: boolean;
}>(({ isPrimary, theme }) => ({
	marginRight: isPrimary ? theme.spaces.l : 'none',
}));
