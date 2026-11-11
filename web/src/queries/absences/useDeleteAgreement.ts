import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getAgreementByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useDeleteAgreement = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError, unknown>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.delete(getAgreementByIdApiEndpoint(id));

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
