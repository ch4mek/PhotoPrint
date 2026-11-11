import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { MerchElementsLookupProps } from '../types';

import { merchQueriesKeys } from '@/constants/queriesKeys';
import { GET_MERCH_ELEMENTS_LOOKUP_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { MerchElementsLookupResponse } from '@/pages/Merch/components/StorageTab/types';

export const useMerchElementsLookup = (props?: MerchElementsLookupProps) => {
	const { enabled } = props || {};

	return useQueryWithCallback<Array<MerchElementsLookupResponse>, AxiosError>({
		queryKey: [merchQueriesKeys.merchElementsLookup],
		queryFn: () =>
			axios
				.get<Array<MerchElementsLookupResponse>>(
					`${GET_MERCH_ELEMENTS_LOOKUP_ENDPOINT}`
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled,
	});
};
