import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_MERCH_TRANSACTIONS_ENDPOINT } from '@/constants/queryPaths';
import {
	MerchStorageFormValues,
	MerchStorageSubmitFormValues,
} from '@/forms/MerchStorageForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useAddStorageTransaction = (
	options?: UseMutationOptions<
		MerchStorageFormValues,
		AxiosError<ServerResponseError>,
		MerchStorageSubmitFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(
					GET_MERCH_TRANSACTIONS_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
