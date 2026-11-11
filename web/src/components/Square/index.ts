import styled from '@emotion/styled';

import { COLOR_MAP } from '@/constants/booking';
import { BorderRadiuses } from '@/constants/borderRadiuses';

import { SquareProps } from './types';

const SQUARE_SIDE = 21;
const SQUARE_COLOR_OPACITY = 0.8;

export const Square = styled('div')<SquareProps>(
	({ theme, type, color, side, background }) => ({
		position: 'relative',
		width: side || SQUARE_SIDE,
		height: side || SQUARE_SIDE,
		borderRadius: BorderRadiuses.xs,
		cursor: 'pointer',
		backgroundColor:
			color ||
			theme.palette[
				COLOR_MAP[type as keyof typeof COLOR_MAP] as keyof typeof theme.palette
			],
		opacity: SQUARE_COLOR_OPACITY,

		...(background && {
			backgroundColor: 'transparent',
			background,
		}),
	})
);
