import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_PROJECTS_LOOKUP } from '@/constants/queryPaths';
import { getKeywordQuery } from '@/helpers/queries';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type { ProjectLookupResponse } from '@/types/project';

import { ProjectsLookupProps } from './types';

export const useProjectsLookup = (props?: ProjectsLookupProps) => {
	const { search, isPublic, isActive, onlyManagedProjects, enabled } =
		props || {};
	const query = getKeywordQuery({
		search,
		isPublic,
		isActive,
		onlyManagedProjects,
	});

	return useQueryWithCallback<Array<ProjectLookupResponse>, AxiosError>({
		queryKey: ['projects-lookup', query],
		queryFn: () =>
			axios
				.get<Array<ProjectLookupResponse>>(`${GET_PROJECTS_LOOKUP}${query}`)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled,
	});
};
