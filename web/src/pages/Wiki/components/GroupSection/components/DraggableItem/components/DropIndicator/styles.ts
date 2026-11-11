import styled from '@emotion/styled';

import { topPositions } from '@/pages/Wiki/components/GroupSection/components/DraggableItem/components/DropIndicator/constants';
import { DropIndicatorPositionType } from '@/pages/Wiki/types';

export const DropIndicatorWrapper = styled('div')<{
	position: DropIndicatorPositionType;
}>(({ position, theme }) => ({
	left: 0,
	width: '100%',
	position: 'absolute',
	borderTop: `1px solid ${theme.palette['graphics-scroll']}`,
	top: topPositions[position],
}));
