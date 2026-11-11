import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_INTERVIEWS_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useDeleteInterview = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError, unknown>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.delete(`${GET_INTERVIEWS_ENDPOINT}/${id}`);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
