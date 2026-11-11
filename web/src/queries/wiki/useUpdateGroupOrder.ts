import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { UPDATE_KNOWLEDGE_GROUPS_ORDER_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { GroupOrderItem } from '@/queries/wiki/types';

export const useUpdateGroupOrder = (
	options?: UseMutationOptions<
		GroupOrderItem,
		AxiosError<ServerResponseError>,
		GroupOrderItem
	>
) =>
	useMutation<GroupOrderItem, AxiosError<ServerResponseError>, GroupOrderItem>({
		mutationFn: async (data: GroupOrderItem) => {
			try {
				const response = await axios.put(
					UPDATE_KNOWLEDGE_GROUPS_ORDER_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
