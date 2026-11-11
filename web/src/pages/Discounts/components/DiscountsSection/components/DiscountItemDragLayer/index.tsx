import { useTheme } from '@emotion/react';
import { useDragLayer } from 'react-dnd';

import dragPreviewDark from '@/assets/drag/previewDark.png';
import dragPreviewLight from '@/assets/drag/previewLight.png';
import { ThemeMode } from '@/theme';
import { DiscountSegmentTypes } from '@/types/discounts';

import { DiscountItemDragPreview } from './styles';
import { DiscountDragLayerCollectedProps } from './types';

export function DiscountItemDragLayer() {
	const theme = useTheme();

	const {
		isDragging,
		clientOffset,
		initialClientOffset,
		initialSourceClientOffset,
		item,
		itemType,
	} = useDragLayer<DiscountDragLayerCollectedProps>((monitor) => ({
		isDragging: monitor.isDragging(),
		clientOffset: monitor.getClientOffset(),
		initialClientOffset: monitor.getInitialClientOffset(),
		initialSourceClientOffset: monitor.getInitialSourceClientOffset(),
		item: monitor.getItem(),
		itemType: monitor.getItemType(),
	}));

	if (
		!isDragging ||
		!clientOffset ||
		itemType !== DiscountSegmentTypes.Discount
	) {
		return null;
	}

	const cursorToItemOffsetX =
		initialClientOffset && initialSourceClientOffset
			? initialClientOffset.x - initialSourceClientOffset.x
			: 0;
	const cursorToItemOffsetY =
		initialClientOffset && initialSourceClientOffset
			? initialClientOffset.y - initialSourceClientOffset.y
			: 0;

	const previewLeft = clientOffset.x - cursorToItemOffsetX;
	const previewTop = clientOffset.y - cursorToItemOffsetY;

	const dragPreviewImage =
		theme.mode === ThemeMode.LIGHT ? dragPreviewLight : dragPreviewDark;

	return (
		<DiscountItemDragPreview
			width={item.width}
			height={item.height}
			left={previewLeft}
			top={previewTop}
			src={dragPreviewImage}
			alt="Discount drag preview"
		/>
	);
}
