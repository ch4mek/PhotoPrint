import axios from 'axios';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_POST_MERCH_STORAGE_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import {
	BodyFilters,
	GetMerchStorageQueryParams,
	MerchStorage,
} from '@/pages/Merch/components/StorageTab/types';
import { ByPageResponse } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchStorage = (
	params: GetMerchStorageQueryParams,
	body: BodyFilters,
	options?: PatchedQueryOptions<ByPageResponse<MerchStorage>>
) =>
	useQueryWithCallback({
		queryKey: [
			merchQueriesKeys.getMerchStorageData,
			JSON.stringify(params),
			JSON.stringify(body),
		],
		queryFn: async () => {
			const { skip, top } = params;

			const { data } = await axios.post<ByPageResponse<MerchStorage>>(
				`${GET_POST_MERCH_STORAGE_ENDPOINT}?top=${top}&skip=${skip}`,
				body
			);

			return data;
		},
		refetchOnWindowFocus: false,
		...options,
	});
