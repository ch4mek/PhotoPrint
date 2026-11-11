import axios from 'axios';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_POST_MERCH_KITS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import {
	BodyFilters,
	GetMerchKitsQueryParams,
	MerchKits,
} from '@/pages/Merch/components/KitsTab/types';
import { ByPageResponse } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchKits = (
	params: GetMerchKitsQueryParams,
	body: BodyFilters,
	options?: PatchedQueryOptions<ByPageResponse<MerchKits>>
) =>
	useQueryWithCallback({
		queryKey: [merchQueriesKeys.getMerchKitsData, params, body],
		queryFn: async () => {
			const { top, skip } = params;

			const { data } = await axios.post<ByPageResponse<MerchKits>>(
				`${GET_POST_MERCH_KITS_ENDPOINT}?top=${top}&skip=${skip}`,
				body
			);

			return data;
		},
		refetchOnWindowFocus: false,
		...options,
	});
