import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { projectExitQueriesKeys } from '@/constants/queriesKeys';
import { GET_PROJECT_EXIT_USERS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ProjectExitUserResponse } from '@/types/projectExits';

export const useProjectExitUsersLookup = () =>
	useQueryWithCallback<Array<ProjectExitUserResponse>, AxiosError>({
		queryKey: [projectExitQueriesKeys.getProjectExitUsers],
		queryFn: () =>
			axios
				.get<Array<ProjectExitUserResponse>>(
					GET_PROJECT_EXIT_USERS_API_ENDPOINT
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
