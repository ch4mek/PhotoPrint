import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { microfrontendsQueriesKeys } from '@/constants/queriesKeys';
import { GET_MICROFRONTENDS_CONFIG } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { MFERegistryResponse } from '@/types/microfrontends';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetMicrofrontends = (
	queryOptions?: PatchedQueryOptions<
		MFERegistryResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<MFERegistryResponse, AxiosError<ServerResponseError>>({
		...queryOptions,
		queryKey: [microfrontendsQueriesKeys.microfrontends],
		queryFn: async () => {
			const { data } = await axios.get(GET_MICROFRONTENDS_CONFIG, {
				responseType: 'text',
			});
			const jsonData = JSON.parse(data);

			return jsonData;
		},
		refetchOnWindowFocus: false,
		staleTime: Infinity,
		placeholderData: keepPreviousData,
	});
