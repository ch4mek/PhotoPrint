import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { getAgreementByIdApiEndpoint } from '@/constants/queryPaths';
import { handleRequestError } from '@/helpers/requestErrorHandler';
import { ServerResponseError } from '@/queries/types';
import { AgreementItem } from '@/types/absences';

export const useUpdateAgreement = (
	options?: UseMutationOptions<
		AgreementItem,
		AxiosError<ServerResponseError>,
		AgreementItem
	>
) =>
	useMutation({
		mutationFn: async ({ id, ...data }) => {
			try {
				const response = await axios.put(getAgreementByIdApiEndpoint(id), data);

				return response.data;
			} catch (error) {
				return handleRequestError(error);
			}
		},
		...options,
	});
