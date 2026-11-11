import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';
import { convertDateFromToValueToQueryParam } from '../utils/convertFromToValueToQueryParam';

import { feedbackAutoselectOptions } from '@/constants/feedback';
import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_CONVERSATIONS_ENDPOINT } from '@/constants/queryPaths';
import { replaceSpecialCharacters } from '@/helpers/replaceSpecialCharacters';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { typeOptions } from '@/pages/TeamBoardUser/components/ConversationTable/constants';
import { convertFilterArrayToQueryParam } from '@/queries/utils';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserConversation } from '@/types/teamboard';

import { ConversationQueryFilter, GetConversationQueryParams } from './types';

export const useUserConversations = (
	params: GetConversationQueryParams,
	options?: PatchedQueryOptions<
		ByPageResponse<UserConversation>,
		AxiosError<ServerResponseError>,
		ByPageResponse<UserConversation>
	>
) =>
	useQueryWithCallback({
		queryKey: [TeamboardQueriesKeys.getUserConversation, params],
		queryFn: async () => {
			const { filters, top, skip, userId, orderBy } = params;
			const fromToPropName = 'date';

			try {
				const queryFilters = filters ?? ({} as ConversationQueryFilter);
				const {
					id,
					agreements,
					date,
					conversationResult,
					conversationType,
					employeeTheses,
					supervisor,
					theses,
				} = queryFilters;
				const query = buildQuery({
					top,
					skip,
					filter: {
						and: [
							id && `id eq ${id}`,
							userId && `userId eq ${userId}`,
							agreements &&
								`contains(tolower(agreements), tolower('${replaceSpecialCharacters(
									agreements
								)}'))`,
							agreements &&
								`contains(tolower(agreements), tolower('${replaceSpecialCharacters(
									agreements
								)}'))`,
							conversationResult && {
								conversationResult: convertFilterArrayToQueryParam(
									conversationResult,
									feedbackAutoselectOptions.length
								),
							},
							conversationType && {
								conversationType: convertFilterArrayToQueryParam(
									conversationType,
									typeOptions.length
								),
							},
							employeeTheses &&
								`contains(tolower(employeeTheses), tolower('${replaceSpecialCharacters(
									employeeTheses
								)}'))`,
							theses &&
								`contains(tolower(theses), tolower('${replaceSpecialCharacters(
									theses
								)}'))`,
							supervisor &&
								`contains(tolower(SupervisorFullName), tolower('${replaceSpecialCharacters(
									supervisor
								)}'))`,
							...convertDateFromToValueToQueryParam(date, fromToPropName),
						],
					},
					orderBy,
				});

				const { data } = await axios.get<ByPageResponse<UserConversation>>(
					`${GET_USER_CONVERSATIONS_ENDPOINT}/${query}`
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
