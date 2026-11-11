import { UseQueryResult } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { FileKeys } from '@/constants/queriesKeys';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetBlobFile = (
	uri: string,
	options?: PatchedQueryOptions<Blob, AxiosError>
): UseQueryResult<Blob, AxiosError> =>
	useQueryWithCallback<Blob, AxiosError>({
		queryKey: [FileKeys.blobFile, uri],
		queryFn: () =>
			axios
				.get<Blob>(uri, {
					responseType: 'blob',
					paramsSerializer: {
						indexes: true,
					},
				})
				.then((response) => response.data)
				.catch((error) => {
					throw error;
				}),
		refetchOnWindowFocus: false,
		retry: false,
		gcTime: 0,
		...options,
	});
