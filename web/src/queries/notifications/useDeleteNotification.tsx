import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_NOTIFICATIONS } from '@/constants/queryPaths';

import { DeleteNotificationResponse } from './types';

export const useDeleteNotification = (
	options?: UseMutationOptions<
		DeleteNotificationResponse,
		AxiosError<ServerResponseError>,
		number
	>
) =>
	useMutation<
		DeleteNotificationResponse,
		AxiosError<ServerResponseError>,
		number
	>({
		mutationFn: async (notificationId: number) => {
			const response = await axios.delete<DeleteNotificationResponse>(
				`${GET_NOTIFICATIONS}/${notificationId}`
			);

			return response.data;
		},
		...options,
	});
