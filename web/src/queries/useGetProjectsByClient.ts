import axios, { AxiosError } from 'axios';

import { ProjectsQueriesKeys } from '@/constants/queriesKeys';
import {
	GET_PROJECTS_LOOKUP,
	getClientProjectsEndpointUrl,
} from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { ProjectLookupResponse } from '@/types/project';

export const useGetProjectsByClient = (
	clientId: number | null | string,
	options?: PatchedQueryOptions<Array<ProjectLookupResponse>, AxiosError>,
	managedByPMO?: boolean
) =>
	useQueryWithCallback<Array<ProjectLookupResponse>, AxiosError>({
		...options,
		queryKey: [
			ProjectsQueriesKeys.projectsByClientLookup,
			clientId,
			managedByPMO,
		],
		queryFn: async () => {
			const onlyManagedProjects = managedByPMO ? true : undefined;

			if (!clientId) {
				const res = await axios.get<Array<ProjectLookupResponse>>(
					GET_PROJECTS_LOOKUP,
					{
						params: {
							isActive: true,
							onlyManagedProjects,
						},
					}
				);

				return res.data;
			}
			const { data } = await axios.get(getClientProjectsEndpointUrl(clientId), {
				params: {
					onlyManagedProjects,
				},
			});

			return data;
		},
		refetchOnWindowFocus: false,
	});
