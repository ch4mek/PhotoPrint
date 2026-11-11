import { NotificationType } from '@/constants/notifications';
import { ServerToastType } from '@/types/common';

export const convertServerToastType = (
	serverToastType: ServerToastType
): NotificationType => {
	switch (serverToastType) {
		case 'Info':
			return NotificationType.INFO;
		case 'Success':
			return NotificationType.SUCCESS;
		default:
			return NotificationType.ERROR;
	}
};
