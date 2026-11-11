import error from '@/assets/toast/error.png';
import info from '@/assets/toast/info.png';
import success from '@/assets/toast/success.png';
import { NotificationType } from '@/constants/notifications';
import {
	getNotificationColor,
	getNotificationImageSrc,
} from '@/helpers/getNotificationImg';
import { ERROR, INFO, SUCCESS } from '@/theme';

describe('Helper function getNotificationImageSrc', () => {
	it('Should return error image source for NotificationType.ERROR', () => {
		const result = getNotificationImageSrc(NotificationType.ERROR);
		expect(result).toBe(error);
	});

	it('Should return info image source for NotificationType.INFO', () => {
		const result = getNotificationImageSrc(NotificationType.INFO);
		expect(result).toBe(info);
	});

	it('Should return success image source for NotificationType.SUCCESS', () => {
		const result = getNotificationImageSrc(NotificationType.SUCCESS);
		expect(result).toBe(success);
	});

	it('Should return success image source for unknown notification type', () => {
		const result = getNotificationImageSrc('unknownType' as NotificationType);
		expect(result).toBe(success);
	});
});

describe('Helper function getNotificationColor', () => {
	it('Should return error color for NotificationType.ERROR', () => {
		const result = getNotificationColor(NotificationType.ERROR);
		expect(result).toBe(ERROR);
	});

	it('Should return info color for NotificationType.INFO', () => {
		const result = getNotificationColor(NotificationType.INFO);
		expect(result).toBe(INFO);
	});

	it('Should return success color for NotificationType.SUCCESS', () => {
		const result = getNotificationColor(NotificationType.SUCCESS);
		expect(result).toBe(SUCCESS);
	});

	it('Should return success color for unknown notification type', () => {
		const result = getNotificationColor('unknownType' as NotificationType);
		expect(result).toBe(SUCCESS);
	});
});
