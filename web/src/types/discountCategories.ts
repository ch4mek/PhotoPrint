import { DiscountTag } from '@/types/discounts';

export interface DiscountCategory {
	id: number | string;
	name: string;
	orderWeight: number;
	discountCategoryTags: DiscountTag[];
}

export interface DiscountCategoryFormValues {
	name: string;
	tagName: string;
	discountCategoryTags: string[];
}

export interface DiscountCategoryMutationData
	extends Omit<DiscountCategoryFormValues, 'tagName'> {
	orderWeight: number;
}

export interface CreateDiscountCategoryMutationData
	extends DiscountCategoryMutationData {
	id: number | string;
}

export interface DiscountCategoryOrder {
	discountCategoryId: number;
	orderWeight: number;
}

export interface DiscountCategoryOrderData {
	discountCategoryOrders: DiscountCategoryOrder[];
}
