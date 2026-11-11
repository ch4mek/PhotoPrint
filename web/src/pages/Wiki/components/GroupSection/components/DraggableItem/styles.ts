import styled from '@emotion/styled';

import { DEFAULT_OPACITY, HALF_OPACITY } from '@/constants/opacities';
import { COLLAPSE_ITEM_CLASSNAME } from '@/pages/Wiki/components/GroupSection/constants';

export const DropItemWrapper = styled('div')({
	position: 'relative',
	width: '100%',
});

export const DraggableItemWrapper = styled('div')<{
	isDragging: boolean;
	isActiveZone: boolean;
}>(({ isDragging, isActiveZone, theme }) => ({
	opacity: isDragging ? HALF_OPACITY : DEFAULT_OPACITY,

	...(isActiveZone && {
		[`& > .${COLLAPSE_ITEM_CLASSNAME}`]: {
			backgroundColor: theme.palette['hover-base-2'],
			boxShadow: `inset 0 0 0 1px ${theme.palette['graphics-scroll']}`,
		},
	}),
}));
