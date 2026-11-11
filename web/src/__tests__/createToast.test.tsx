import { NewToastProps } from '@mms/mms-ui-library/components/NewToast';

import { NotificationType } from '@/constants/notifications';
import { createToast } from '@/helpers/createToast';

jest.mock('@mms/mms-ui-library/components/NewToast', () => ({
	NewToast: ({ status, text, onClose }: NewToastProps) => (
		<div data-testid="toast" data-status={status}>
			<div data-testid="text">{text}</div>
			<button onClick={onClose} type="button">
				Close
			</button>
		</div>
	),
}));

describe('Helper function createToast', () => {
	it('should create toast object with correct structure', () => {
		const result = createToast(NotificationType.SUCCESS, 'Test description');

		expect(result).toHaveProperty('message');
		expect(result).toHaveProperty('settings');
		expect(result.settings.type).toBe(NotificationType.SUCCESS);
		expect(result.settings.style).toEqual({ fontFamily: 'inherit' });
	});

	it('should create toast with toastId when provided', () => {
		const result = createToast(
			NotificationType.ERROR,
			'Error Description',
			'error-123'
		);

		expect(result.settings.toastId).toBe('error-123');
	});
});
