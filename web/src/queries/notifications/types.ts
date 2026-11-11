import { QueryOptions } from 'odata-query';

import { RawUserNotification } from '@/types/notifications';

export interface DeleteNotificationResponse {
	userId: number;
	id: number;
}

export type GetNotificationsParams = Partial<
	QueryOptions<RawUserNotification>
> & { userId: number };

export interface DeleteAllNotificationResponse {
	deletedCount: number;
}

export interface ReadNotificationsResponse {
	updatedCount: number;
}
