import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getArticleByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

import { FullArticleItem, UpdateArticleParams } from './types';

export const useChangeArticle = (
	options?: UseMutationOptions<
		FullArticleItem,
		AxiosError<ServerResponseError>,
		UpdateArticleParams
	>
) =>
	useMutation<
		FullArticleItem,
		AxiosError<ServerResponseError>,
		UpdateArticleParams
	>({
		mutationFn: async ({ id, ...data }: UpdateArticleParams) => {
			try {
				const response = await axios.put(getArticleByIdEndpoint(id), data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
