import axios, { AxiosError } from 'axios';

import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { KNOWLEDGE_GROUPS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { KnowledgeGroupItem } from '@/queries/wiki/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetKnowledgeGroups = (
	options?: PatchedQueryOptions<KnowledgeGroupItem[], AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [wikiQueriesKeys.getKnowledgeGroupsData],
		queryFn: () =>
			axios
				.get<KnowledgeGroupItem[]>(KNOWLEDGE_GROUPS_API_ENDPOINT)
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
