import { UseMutationOptions, useMutation } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { SOMETHING_WENT_WRONG } from '@/constants/errors';
import { HttpStatuses } from '@/constants/httpStatuses';
import { NotificationType } from '@/constants/notifications';
import { GET_BOOKING_API_ENDPOINT } from '@/constants/queryPaths';
import { ToastContextValues } from '@/context/types';
import { createToast } from '@/helpers/createToast';

import { DeleteMutationVariables } from './types';

export const useDeleteBooking = (
	options: UseMutationOptions<void, AxiosError, DeleteMutationVariables>,
	toast: ToastContextValues
) =>
	useMutation<void, AxiosError, DeleteMutationVariables>({
		...options,
		mutationFn: ({ id }) =>
			axios
				.delete(`${GET_BOOKING_API_ENDPOINT}/${id}`)
				.then(() => {})
				.catch((error: unknown) => {
					if (
						error instanceof AxiosError &&
						error.response?.status !== HttpStatuses.NotFound
					) {
						toast.open(
							createToast(NotificationType.ERROR, SOMETHING_WENT_WRONG)
						);
					}
				}),
	});
