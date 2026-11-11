import styled from '@emotion/styled';
import { CSSProperties } from 'react';
import { XYCoord } from 'react-dnd';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';
import { addOpacityToColor } from '@/helpers/addOpacityToColor';

const PREVIEW_OPACITY = 0.9;
const SHADOW_OPACITY = 0.1;
const MAX_PREVIEW_WIDTH = 250;

export const getPreviewItemStyles = (
	initialCursorOffset: XYCoord | null,
	initialOffset: XYCoord | null,
	currentOffset: XYCoord | null
) => {
	if (!initialOffset || !currentOffset || !initialCursorOffset) {
		return {
			display: 'none',
		};
	}

	const x = initialCursorOffset.x + (currentOffset.x - initialOffset.x);
	const y = initialCursorOffset.y + (currentOffset.y - initialOffset.y);
	const transform = `translate(${x}px, ${y}px)`;

	return {
		transform,
		WebkitTransform: transform,
		width: MAX_PREVIEW_WIDTH,
	};
};

export const DraggableItemWrapper = styled('div')<{ styles: CSSProperties }>(
	({ styles, theme }) => ({
		...styles,
		backgroundColor: addOpacityToColor(
			theme.palette['background-white'],
			PREVIEW_OPACITY
		),
		border: `1px solid ${theme.palette['graphics-divider-2']}`,
		boxShadow: `0 0 10px 0 ${addOpacityToColor(
			theme.palette['background-primary-3'],
			SHADOW_OPACITY
		)}`,
		color: theme.palette['text-black'],
		borderRadius: BorderRadiuses.m,
		padding: theme.spaces.s,
		width: MAX_PREVIEW_WIDTH,
		textOverflow: 'ellipsis',
		whiteSpace: 'nowrap',
		overflow: 'hidden',
		...theme.typography.body['m-400'],
	})
);

export const DraggablePreviewItemWrapper = styled('div')({
	position: 'fixed',
	pointerEvents: 'none',
	zIndex: zIndexes.tooltip,
	left: 0,
	top: 0,
	width: '100%',
	height: '100%',
});
