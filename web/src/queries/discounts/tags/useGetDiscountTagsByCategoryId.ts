import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { getDiscountTagsByCategoryIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountTag } from '@/types/discounts';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDiscountTagsByCategoryId = (
	categoryId?: number | string,
	options?: PatchedQueryOptions<DiscountTag[], AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [
			discountQueriesKeys.getDiscountsTagsByCategoryIdData,
			categoryId,
		],
		queryFn: () =>
			axios
				.get<DiscountTag[]>(getDiscountTagsByCategoryIdEndpoint(categoryId!))
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
