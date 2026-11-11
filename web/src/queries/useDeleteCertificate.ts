import { useMutation } from '@tanstack/react-query';
import axios from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import {
	NotificationType,
	CERTIFICATE_DELETED_SUCCESSFULLY,
} from '@/constants/notifications';
import { GET_CERTIFICATES_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import { DeleteCertificateOptions } from './types';

export const useDeleteCertificate = (toast: ToastContextValues) =>
	useMutation({
		mutationFn: async ({ certificateId, userId }: DeleteCertificateOptions) => {
			try {
				const { data } = await axios.delete(
					`${GET_CERTIFICATES_API_ENDPOINT}/${certificateId}`,
					{
						params: { userId },
					}
				);
				toast.open(
					createToast(
						NotificationType.SUCCESS,
						CERTIFICATE_DELETED_SUCCESSFULLY
					)
				);

				return data;
			} catch (error) {
				toast.open(createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG));

				return Promise.reject(error);
			}
		},
	});
