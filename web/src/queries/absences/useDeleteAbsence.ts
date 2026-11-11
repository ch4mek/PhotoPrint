import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getAbsenceByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useDeleteAbsence = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError, unknown>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.delete(getAbsenceByIdApiEndpoint(id), {
					data: {},
				});

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
