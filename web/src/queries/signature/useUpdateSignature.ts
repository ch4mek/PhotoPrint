import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	NotificationType,
	HAS_BEEN_GENERATED,
} from '@/constants/notifications';
import { GET_SIGNATURE } from '@/constants/queryPaths';
import type { SignatureProps } from '@/forms/SignatureForm/Signature/types';
import type { SignatureResponse } from '@/queries/types';

export const useUpdateSignature = (
	showToast: (message: string, type: NotificationType) => void,
	options?: UseMutationOptions<
		SignatureResponse,
		AxiosError,
		SignatureProps,
		unknown
	>
) =>
	useMutation<SignatureResponse, AxiosError, SignatureProps>({
		...options,
		mutationFn: async ({ fullName, position, phone, email }) => {
			try {
				const { data } = await axios.put(GET_SIGNATURE, {
					fullName,
					position,
					phone,
					email,
				});
				showToast(HAS_BEEN_GENERATED, NotificationType.SUCCESS);

				return data;
			} catch (error: unknown | AxiosError) {
				const axiosError = error as AxiosError;
				showToast(
					(axiosError.response?.data as { Message: string })?.Message ||
						SOMETHING_WENT_WRONG,
					NotificationType.ERROR
				);

				return Promise.reject();
			}
		},
	});
