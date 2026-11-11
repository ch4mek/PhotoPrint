import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getRestoreProjectExitApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useRestoreProjectExits = (
	options?: UseMutationOptions<
		void,
		AxiosError<ServerResponseError, unknown>,
		number | string
	>
) =>
	useMutation({
		mutationFn: async (id) => {
			try {
				const response = await axios.patch(
					getRestoreProjectExitApiEndpoint(id)
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
