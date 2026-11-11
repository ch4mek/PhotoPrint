import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { projectExitQueriesKeys } from '@/constants/queriesKeys';
import { GET_PROJECT_EXIT_REASONS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ProjectExitReason } from '@/pages/Staffing/components/ProjectExitsTab/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetClosingReasons = (
	options?: PatchedQueryOptions<
		ProjectExitReason[],
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<ProjectExitReason[], AxiosError<ServerResponseError>>({
		...options,
		queryKey: [projectExitQueriesKeys.getProjectExitReasons],
		queryFn: () =>
			axios.get(GET_PROJECT_EXIT_REASONS_API_ENDPOINT).then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
