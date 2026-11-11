import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_MERCH_ELEMENTS_ENDPOINT } from '@/constants/queryPaths';
import { MerchElementsFormValues } from '@/forms/MerchElementForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useUpdateMerchElement = (
	options?: UseMutationOptions<
		MerchElementsFormValues,
		AxiosError<ServerResponseError>,
		MerchElementsFormValues
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.put(GET_MERCH_ELEMENTS_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
