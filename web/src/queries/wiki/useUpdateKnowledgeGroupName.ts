import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getKnowledgeGroupNameUpdateEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { KnowledgeGroupItem, UpdateGroupNameParam } from '@/queries/wiki/types';

export const useUpdateKnowledgeGroupName = (
	options?: UseMutationOptions<
		KnowledgeGroupItem,
		AxiosError<ServerResponseError>,
		UpdateGroupNameParam
	>
) =>
	useMutation<
		KnowledgeGroupItem,
		AxiosError<ServerResponseError>,
		UpdateGroupNameParam
	>({
		mutationFn: async (data: UpdateGroupNameParam) => {
			try {
				const { id, name } = data;
				const response = await axios.patch(
					getKnowledgeGroupNameUpdateEndpoint(id, name)
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
