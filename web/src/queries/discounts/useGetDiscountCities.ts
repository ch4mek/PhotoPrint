import { UseQueryOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { GET_DISCOUNTS_CITIES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountCity } from '@/types/discounts';

export const useGetDiscountCities = (
	options?: UseQueryOptions<DiscountCity[], AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountsCitiesData],
		queryFn: () =>
			axios
				.get<DiscountCity[]>(GET_DISCOUNTS_CITIES_API_ENDPOINT)
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
