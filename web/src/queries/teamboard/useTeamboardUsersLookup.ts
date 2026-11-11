import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_TEAMBOARD_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { LookupUserResponse } from '@/types/userInfo';

export const useTeamboardUsersLookup = (
	queryOptions?: PatchedQueryOptions<
		Array<LookupUserResponse>,
		AxiosError<ServerResponseError>
	>,
	includeInactive?: boolean
) => {
	const customParams = new URLSearchParams();

	if (includeInactive !== undefined) {
		customParams.append('includeInactive', includeInactive.toString());
	}

	return useQueryWithCallback<
		Array<LookupUserResponse>,
		AxiosError<ServerResponseError>
	>({
		...queryOptions,
		queryKey: [TeamboardQueriesKeys.teamboardUsersLookup],
		queryFn: async () => {
			const { data } = await axios.get(
				`${GET_TEAMBOARD_API_ENDPOINT}/lookup?${customParams.toString()}`
			);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: false,
	});
};
