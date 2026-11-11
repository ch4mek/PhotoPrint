import styled from '@emotion/styled';

import {
	DEFAULT_DISABLED_GRID_CELL_OPACITY,
	DEFAULT_OPACITY,
} from '@/constants/opacities';

import { CellTextProps } from './types';

export const verticalEllipsisStyles = (verticalEllipsisLineCount: number) =>
	({
		display: '-webkit-box',
		WebkitLineClamp: verticalEllipsisLineCount,
		WebkitBoxOrient: 'vertical',
		textOverflow: 'ellipsis',
		overflow: 'hidden',
		height: 'auto',
		whiteSpace: 'normal',
		wordBreak: 'break-word',
	} as const);

export const CellWrapper = styled('div')<CellTextProps>(
	({
		padding,
		disabled,
		verticalEllipsisLineCount,
		maxWidth,
		height = '100%',
		width = 'auto',
	}) => ({
		textOverflow: 'ellipsis',
		padding: padding || 0,
		opacity: disabled ? DEFAULT_DISABLED_GRID_CELL_OPACITY : DEFAULT_OPACITY,
		overflow: 'hidden',
		width,
		...(verticalEllipsisLineCount &&
			verticalEllipsisStyles(verticalEllipsisLineCount)),
		maxWidth,
		height,
	})
);
