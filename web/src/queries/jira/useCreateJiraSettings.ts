import {
	useMutation,
	UseMutationOptions,
	useQueryClient,
} from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { jiraQueriesKeys } from '@/constants/queriesKeys';
import { GET_JIRA_SETTINGS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { CreateJiraSettingsParams, JiraSettingsEntity } from '@/types/jira';

export const useCreateJiraSettings = (
	options?: UseMutationOptions<
		JiraSettingsEntity,
		AxiosError<ServerResponseError>,
		CreateJiraSettingsParams
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		...restOptions,
		mutationFn: async (params) => {
			try {
				const response = await axios.post<JiraSettingsEntity>(
					GET_JIRA_SETTINGS_API_ENDPOINT,
					params
				);

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
