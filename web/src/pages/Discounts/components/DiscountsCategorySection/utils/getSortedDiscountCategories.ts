import { DiscountCategory } from '@/types/discountCategories';

export const getSortedDiscountCategories = (
	categories?: DiscountCategory[]
) => {
	if (!categories) {
		return [];
	}

	return categories.sort(
		(firstCategory, secondCategory) =>
			firstCategory.orderWeight - secondCategory.orderWeight
	);
};
