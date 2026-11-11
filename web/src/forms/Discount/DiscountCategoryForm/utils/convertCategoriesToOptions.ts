import { DiscountTag } from '@/types/discounts';

export const convertCategoriesToOptions = (data?: DiscountTag[]): string[] => {
	if (!data) {
		return [];
	}

	return data.map(({ name }) => name);
};
