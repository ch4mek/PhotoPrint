import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { absencesQueriesKeys } from '@/constants/queriesKeys';
import { GET_APPROVERS_USERS_LOOKUP_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { LookupUserResponse } from '@/types/userInfo';

export const useApproversUsersLookup = (
	queryOptions?: PatchedQueryOptions<
		Array<LookupUserResponse>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		Array<LookupUserResponse>,
		AxiosError<ServerResponseError>
	>({
		...queryOptions,
		queryKey: [absencesQueriesKeys.getRequestsUsersLookup],
		queryFn: async () => {
			const { data } = await axios.get(
				`${GET_APPROVERS_USERS_LOOKUP_API_ENDPOINT}`
			);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: false,
	});
