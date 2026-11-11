import { NotificationType } from '@/constants/notifications';
import { convertServerToastType } from '@/helpers/convertServerToastType';
import { ServerToastType } from '@/types/common';

describe('Helper function convertServerToastType', () => {
	it('should return INFO for Info server type', () => {
		expect(convertServerToastType('Info')).toBe(NotificationType.INFO);
	});

	it('should return SUCCESS for Success server type', () => {
		expect(convertServerToastType('Success')).toBe(NotificationType.SUCCESS);
	});

	it('should return ERROR for any other server type', () => {
		expect(convertServerToastType('test_unknown' as ServerToastType)).toBe(
			NotificationType.ERROR
		);
	});
});
