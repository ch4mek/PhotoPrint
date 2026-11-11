import axios, { AxiosError } from 'axios';

import { digitalHrQueriesKeys } from '@/constants/queriesKeys';
import { getChatHistoryApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { ByPageResponse, ServerResponseError } from '@/queries/types';
import { Message } from '@/types/chatWidget';
import { PatchedQueryOptions } from '@/types/patchedQueryOptions';

export const useGetChatHistory = (
	orderBy: string,
	pageNumber: number,
	rowsPerPage: number,
	options?: PatchedQueryOptions<
		ByPageResponse<Message>,
		AxiosError<ServerResponseError>
	>
) =>
	useQueryWithCallback<
		ByPageResponse<Message>,
		AxiosError<ServerResponseError>
	>({
		queryKey: [
			digitalHrQueriesKeys.getChatHistory,
			pageNumber,
			rowsPerPage,
			orderBy,
		],
		queryFn: async () => {
			try {
				const response = await axios.post<ByPageResponse<Message>>(
					getChatHistoryApiEndpoint(
						orderBy,
						rowsPerPage,
						rowsPerPage * pageNumber
					)
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},

		refetchOnWindowFocus: false,
		...options,
	});
