import type { Identifier, XYCoord } from 'dnd-core';

interface DraggedDiscountItem {
	id: string;
	index: number;
	width: number;
	height: number;
}

export interface DiscountItemDragPreviewProps {
	left: number;
	top: number;
	width: number;
	height: number;
}

export interface DiscountDragLayerCollectedProps {
	isDragging: boolean;
	clientOffset: XYCoord | null;
	initialClientOffset: XYCoord | null;
	initialSourceClientOffset: XYCoord | null;
	item: DraggedDiscountItem;
	itemType: Identifier | null;
}
