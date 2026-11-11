import styled from '@emotion/styled';

import { DEFAULT_OPACITY } from '@/constants/opacities';

export const DraggableItemWrapper = styled('div')<{ isDragging: boolean }>(
	({ isDragging }) => ({
		opacity: isDragging ? 0 : DEFAULT_OPACITY,
	})
);
