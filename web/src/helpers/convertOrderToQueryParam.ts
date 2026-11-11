import { SortDirection } from '@/constants/sort';

import { invertSortDirection } from './invertSortDirection';

export const convertOrderToQueryParam = (
	order: SortDirection,
	orderBy: string,
	paramsToReverseOrder: string[]
) => {
	if (paramsToReverseOrder.includes(orderBy)) {
		return invertSortDirection(order);
	}

	return order;
};
