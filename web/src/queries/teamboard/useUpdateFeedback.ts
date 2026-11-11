import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_FEEDBACKS_ENDPOINT } from '@/constants/queryPaths';

import { UpdateUserFeedback } from './types';

export const useUpdateFeedback = (
	options?: UseMutationOptions<
		UpdateUserFeedback,
		AxiosError<ServerResponseError>,
		UpdateUserFeedback
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserFeedback,
		AxiosError<ServerResponseError>,
		UpdateUserFeedback
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserFeedbacks],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserFeedbacksReviewers],
			});
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserFeedbacksClients],
			});

			return options?.onSuccess?.(...args);
		},
		mutationFn: (params) =>
			axios
				.put(GET_USER_FEEDBACKS_ENDPOINT, {
					...params,
				})
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
};
