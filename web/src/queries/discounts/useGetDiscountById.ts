import axios, { AxiosError } from 'axios';

import { discountQueriesKeys } from '@/constants/queriesKeys';
import { getDiscountByIdEndPoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { Discount } from '@/types/discounts';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetDiscountById = (
	discountId?: number | string,
	options?: PatchedQueryOptions<Discount, AxiosError<ServerResponseError>>
) =>
	useQueryWithCallback({
		queryKey: [discountQueriesKeys.getDiscountByIdData, discountId],
		queryFn: () =>
			axios
				.get<Discount>(getDiscountByIdEndPoint(discountId!))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
