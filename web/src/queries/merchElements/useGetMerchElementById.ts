import axios, { AxiosError } from 'axios';

import { MerchElementsItemEntity } from '../../pages/Merch/components/ElementsTab/types';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { getMerchElementByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMerchElementById = (
	merchId?: number | string,
	options?: PatchedQueryOptions<MerchElementsItemEntity, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [merchQueriesKeys.getMerchElementByIdData, merchId],
		queryFn: () =>
			axios
				.get<MerchElementsItemEntity>(getMerchElementByIdEndpoint(merchId!))
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
