import { Discount } from '@/types/discounts';

export interface DiscountCardProps {
	item: Discount;
	isMoveMode: boolean;
	handleDeleteClick: (id: string | number, title: string) => void;
	handleEditClick: (id: string | number) => void;
	handleItemClick: (id: string | number) => void;
}
