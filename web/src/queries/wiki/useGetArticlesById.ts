import axios, { AxiosError } from 'axios';

import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { getArticleByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { FullArticleItem } from './types';

export const useGetArticleById = (
	articleId?: number | string,
	options?: PatchedQueryOptions<FullArticleItem, AxiosError>
) =>
	useQueryWithCallback({
		queryKey: [wikiQueriesKeys.getArticleById, articleId],
		queryFn: () =>
			axios
				.get<FullArticleItem>(getArticleByIdEndpoint(articleId!))
				.then((res) => res.data)
				.catch(handleRequestError),
		refetchOnWindowFocus: false,
		...options,
	});
