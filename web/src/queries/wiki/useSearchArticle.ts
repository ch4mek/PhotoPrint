import axios, { AxiosError } from 'axios';

import { wikiQueriesKeys } from '@/constants/queriesKeys';
import { SEARCH_ARTICLES_API_ENDPOINT } from '@/constants/queryPaths';
import { defaultSearchOptions } from '@/constants/search';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

import { SearchArticleParams, SearchArticleResponse } from './types';

export const useSearchArticle = <T = SearchArticleResponse>(
	params: SearchArticleParams,
	options?: PatchedQueryOptions<T, AxiosError>
) => {
	const { groupId, tagId, name, keywords } = params;

	const actualName =
		name && name.length <= defaultSearchOptions.maxCharacters ? name : '';

	return useQueryWithCallback({
		enabled: !!(groupId || tagId || keywords || name),
		queryKey: [
			wikiQueriesKeys.searchArticleData,
			{ groupId, tagId, name: actualName, keywords },
		],
		queryFn: () =>
			axios
				.get<T>(SEARCH_ARTICLES_API_ENDPOINT, {
					params: {
						groupId,
						tagId,
						name: actualName,
						keywords,
					},
				})
				.then((res) => res.data),
		refetchOnWindowFocus: false,
		...options,
	});
};
