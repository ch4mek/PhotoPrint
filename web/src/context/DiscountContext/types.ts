import { Dispatch, SetStateAction } from 'react';

import { DiscountCategoryOrder } from '@/types/discountCategories';
import { DiscountOrder } from '@/types/discounts';

export interface DiscountContextValues {
	activeCategoryId: null | number;
	setActiveCategoryId: Dispatch<SetStateAction<null | number>>;
	activeTagId: null | number;
	setActiveTagId: Dispatch<SetStateAction<null | number>>;
	activeCityId: null | number;
	setActiveCityId: Dispatch<SetStateAction<null | number>>;
	debouncedSearchValue: string;
	searchValue: string;
	setSearchValue: Dispatch<SetStateAction<string>>;
	isMoveModeEnable: boolean;
	setIsMoveModeEnable: Dispatch<SetStateAction<boolean>>;
	isMoveEnable: boolean;
	categoryOrderData: DiscountCategoryOrder[];
	setCategoryOrderData: Dispatch<SetStateAction<DiscountCategoryOrder[]>>;
	discountOrderData: DiscountOrder[];
	setDiscountOrderData: Dispatch<SetStateAction<DiscountOrder[]>>;
}
