import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { getFeedbackEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserFeedback } from '@/types/teamboard';

export const useUserFeedbackById = (
	feedbackId: number | undefined,
	options?: PatchedQueryOptions<
		UserFeedback,
		AxiosError<ServerResponseError>,
		UserFeedback
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserFeedbackById, feedbackId],
		queryFn: async () => {
			try {
				const { data } = await axios.get<UserFeedback>(
					`${getFeedbackEndpointUrl(feedbackId!)}`
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		refetchOnWindowFocus: false,
		enabled: Boolean(feedbackId),
		...options,
	});
