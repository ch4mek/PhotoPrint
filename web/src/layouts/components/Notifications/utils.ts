import {
	EventRecordsRemovedNotification,
	NotificationType,
	RawUserNotification,
	UserNotification,
} from '@/types/notifications';

export const parseRawNotification = ({
	outputJson,
	...restProps
}: RawUserNotification): UserNotification => ({
	...restProps,
	...JSON.parse(outputJson),
	readByUser: restProps.read,
});

export const isEventRecordsRemovedNotification = (
	notification: UserNotification | RawUserNotification
): notification is EventRecordsRemovedNotification =>
	notification.notificationType === NotificationType.EventRecordsRemoved;
