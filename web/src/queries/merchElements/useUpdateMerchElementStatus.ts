import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { updateMerchElementStatusEndpoint } from '@/constants/queryPaths';
import { MerchElementUpdateStatus } from '@/forms/MerchElementForm/types';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';

export const useUpdateMerchElementStatus = (
	merchId?: number | string,
	options?: UseMutationOptions<
		MerchElementUpdateStatus,
		AxiosError<ServerResponseError>,
		MerchElementUpdateStatus
	>
) =>
	useMutation({
		mutationFn: async (data) => {
			try {
				const response = await axios.patch(
					updateMerchElementStatusEndpoint(merchId!),
					data
				);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
