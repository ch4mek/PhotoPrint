import { UseQueryOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { GET_DISCOUNT_CATEGORIES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountCategory } from '@/types/discountCategories';

export const useGetDiscountCategories = (
	options?: UseQueryOptions<DiscountCategory[], AxiosError>,
	queryKey?: string
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountCategoriesData, queryKey],
		queryFn: () =>
			axios
				.get<DiscountCategory[]>(GET_DISCOUNT_CATEGORIES_API_ENDPOINT)
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
