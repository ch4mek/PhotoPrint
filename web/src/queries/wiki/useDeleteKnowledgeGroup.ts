import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getKnowledgeGroupByIdEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { KnowledgeGroupItem } from '@/queries/wiki/types';

export const useDeleteKnowledgeGroup = (
	options?: UseMutationOptions<
		KnowledgeGroupItem,
		AxiosError<ServerResponseError>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id: number | string) => {
			try {
				const response = await axios.delete(getKnowledgeGroupByIdEndpoint(id));

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
