import { TimePrefix } from '@/constants/time';
import { generateTimeValue } from '@/helpers/generateTimeValue';

describe('Helper function generateTimeValue', () => {
	it('should return a string with number and prefix (minutes)', () => {
		expect(generateTimeValue(5, TimePrefix.Minutes)).toBe('5 min');
	});

	it('should return a string with number and prefix (hours)', () => {
		expect(generateTimeValue(2, TimePrefix.Hours)).toBe('2 h');
	});

	it('should handle zero hours value', () => {
		expect(generateTimeValue(0, TimePrefix.Hours)).toBe('0 h');
	});

	it('should handle zero minutes value', () => {
		expect(generateTimeValue(0, TimePrefix.Minutes)).toBe('0 min');
	});
});
