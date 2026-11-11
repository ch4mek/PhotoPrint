import axios, { AxiosError } from 'axios';

import { GroupQueriesKeys } from '@/constants/queriesKeys';
import { GET_RESOURCE_GROUPS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { DepartmentGroup } from '@/types/groupMembers';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGroupsLookup = (
	departmentId?: string | null,
	options?: PatchedQueryOptions<
		DepartmentGroup[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [GroupQueriesKeys.getGroups, departmentId],
		queryFn: () =>
			axios
				.get<DepartmentGroup[]>(GET_RESOURCE_GROUPS_API_ENDPOINT, {
					params: {
						departmentId,
					},
				})
				.then((response) => response.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
