import error from '@/assets/toast/error.png';
import info from '@/assets/toast/info.png';
import success from '@/assets/toast/success.png';
import { NotificationType } from '@/constants/notifications';
import { ERROR, SUCCESS, INFO } from '@/theme';

export function getNotificationImageSrc(type: NotificationType) {
	switch (type) {
		case NotificationType.ERROR:
			return error;
		case NotificationType.INFO:
			return info;
		case NotificationType.SUCCESS:
		default:
			return success;
	}
}

export function getNotificationColor(type: NotificationType) {
	switch (type) {
		case NotificationType.ERROR:
			return ERROR;
		case NotificationType.INFO:
			return INFO;
		case NotificationType.SUCCESS:
		default:
			return SUCCESS;
	}
}
