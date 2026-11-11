import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { GET_NOTIFICATIONS } from '@/constants/queryPaths';

import { ReadNotificationsResponse } from './types';

export const useReadNotifications = (
	options?: UseMutationOptions<
		ReadNotificationsResponse,
		AxiosError<ServerResponseError>,
		number[]
	>
) =>
	useMutation<
		ReadNotificationsResponse,
		AxiosError<ServerResponseError>,
		number[]
	>({
		mutationFn: async (notificationIds: number[]) => {
			const response = await axios.patch<ReadNotificationsResponse>(
				`${GET_NOTIFICATIONS}/read`,
				notificationIds
			);

			return response.data;
		},
		...options,
	});
