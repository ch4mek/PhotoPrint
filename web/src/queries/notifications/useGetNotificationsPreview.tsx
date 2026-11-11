import { UseQueryResult } from '@tanstack/react-query';
import axios, { AxiosError } from 'axios';

import { NotificationQueriesKeys } from '@/constants/queriesKeys';
import { getUserNotificationsPreviewEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { NotificationType } from '@/types/notifications';

export const useGetUserNotificationsPreview = (
	userId: number,
	includedNotificationTypes?: Array<NotificationType>
): UseQueryResult<boolean, AxiosError> =>
	useQueryWithCallback<boolean, AxiosError>({
		queryKey: [
			NotificationQueriesKeys.notificationPreview,
			userId,
			includedNotificationTypes,
		],
		queryFn: () =>
			axios
				.get<boolean>(`${getUserNotificationsPreviewEndpointUrl(userId)}`, {
					params: {
						notificationTypes: includedNotificationTypes,
					},
					paramsSerializer: {
						indexes: null,
					},
				})
				.then((respone) => respone.data)
				.catch((error) => error),
		refetchOnWindowFocus: false,
	});
