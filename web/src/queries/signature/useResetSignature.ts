import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { NotificationType } from '@/constants/notifications';
import { GET_SIGNATURE } from '@/constants/queryPaths';
import type { SignatureResponse } from '@/queries/types';

export const useResetSignature = (
	toast: (message: string, type: NotificationType) => void,
	options?: UseMutationOptions<SignatureResponse, AxiosError>
) =>
	useMutation<SignatureResponse, AxiosError>({
		...options,
		mutationFn: async () => {
			try {
				const { data } = await axios.delete(GET_SIGNATURE);

				return data;
			} catch {
				toast(SOMETHING_WENT_WRONG, NotificationType.ERROR);

				return Promise.reject();
			}
		},
	});
