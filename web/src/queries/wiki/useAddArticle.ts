import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ARTICLES_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

import { AddArticleParams, FullArticleItem } from './types';

export const useAddArticle = (
	options?: UseMutationOptions<
		FullArticleItem,
		AxiosError<ServerResponseError>,
		AddArticleParams
	>
) =>
	useMutation<
		FullArticleItem,
		AxiosError<ServerResponseError>,
		AddArticleParams
	>({
		mutationFn: async (data: AddArticleParams) => {
			try {
				const response = await axios.post<FullArticleItem>(
					ARTICLES_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
