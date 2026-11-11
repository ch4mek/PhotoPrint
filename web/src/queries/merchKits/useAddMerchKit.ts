import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_MERCH_KITS_ENDPOINT } from '@/constants/queryPaths';
import { MerchKitsFormValues } from '@/forms/MerchKitsFrom/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useAddMerchKit = (
	options?: UseMutationOptions<
		MerchKitsFormValues,
		AxiosError<ServerResponseError>,
		MerchKitsFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(GET_MERCH_KITS_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
