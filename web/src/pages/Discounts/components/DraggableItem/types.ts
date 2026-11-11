import { DiscountSegmentTypes } from '@/types/discounts';

export interface DraggableObject {
	index: number;
	id: number;
}

export interface DraggableItemProps {
	id: number | string;
	index: number;
	moveCard: (firstIndex: number, secondIndex: number) => void;
	type: DiscountSegmentTypes;
	isDragMode: boolean;
}
