import axios, { AxiosError } from 'axios';

import { jiraImportQueriesKeys } from '@/constants/queriesKeys';
import { JIRA_IMPORT_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { JiraImportProjectsResponse } from '@/types/jiraImport';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetJiraImportProjects = (
	options?: PatchedQueryOptions<
		JiraImportProjectsResponse,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		JiraImportProjectsResponse,
		AxiosError<ServerResponseError>
	>({
		queryKey: [jiraImportQueriesKeys.projects],
		queryFn: async () => {
			try {
				const response = await axios.get<JiraImportProjectsResponse>(
					JIRA_IMPORT_PROJECTS_API_ENDPOINT
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		refetchOnWindowFocus: false,
		...options,
	});
