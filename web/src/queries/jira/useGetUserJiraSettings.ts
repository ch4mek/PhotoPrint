import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';

import { jiraQueriesKeys } from '@/constants/queriesKeys';
import { GET_JIRA_SETTINGS_SEARCH_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { JiraSettingsEntity, GetJiraSettingsQueryParams } from '@/types/jira';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetUserJiraSettings = (
	params: GetJiraSettingsQueryParams,

	options?: PatchedQueryOptions<
		ByPageResponse<JiraSettingsEntity>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [jiraQueriesKeys.getJiraSettings, params],

		queryFn: async () => {
			const { top, skip, orderBy, filters, userId } = params;

			const { url, email, projectTag, projectName } = filters ?? {};

			const query = buildQuery({
				top,
				skip,
				orderBy,
				filter: {
					and: [
						userId &&
							`cast(userId, Edm.String) eq cast('${replaceSpecialCharacters(
								String(userId)
							)}', Edm.String)`,
						url &&
							`contains(tolower(url), tolower('${replaceSpecialCharacters(
								url
							)}'))`,
						email &&
							`contains(tolower(email), tolower('${replaceSpecialCharacters(
								email
							)}'))`,
						projectTag &&
							`contains(tolower(jiraProjectTag), tolower('${replaceSpecialCharacters(
								projectTag
							)}'))`,
						projectName &&
							`contains(tolower(projectName), tolower('${replaceSpecialCharacters(
								projectName
							)}'))`,
					],
				},
			});

			try {
				const response = await axios.get<ByPageResponse<JiraSettingsEntity>>(
					`${GET_JIRA_SETTINGS_SEARCH_ENDPOINT}${query}`
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
