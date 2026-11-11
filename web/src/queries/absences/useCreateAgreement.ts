import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { AGREEMENT_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { Agreement } from '@/types/absences';

export const useCreateAgreement = (
	options?: UseMutationOptions<
		Agreement,
		AxiosError<ServerResponseError>,
		Agreement
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(AGREEMENT_API_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
