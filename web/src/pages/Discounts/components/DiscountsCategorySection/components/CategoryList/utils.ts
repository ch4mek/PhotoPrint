import {
	DiscountCategory,
	DiscountCategoryOrder,
} from '@/types/discountCategories';

export const getCategoryListFromOrderData =
	(
		data: Array<DiscountCategory> | undefined,
		orderData: Array<DiscountCategoryOrder>
	) =>
	() =>
		orderData.length && data
			? orderData.map(
					({ discountCategoryId }) =>
						data.find(({ id }) => id === discountCategoryId)!
			  )
			: data ?? [];
