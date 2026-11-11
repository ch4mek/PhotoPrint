import { PropsWithChildren, useEffect, useRef } from 'react';
import { useDrag, useDrop } from 'react-dnd';
import { getEmptyImage } from 'react-dnd-html5-backend';

import { DraggableItemWrapper } from '@/pages/Discounts/components/DraggableItem/styles';
import {
	DraggableItemProps,
	DraggableObject,
} from '@/pages/Discounts/components/DraggableItem/types';
import { DiscountSegmentTypes } from '@/types/discounts';

export function DraggableItem({
	id,
	index,
	moveCard,
	children,
	type,
	isDragMode,
}: PropsWithChildren<DraggableItemProps>) {
	const ref = useRef<HTMLDivElement>(null);
	const [{ handlerId }, drop] = useDrop({
		accept: type,
		collect(monitor) {
			return {
				handlerId: monitor.getHandlerId(),
			};
		},
		hover(item, monitor) {
			const draggedItem = item as DraggableObject;

			if (!ref.current || !isDragMode) {
				return;
			}

			const dragId = draggedItem.id;
			const hoverId = id;

			if (dragId === hoverId) {
				return;
			}

			const dragIndex = draggedItem.index;
			const hoverIndex = index;

			const hoverBoundingRect = ref.current.getBoundingClientRect();
			const clientOffset = monitor.getClientOffset();

			if (!clientOffset) {
				return;
			}

			const hoverMiddleY =
				(hoverBoundingRect.bottom - hoverBoundingRect.top) / 2;
			const hoverMiddleX =
				(hoverBoundingRect.right - hoverBoundingRect.left) / 2;

			const hoverClientY = clientOffset.y - hoverBoundingRect.top;
			const hoverClientX = clientOffset.x - hoverBoundingRect.left;

			const isVerticalMove =
				(dragIndex < hoverIndex && hoverClientY > hoverMiddleY) ||
				(dragIndex > hoverIndex && hoverClientY < hoverMiddleY);

			const isHorizontalMove =
				(dragIndex < hoverIndex && hoverClientX > hoverMiddleX) ||
				(dragIndex > hoverIndex && hoverClientX < hoverMiddleX);

			if (!isVerticalMove && type === DiscountSegmentTypes.Category) {
				return;
			}

			if (
				type === DiscountSegmentTypes.Discount &&
				!isHorizontalMove &&
				!isVerticalMove
			) {
				return;
			}

			moveCard(dragIndex, hoverIndex);
			// eslint-disable-next-line no-param-reassign
			draggedItem.index = hoverIndex;
		},
	});

	const [{ isDragging }, drag, preview] = useDrag(
		() => ({
			type,
			item: () => {
				const rect = ref.current?.getBoundingClientRect();

				return {
					id,
					index,
					width: rect?.width,
					height: rect?.height,
				};
			},
			canDrag: isDragMode,
			collect: (monitor) => ({ isDragging: monitor.isDragging() }),
		}),
		[id, index, isDragMode]
	);

	useEffect(() => {
		if (type === DiscountSegmentTypes.Discount) {
			preview(getEmptyImage(), { captureDraggingState: true });
		}
	}, [preview, type]);

	drag(drop(ref));

	return (
		<DraggableItemWrapper
			ref={ref}
			isDragging={isDragging}
			data-handler-id={handlerId}
		>
			{children}
		</DraggableItemWrapper>
	);
}
