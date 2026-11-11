import { Buffer } from 'buffer';

import axios, { AxiosError } from 'axios';

import { FileKeys } from '@/constants/queriesKeys';
import { GET_FILE } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ServerResponseError } from '@/queries/types';
import { ImageFileSize } from '@/types/file';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

interface UseGetFileParams {
	fileId?: string;
	imageSize?: ImageFileSize;
}

export const useGetFile = (
	{ fileId, imageSize }: UseGetFileParams = {},
	options?: PatchedQueryOptions<
		string | undefined,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback({
		queryKey: [FileKeys.userAvatar, fileId, imageSize],
		queryFn: async () => {
			if (!fileId) {
				return undefined;
			}

			try {
				const params = imageSize ? { size: imageSize } : {};
				const response = await axios.get(`${GET_FILE}/${fileId}`, {
					responseType: 'arraybuffer',
					params,
				});

				return Buffer.from(response.data, 'binary').toString('base64');
			} catch {
				return undefined;
			}
		},
		refetchOnWindowFocus: false,
		refetchOnMount: false,
		staleTime: Infinity,
		...options,
	});
