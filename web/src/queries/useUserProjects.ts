import axios, { AxiosError } from 'axios';

import { getUserPrivateProjectsEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import type { PrivateProjectsLookupResponse } from '@/types/project';

export const useUserProjects = (
	userId: number,
	params?: { isActive?: boolean; managedByPMO?: boolean },
	options?: PatchedQueryOptions<
		PrivateProjectsLookupResponse[],
		AxiosError<unknown, any>,
		PrivateProjectsLookupResponse[]
	>
) =>
	useQueryWithCallback<Array<PrivateProjectsLookupResponse>, AxiosError>({
		queryKey: ['user-projects', userId, params],
		queryFn: () => {
			if (!userId) {
				return [];
			}

			return axios
				.get<Array<PrivateProjectsLookupResponse>>(
					getUserPrivateProjectsEndpointUrl(userId),
					{
						params: {
							onlyManagedProjects: params?.managedByPMO,
							isActive: params?.isActive,
						},
					}
				)
				.then((res) => res.data);
		},
		refetchOnWindowFocus: false,
		...options,
	});
