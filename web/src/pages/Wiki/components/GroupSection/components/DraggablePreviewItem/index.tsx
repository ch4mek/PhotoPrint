import { useDragLayer } from 'react-dnd';

import {
	DraggableItemWrapper,
	DraggablePreviewItemWrapper,
	getPreviewItemStyles,
} from '@/pages/Wiki/components/GroupSection/components/DraggablePreviewItem/styles';

export function DraggablePreviewItem() {
	const {
		item,
		isDragging,
		initialCursorOffset,
		initialFileOffset,
		currentFileOffset,
	} = useDragLayer((monitor) => ({
		item: monitor.getItem(),
		itemType: monitor.getItemType(),
		initialCursorOffset: monitor.getInitialClientOffset(),
		initialFileOffset: monitor.getInitialSourceClientOffset(),
		currentFileOffset: monitor.getSourceClientOffset(),
		isDragging: monitor.isDragging(),
	}));

	if (!isDragging) {
		return null;
	}

	return (
		<DraggablePreviewItemWrapper>
			<DraggableItemWrapper
				styles={getPreviewItemStyles(
					initialCursorOffset,
					initialFileOffset,
					currentFileOffset
				)}
			>
				{item.name}
			</DraggableItemWrapper>
		</DraggablePreviewItemWrapper>
	);
}
