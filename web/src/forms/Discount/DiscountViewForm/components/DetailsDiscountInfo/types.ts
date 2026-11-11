import { RefAttributes } from 'react';

import { Discount } from '@/types/discounts';

export interface DetailsDiscountInfoProps
	extends RefAttributes<HTMLDivElement> {
	discountData?: Discount;
}
