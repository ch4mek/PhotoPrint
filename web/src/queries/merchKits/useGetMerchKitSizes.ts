import axios, { AxiosError } from 'axios';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { getMerchKitSizesEndpoint } from '@/constants/queryPaths';
import { MerchKitStorageCounts } from '@/forms/MerchKitsTransactionForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchKitSizes = (
	kitId: number | string,
	warehouseId: number | string,
	options?: PatchedQueryOptions<MerchKitStorageCounts, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [merchQueriesKeys.getMerchKitSizes, kitId, warehouseId],
		queryFn: () =>
			axios
				.get<MerchKitStorageCounts>(
					getMerchKitSizesEndpoint(kitId, warehouseId)
				)
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
