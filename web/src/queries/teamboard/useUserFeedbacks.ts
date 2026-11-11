import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';
import { convertFilterArrayToQueryParam } from '../utils';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { feedbackAutoselectOptions } from '@/constants/feedback';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_FEEDBACKS_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserFeedback } from '@/types/teamboard';

import { FeedbackQueryFilter, GetFeedbackQueryParams } from './types';

export const useUserFeedbacks = (
	params: GetFeedbackQueryParams,
	options?: PatchedQueryOptions<
		ByPageResponse<UserFeedback>,
		AxiosError<ServerResponseError>,
		ByPageResponse<UserFeedback>
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserFeedbacks, params],
		queryFn: async () => {
			const { filters, top, skip, userId, orderBy } = params;
			try {
				const queryFilters = filters ?? ({} as FeedbackQueryFilter);
				const { actions, client, date, feedback, project, reviewer, type, id } =
					queryFilters;
				const query = buildQuery({
					top,
					skip,
					filter: {
						and: [
							id && `id eq ${id}`,
							userId && `userId eq ${userId}`,
							actions &&
								`contains(tolower(actionsTodo), tolower('${replaceSpecialCharacters(
									actions
								)}'))`,
							client &&
								`contains(tolower(clientName), tolower('${replaceSpecialCharacters(
									client
								)}'))`,
							feedback &&
								`contains(tolower(feedback), tolower('${replaceSpecialCharacters(
									feedback
								)}'))`,
							project &&
								`contains(tolower(projectName), tolower('${replaceSpecialCharacters(
									project
								)}'))`,
							reviewer &&
								`contains(tolower(reviewer), tolower('${replaceSpecialCharacters(
									reviewer
								)}'))`,
							type && {
								externalFeedbackType: convertFilterArrayToQueryParam(
									type,
									feedbackAutoselectOptions.length
								),
							},
							...convertDateFromToValueToQueryParam(
								date,
								'externalFeedbackDate'
							),
						],
					},
					orderBy,
				});

				const { data } = await axios.get<ByPageResponse<UserFeedback>>(
					`${GET_USER_FEEDBACKS_ENDPOINT}/${query}`
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		...options,
	});
