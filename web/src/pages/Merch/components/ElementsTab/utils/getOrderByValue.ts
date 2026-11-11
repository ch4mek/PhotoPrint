import { SortDirection } from '@/constants/sort';
import { capitalizeString } from '@/helpers/capitalizeString';

export const getOrderByValue = (
	orderByCondition: string,
	order?: SortDirection
) => {
	if (orderByCondition !== 'storage' && orderByCondition !== 'minAmount') {
		return `${capitalizeString(orderByCondition)} ${order}`;
	}
};
