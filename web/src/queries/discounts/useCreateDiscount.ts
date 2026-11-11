import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { GET_DISCOUNTS_API_ENDPOINT } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { Discount, DiscountAddData } from '@/types/discounts';

export const useCreateDiscount = (
	options?: UseMutationOptions<
		Discount,
		AxiosError<ServerResponseError>,
		DiscountAddData
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.post(GET_DISCOUNTS_API_ENDPOINT, data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
