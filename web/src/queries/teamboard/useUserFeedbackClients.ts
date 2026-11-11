import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_FEEDBACKS_CLIENTS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useUserFeedbacksClients = (
	userId: number,
	options?: PatchedQueryOptions<
		string[],
		AxiosError<ServerResponseError>,
		string[]
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserFeedbacksClients, userId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<string[]>(
					`${GET_USER_FEEDBACKS_CLIENTS_ENDPOINT}`,
					{
						params: {
							userId,
						},
					}
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		refetchOnWindowFocus: false,
		...options,
	});
