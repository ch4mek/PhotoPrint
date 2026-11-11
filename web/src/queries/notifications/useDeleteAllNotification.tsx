import { useMutation, UseMutationOptions } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { ServerResponseError } from '../types';

import { getUserNotificationPrefixEndpointUrl } from '@/constants/queryPaths';
import { NotificationType } from '@/types/notifications';

import { DeleteAllNotificationResponse } from './types';

export const useDeleteAllNotification = (
	includedNotificationTypes: Array<NotificationType>,
	options?: UseMutationOptions<
		DeleteAllNotificationResponse,
		AxiosError<ServerResponseError>,
		number
	>
) =>
	useMutation<
		DeleteAllNotificationResponse,
		AxiosError<ServerResponseError>,
		number
	>({
		mutationFn: async (userId: number) => {
			const { data } = await axios.delete<DeleteAllNotificationResponse>(
				`${getUserNotificationPrefixEndpointUrl(userId)}`,
				{
					params: {
						notificationTypes: includedNotificationTypes,
					},
					paramsSerializer: {
						indexes: null,
					},
				}
			);

			return data;
		},
		...options,
	});
