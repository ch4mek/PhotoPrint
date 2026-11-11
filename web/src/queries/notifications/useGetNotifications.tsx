import axios, { AxiosError } from 'axios';
import buildQuery from 'odata-query';

import { ByPageResponse, ServerResponseError } from '../types';

import { NotificationQueriesKeys } from '@/constants/queriesKeys';
import { getUserNotificationsEndpointUrl } from '@/constants/queryPaths';
import { useQueryWithCallback } from '@/helpers/useQueryWithCallback';
import { RawUserNotification } from '@/types/notifications';

import { GetNotificationsParams } from './types';

export const useGetUserNotifications = (params: GetNotificationsParams) =>
	useQueryWithCallback<ByPageResponse<RawUserNotification>>({
		queryKey: [NotificationQueriesKeys.userNotifications, params],
		queryFn: async () => {
			const { userId, ...restParams } = params;
			try {
				const query = buildQuery(restParams);

				const { data } = await axios.get<ByPageResponse<RawUserNotification>>(
					`${getUserNotificationsEndpointUrl(userId)}/${query}`
				);

				return data;
			} catch (error) {
				throw error as AxiosError<ServerResponseError>;
			}
		},
		refetchOnWindowFocus: false,
		gcTime: 0,
	});
