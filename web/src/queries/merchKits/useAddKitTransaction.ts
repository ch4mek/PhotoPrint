import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_MERCH_KITS_TRANSACTIONS_ENDPOINT } from '@/constants/queryPaths';
import { MerchKitsTransactionFormValues } from '@/forms/MerchKitsTransactionForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useAddKitTransaction = (
	options?: UseMutationOptions<
		MerchKitsTransactionFormValues,
		AxiosError<ServerResponseError>,
		MerchKitsTransactionFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(
					GET_MERCH_KITS_TRANSACTIONS_ENDPOINT,
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
