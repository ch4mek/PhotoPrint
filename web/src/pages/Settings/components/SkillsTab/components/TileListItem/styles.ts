import { css, Theme } from '@emotion/react';
import styled from '@emotion/styled';
import { Typography } from '@mms/mms-ui-library/components/Typography';

import { BLACK, ThemeMode, WHITE } from '../../../../../../theme';
import { TILE_LIST_ITEM_HEIGHT } from '../../constants';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { EXTRA_SMALL_OPACITY } from '@/constants/opacities';
import { createGradient } from '@/helpers/createGradient';

const getTileListBackgroundColor = (
	isSelected: boolean,
	theme: Theme
): string => {
	if (isSelected) {
		return createGradient(
			theme.palette['background-primary-1'],
			theme.mode === ThemeMode.LIGHT ? BLACK : WHITE,
			EXTRA_SMALL_OPACITY
		);
	}

	return theme.palette['background-white'];
};

export const TileListItemWrapper = styled('div')<{
	isSelected: boolean;
	top: number;
	sameSidesPaddings: boolean;
	isPointerCursor: boolean;
}>(
	({ theme, isSelected, top, sameSidesPaddings, isPointerCursor }) => css`
		position: absolute;
		width: 100%;
		top: ${top}px;
		display: flex;
		align-items: center;
		padding: 0 ${sameSidesPaddings ? theme.spaces.m : theme.spaces.xs}px 0
			${theme.spaces.m}px;
		gap: ${theme.spaces.s}px;
		border-radius: ${BorderRadiuses.m}px;
		height: ${TILE_LIST_ITEM_HEIGHT}px;
		background: ${getTileListBackgroundColor(isSelected, theme)};

		& > span {
			flex-grow: 1;
		}

		&:hover {
			cursor: ${isPointerCursor ? 'pointer' : 'default'};
			background-color: ${theme.palette['hover-base-2']};
		}
	`
);
export const TileListTypography = styled(Typography)(
	({ theme }) => css`
		text-overflow: ellipsis;
		white-space: nowrap;
		overflow: hidden;
		display: block;
		color: ${theme.palette['text-black']};
	`
);
