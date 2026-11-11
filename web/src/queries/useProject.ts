import { keepPreviousData } from '@tanstack/react-query';
import axios from 'axios';

import { GET_PROJECTS_API_ENDPOINT } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import type { ProjectEntity } from '@/types/project';

export const PROJECT_QUERY_KEY = 'project';

export const useProject = (id: number) =>
	useQueryWithCallback<ProjectEntity>({
		queryKey: [PROJECT_QUERY_KEY, id],
		queryFn: () =>
			axios
				.get<ProjectEntity>(`${GET_PROJECTS_API_ENDPOINT}/${id}`)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
		enabled: !!id,
	});
