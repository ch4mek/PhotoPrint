import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { GET_SEARCHED_DISCOUNTS_API_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountFilterParam } from '@/queries/discounts/types';
import { Discount } from '@/types/discounts';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDiscounts = (
	filter: DiscountFilterParam,
	options?: PatchedQueryOptions<Discount[], AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountsData, JSON.stringify(filter)],
		queryFn: () => {
			const { discountCategoryId, tagId, cityId, name } = filter;
			const query = new URLSearchParams({
				...(discountCategoryId != null && {
					discountCategoryId: discountCategoryId.toString(),
				}),
				...(tagId != null && { tagId: tagId.toString() }),
				...(cityId != null && { cityId: cityId.toString() }),
				...(name && { name: replaceSpecialCharacters(name).toLowerCase() }),
			}).toString();

			return axios
				.get<Discount[]>(`${GET_SEARCHED_DISCOUNTS_API_ENDPOINT}?${query}`)
				.then((res) => res.data)
				.catch(handleRequestError);
		},
		refetchOnWindowFocus: false,
		...options,
	});
