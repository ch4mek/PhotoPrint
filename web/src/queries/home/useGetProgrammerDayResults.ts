import axios, { AxiosError } from 'axios';

import { homeQueriesKeys } from '@/constants/queriesKeys';
import { USER_SCORE_LEADERBOARD_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { ProgrammerDayResult } from './types';

export const useGetProgrammerDayResults = (
	options?: PatchedQueryOptions<
		ProgrammerDayResult[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [homeQueriesKeys.getProgrammerDayResults],
		queryFn: async () => {
			try {
				const { data } = await axios.get<ProgrammerDayResult[]>(
					USER_SCORE_LEADERBOARD_API_ENDPOINT
				);

				return data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		refetchOnWindowFocus: false,
		...options,
	});
