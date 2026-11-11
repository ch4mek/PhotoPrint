import axios, { AxiosError } from 'axios';

import { getInquryCommentsEndpoint } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { Comment } from '@/types/comments';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetRequestComments = (
	id: number | string | undefined,
	options?: PatchedQueryOptions<Comment[], AxiosError>
) =>
	useQueryWithCallback({
		enabled: !!id,
		queryKey: [getInquryCommentsEndpoint(id!)],
		queryFn: () =>
			axios.get(getInquryCommentsEndpoint(id!)).then((res) => res.data),
		refetchOnWindowFocus: false,
		...options,
	});
