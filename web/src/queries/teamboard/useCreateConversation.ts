import {
	useMutation,
	useQueryClient,
	UseMutationOptions,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_CONVERSATIONS_ENDPOINT } from '@/constants/queryPaths';
import {
	CreateUserConversation,
	UpdateUserConversation,
} from '@/forms/TeamboardForm/Conversation/types';

export const useCreateConverstaion = (
	options?: UseMutationOptions<
		UpdateUserConversation,
		AxiosError<ServerResponseError>,
		CreateUserConversation
	>
) => {
	const queryClient = useQueryClient();

	return useMutation<
		UpdateUserConversation,
		AxiosError<ServerResponseError>,
		CreateUserConversation
	>({
		...options,
		onSuccess: (...args) => {
			queryClient.invalidateQueries({
				queryKey: [TeamboardQueriesKeys.getUserConversation],
			});

			return options?.onSuccess?.(...args);
		},
		mutationFn: (item) =>
			axios
				.post(GET_USER_CONVERSATIONS_ENDPOINT, item)
				.then((res) => res.data)
				.catch((error) => {
					throw error;
				}),
	});
};
