import { pluralize } from '@/helpers/pluralize';

describe('Helper function pluralize', () => {
	it('Should return the word without "s" if count = 1', () => {
		expect(pluralize('day', 1)).toBe('day');
		expect(pluralize('hour', 1)).toBe('hour');
	});

	it('Should return the word with "s" if count ≠ 1', () => {
		expect(pluralize('day', 0)).toBe('days');
		expect(pluralize('day', 2)).toBe('days');
		expect(pluralize('hour', 5)).toBe('hours');
	});

	it('Should add "s" by default if count is not provided', () => {
		expect(pluralize('day')).toBe('days');
	});
});
