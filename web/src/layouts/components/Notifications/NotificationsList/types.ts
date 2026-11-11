import { HubConnection } from '@microsoft/signalr';

import { UserNotification } from '@/types/notifications';

export interface NotificationParameteresContextValue {
	deleteNotification: (notificationId: number) => void;
	readNotification: (notificationId: number) => void;
	handleNotificationsModalClose: VoidFunction;
}

export interface NotificationListProps {
	connection: HubConnection | null;
	refetchUnreadNotificationStatus: VoidFunction;
	hasUnreadNotifications: boolean;
	maxHeight: number;
	handleNotificationsModalClose: VoidFunction;
	handleReceiveNotification: (notification: UserNotification) => void;
}
