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
import { JiraSettingsEntity, UpdateJiraSettingsParams } from '@/types/jira';

export const useUpdateJiraSettings = (
	options?: UseMutationOptions<
		JiraSettingsEntity,
		AxiosError<ServerResponseError>,
		UpdateJiraSettingsParams
	>
) => {
	const queryClient = useQueryClient();
	const { onSuccess, ...restOptions } = options ?? {};

	return useMutation({
		...restOptions,
		mutationFn: async ({ id, ...params }) => {
			try {
				const response = await axios.put<JiraSettingsEntity>(
					getJiraSettingsByIdEndpoint(id),
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
