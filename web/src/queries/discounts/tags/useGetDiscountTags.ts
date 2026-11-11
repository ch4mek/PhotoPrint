import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { GET_DISCOUNTS_TAGS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountTag } from '@/types/discounts';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDiscountTags = (
	options?: PatchedQueryOptions<DiscountTag[], AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountsTagsData],
		queryFn: () =>
			axios
				.get<DiscountTag[]>(GET_DISCOUNTS_TAGS_API_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
