import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_FEEDBACKS_REVIEWERS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useUserFeedbacksReviewers = (
	userId: number,
	options?: PatchedQueryOptions<
		string[],
		AxiosError<ServerResponseError>,
		string[]
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserFeedbacksReviewers, userId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<string[]>(
					`${GET_USER_FEEDBACKS_REVIEWERS_ENDPOINT}`,
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
