import { parseRawNotification } from '../utils';

import { RawUserNotification, UserNotification } from '@/types/notifications';

import { readableNotificationTypes } from './constants';

export const parseNotificationResponse = (
	rawNotifications: Array<RawUserNotification>
): Array<UserNotification> => rawNotifications.map(parseRawNotification);

export const isNotificationReadable = (notification: UserNotification) => {
	const { read, notificationType } = notification;

	if (read) {
		return false;
	}

	if (readableNotificationTypes.includes(notificationType)) {
		return true;
	}

	return false;
};
