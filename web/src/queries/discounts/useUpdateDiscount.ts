import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getDiscountByIdEndPoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { Discount, DiscountUpdateData } from '@/types/discounts';

export const useUpdateDiscount = (
	options?: UseMutationOptions<
		Discount,
		AxiosError<ServerResponseError>,
		DiscountUpdateData
	>
) =>
	useMutation({
		mutationFn: async ({ id, ...data }) => {
			try {
				const response = await axios.put(getDiscountByIdEndPoint(id), data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
