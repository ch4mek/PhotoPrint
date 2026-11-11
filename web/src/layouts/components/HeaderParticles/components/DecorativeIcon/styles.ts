import styled from '@emotion/styled';

import { zIndexes } from '@/constants/zIndexes';

export const DecorativeIconWrapper = styled('img')<{
	top: number;
	left: number;
	width: number;
	height: number;
}>(({ top, left, width, height }) => ({
	position: 'absolute',
	top,
	left,
	width,
	height,
	zIndex: zIndexes.header + 1,
	userSelect: 'none',
	pointerEvents: 'none',
}));
