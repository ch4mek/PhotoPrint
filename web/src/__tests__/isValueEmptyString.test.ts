import { isValueEmptyString } from '@/helpers/isValueEmptyString';

describe('Helper function isValueEmptyString', () => {
	it('should return true for a empty string', () => {
		expect(isValueEmptyString('')).toBe(true);
	});

	it('should return true for a string with whitespace', () => {
		expect(isValueEmptyString(' ')).toBe(true);
	});

	it('should return false for not empty string', () => {
		expect(isValueEmptyString('f')).toBe(false);
	});
});
