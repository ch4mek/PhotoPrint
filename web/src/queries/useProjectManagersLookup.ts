import { keepPreviousData } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_PROJECT_MANAGERS_API_ENDPOINT } from '@/constants/queryPaths';
import { getKeywordQuery } from '@/helpers/queries';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';
import { ProjectManager } from '@/types/userInfo';

interface UseProjectManagersLookupOptions {
	search?: string;
	queryOptions?: PatchedQueryOptions<Array<ProjectManager>, AxiosError>;
}

export const useProjectManagersLookup = ({
	search = '',
	queryOptions,
}: UseProjectManagersLookupOptions) => {
	const query = getKeywordQuery({ search });

	return useQueryWithCallback<Array<ProjectManager>, AxiosError>({
		...queryOptions,
		queryKey: ['project-managers-lookup', search],
		queryFn: () =>
			axios
				.get<Array<ProjectManager>>(
					`${GET_PROJECT_MANAGERS_API_ENDPOINT}/lookup${query}`
				)
				.then((res) => res.data),
		placeholderData: keepPreviousData,
		refetchOnWindowFocus: false,
	});
};
