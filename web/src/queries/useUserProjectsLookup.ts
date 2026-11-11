import axios, { AxiosError } from 'axios';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_PROJECTS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { UserProjectLookup } from '@/types/userProjectLookup';

import { ServerResponseError } from './types';

export const useUserProjectsLookup = (
	userId: string | number | undefined,
	options?: PatchedQueryOptions<
		Array<UserProjectLookup>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		Array<UserProjectLookup>,
		AxiosError<ServerResponseError>
	>({
		queryKey: [TeamboardQueriesKeys.userAssignmentsLookup, `${userId}`],
		queryFn: () =>
			axios
				.get<Array<UserProjectLookup>>(`${GET_USER_PROJECTS_ENDPOINT}/lookup`, {
					params: {
						userId,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		...options,
	});
