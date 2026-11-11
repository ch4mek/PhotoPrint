import {
	RefCallback,
	useCallback,
	useContext,
	useEffect,
	useRef,
	useState,
} from 'react';
import { useDrag, useDrop } from 'react-dnd';
import { getEmptyImage } from 'react-dnd-html5-backend';

import { DropIndicator } from '@/pages/Wiki/components/GroupSection/components/DraggableItem/components/DropIndicator';
import {
	DraggableItemWrapper,
	DropItemWrapper,
} from '@/pages/Wiki/components/GroupSection/components/DraggableItem/styles';
import { DraggableItemProps } from '@/pages/Wiki/components/GroupSection/components/DraggableItem/types';
import { getDropIndicatorPosition } from '@/pages/Wiki/components/GroupSection/components/DraggableItem/utils/getDropIndicatorPosition';
import { DRAGGABLE_CLASSNAME } from '@/pages/Wiki/components/GroupSection/constants';
import { DraggableContext } from '@/pages/Wiki/context/draggableContext';
import { DropIndicatorPositionType, Option } from '@/pages/Wiki/types';
import { isSiblingNode } from '@/pages/Wiki/utils/isSiblingNode';

const { None, Middle } = DropIndicatorPositionType;

export function DraggableItem({
	item,
	children,
	isDragMode,
}: DraggableItemProps) {
	const ref = useRef(null);
	const { updateTree } = useContext(DraggableContext);
	const [dropPreviewPosition, setDropPreviewPosition] =
		useState<DropIndicatorPositionType>(None);

	const [{ handlerId, draggedItem, isOverCurrent }, drop] = useDrop({
		accept: 'any',
		canDrop: (dragObject: Option) => {
			if (!isDragMode || dropPreviewPosition === None || !isOverCurrent) {
				return false;
			}

			if (dragObject.isGroup) {
				if (item.isGroup && dropPreviewPosition === Middle) {
					return false;
				}

				if (!isSiblingNode(dragObject, item)) {
					return false;
				}
			}

			return dragObject.id !== item.id;
		},
		collect(monitor) {
			return {
				handlerId: monitor.getHandlerId(),
				isOverCurrent:
					monitor.isOver({ shallow: true }) && monitor.getItem().id !== item.id,
				canDrop: monitor.canDrop(),
				draggedItem: monitor.getItem(),
			};
		},
		drop(dragObject: Option) {
			if (!ref.current || !isOverCurrent || !isDragMode) {
				return;
			}

			if (dragObject.id === item.id) {
				return;
			}
			updateTree(dragObject, item, dropPreviewPosition);
		},
		hover(dragObject: Option, monitor) {
			if (!ref.current || !isDragMode) {
				return;
			}

			if (dragObject.id === item.id) {
				return;
			}
			const isBefore = getDropIndicatorPosition(monitor, item, ref) ?? None;

			if (isBefore !== dropPreviewPosition) {
				setDropPreviewPosition(() => isBefore);
			}
		},
	});

	const [{ isDragging }, drag, preview] = useDrag({
		type: 'any',
		item,
		canDrag: isDragMode,
		collect: (monitor) => {
			const draggingItem = monitor.getItem();

			return {
				isDragging: monitor.isDragging() && item.id === draggingItem.id,
			};
		},
	});

	const dragCallbackRef = useCallback<RefCallback<HTMLDivElement>>(
		(element) => {
			drag(element);
		},
		[drag]
	);

	const showNewDropPosition = isOverCurrent && draggedItem?.id !== item.id;

	drop(ref);

	useEffect(() => {
		preview(getEmptyImage(), { captureDraggingState: true });
	}, [preview]);

	return (
		<DropItemWrapper
			ref={ref}
			data-handler-id={handlerId}
			className={DRAGGABLE_CLASSNAME}
		>
			{showNewDropPosition &&
				dropPreviewPosition !== None &&
				dropPreviewPosition !== Middle && (
					<DropIndicator position={dropPreviewPosition} />
				)}
			<DraggableItemWrapper
				ref={dragCallbackRef}
				isDragging={isDragging}
				isActiveZone={dropPreviewPosition === Middle && showNewDropPosition}
			>
				{children}
			</DraggableItemWrapper>
		</DropItemWrapper>
	);
}
