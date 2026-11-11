import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { getDiscountCategoryByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { DiscountCategory } from '@/types/discountCategories';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDiscountCategoryById = (
	categoryId?: number | string,
	options?: PatchedQueryOptions<DiscountCategory, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountCategoryByIdData, categoryId],
		queryFn: () =>
			axios
				.get<DiscountCategory>(getDiscountCategoryByIdEndpoint(categoryId!))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
