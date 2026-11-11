import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { jiraQueriesKeys } from '@/constants/queriesKeys';
import { getJiraSettingsByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useDeleteJiraSettings = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError, unknown>,
		number
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		...restOptions,
		mutationFn: async (id) => {
			try {
				const response = await axios.delete(getJiraSettingsByIdEndpoint(id));

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		onSuccess: (data, variables, context) => {
			queryClient.invalidateQueries({
				queryKey: [jiraQueriesKeys.getJiraSettings],
			});

			onSuccess?.(data, variables, context);
		},
	});
};
