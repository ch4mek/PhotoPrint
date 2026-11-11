import { getStringOrFallback } from '@/helpers/getStringOrFallback';

describe('getStringOrFallback', () => {
	it('should return the value for non-empty string', () => {
		expect(getStringOrFallback('hello')).toBe('hello');
		expect(getStringOrFallback('a')).toBe('a');
	});

	it('should return fallback for null', () => {
		expect(getStringOrFallback(null)).toBeNull();
		expect(getStringOrFallback(null, 'default')).toBe('default');
	});

	it('should return fallback for empty string', () => {
		expect(getStringOrFallback('')).toBeNull();
		expect(getStringOrFallback('', 'default')).toBe('default');
	});

	it('should return fallback for whitespace-only string', () => {
		expect(getStringOrFallback('   ')).toBeNull();
		expect(getStringOrFallback('\t\n')).toBeNull();
	});

	it('should use custom fallback value', () => {
		expect(getStringOrFallback('', '')).toBe('');
		expect(getStringOrFallback(null, 0)).toBe(0);
	});
});
