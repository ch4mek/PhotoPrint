import axios, { AxiosError } from 'axios';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { getMerchKitByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { MerchKits } from '@/pages/Merch/components/KitsTab/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchKitById = (
	kitId: number | string,
	options?: PatchedQueryOptions<MerchKits, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [merchQueriesKeys.getMerchKitByIdData, kitId],
		queryFn: () =>
			axios
				.get<MerchKits>(getMerchKitByIdEndpoint(kitId))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
