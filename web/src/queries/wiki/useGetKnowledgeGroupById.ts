import axios, { AxiosError } from 'axios';

import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { getKnowledgeGroupByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { KnowledgeGroupItem } from '@/queries/wiki/types';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetKnowledgeGroupById = (
	groupId?: number | string,
	options?: PatchedQueryOptions<KnowledgeGroupItem, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [wikiQueriesKeys.getKnowledgeGroupById, groupId],
		queryFn: () =>
			axios
				.get<KnowledgeGroupItem>(getKnowledgeGroupByIdEndpoint(groupId!))
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
