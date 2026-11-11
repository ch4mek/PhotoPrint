import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_ANALYZE_QUESTION_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { ChatFormValues, Message } from '@/types/chatWidget';

export const useAnalyzeQuestion = (
	options?: UseMutationOptions<
		Message[],
		AxiosError<ServerResponseError>,
		ChatFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(
					GET_ANALYZE_QUESTION_API_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
