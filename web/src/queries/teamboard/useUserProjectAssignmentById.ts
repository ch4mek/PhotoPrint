import axios from 'axios';

import { UserProjectAssignmentsByIdResponse } from '../types';

import { TeamboardQueriesKeys } from '@/constants/queriesKeys';
import { GET_USER_PROJECTS_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';

export const useUserProjectAssignmentById = (
	projectAssignmentId: number | undefined
) =>
	useQueryWithCallback({
		queryKey: [
			TeamboardQueriesKeys.getProjectAssignmentById,
			projectAssignmentId,
		],
		queryFn: async () =>
			axios
				.get<UserProjectAssignmentsByIdResponse>(
					`${GET_USER_PROJECTS_ENDPOINT}/${projectAssignmentId}`
				)
				.then((response) => response.data),
		refetchOnWindowFocus: false,
		enabled: Boolean(projectAssignmentId),
	});
