import styled from '@emotion/styled';

import { BorderRadiuses } from '@/constants/borderRadiuses';
import { zIndexes } from '@/constants/zIndexes';

import { DiscountItemDragPreviewProps } from './types';

export const DiscountItemDragPreview = styled.img<DiscountItemDragPreviewProps>(
	({ left, top, width, height }) => ({
		position: 'fixed',
		pointerEvents: 'none',
		borderRadius: BorderRadiuses.l2,
		left,
		top,
		width,
		height,
		objectFit: 'cover',
		zIndex: zIndexes.mobileSidebar,
	})
);
