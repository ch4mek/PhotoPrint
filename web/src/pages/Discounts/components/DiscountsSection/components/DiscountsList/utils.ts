import { Discount, DiscountOrder } from '@/types/discounts';

export const getDiscountsListFromOrderData =
	(data: Array<Discount> | undefined, orderData: Array<DiscountOrder>) => () =>
		orderData.length && data
			? orderData.map(
					({ discountId }) => data.find(({ id }) => id === discountId)!
			  )
			: data ?? [];
