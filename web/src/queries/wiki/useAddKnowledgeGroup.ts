import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { KNOWLEDGE_GROUPS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { AddGroupParams, KnowledgeGroupItem } from '@/queries/wiki/types';

export const useAddKnowledgeGroup = (
	options?: UseMutationOptions<
		KnowledgeGroupItem,
		AxiosError<ServerResponseError>,
		AddGroupParams
	>
) =>
	useMutation<
		KnowledgeGroupItem,
		AxiosError<ServerResponseError>,
		AddGroupParams
	>({
		mutationFn: async (data: AddGroupParams) => {
			try {
				const response = await axios.post<KnowledgeGroupItem>(
					KNOWLEDGE_GROUPS_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
