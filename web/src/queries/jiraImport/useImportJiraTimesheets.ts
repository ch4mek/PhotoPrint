import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { JIRA_IMPORT_TIMESHEETS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { JiraImportTimesheetsPayload } from '@/types/jiraImport';

export const useImportJiraTimesheets = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError>,
		JiraImportTimesheetsPayload
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post<void>(
					JIRA_IMPORT_TIMESHEETS_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
