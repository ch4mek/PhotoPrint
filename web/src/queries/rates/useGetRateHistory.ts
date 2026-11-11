import axios, { AxiosError } from 'axios';

import { ratesQueriesKeys } from '@/constants/queriesKeys';
import { getRateHistoryApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { GetRateHistoryResponse } from '@/types/rates';

import { UseGetRateHistoryParams } from './types';

export const useGetRateHistory = (
	{ userId, projectId }: UseGetRateHistoryParams,
	options?: PatchedQueryOptions<
		GetRateHistoryResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<GetRateHistoryResponse, AxiosError<ServerResponseError>>(
		{
			queryKey: [ratesQueriesKeys.getRateHistory, userId, projectId],
			queryFn: async () => {
				try {
					const res = await axios.get<GetRateHistoryResponse>(
						getRateHistoryApiEndpoint(projectId, userId)
					);

					return res.data;
				} catch (error) {
					return handleRequestError(error);
				}
			},
			refetchOnWindowFocus: false,
			...options,
		}
	);
