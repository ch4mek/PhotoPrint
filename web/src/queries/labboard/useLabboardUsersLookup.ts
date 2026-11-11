import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { LabboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_LABBOARD_USERS_LOOKUP_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { LabboardLookupUser } from '@/types/labboard';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useLabboardUsersLookup = (
	queryOptions?: PatchedQueryOptions<
		Array<LabboardLookupUser>,
		AxiosError<ServerResponseError>
	>,
	includeInactive?: boolean
) =>
	useQueryWithCallback<
		Array<LabboardLookupUser>,
		AxiosError<ServerResponseError>
	>({
		...queryOptions,
		queryKey: [LabboardQueriesKeys.labboardUsersLookup, includeInactive],
		queryFn: async () => {
			const { data } = await axios.get<Array<LabboardLookupUser>>(
				GET_LABBOARD_USERS_LOOKUP_API_ENDPOINT,
				{
					params: { includeInactive },
				}
			);

			return data;
		},
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		retry: false,
	});
