import { hashString } from '@/helpers/hashString';

describe('hashString', () => {
	it('should return the same hash for the same input', () => {
		const input = 'hello world';
		expect(hashString(input)).toBe(hashString(input));
	});

	it('should return different hashes for different inputs', () => {
		expect(hashString('abc')).not.toBe(hashString('def'));
		expect(hashString('hello')).not.toBe(hashString('world'));
	});

	it('should return base36 string', () => {
		const result = hashString('test');
		expect(result).toMatch(/^[a-z0-9]+$/);
		expect(typeof result).toBe('string');
	});

	it('should handle empty string', () => {
		expect(hashString('')).toBe('0');
	});

	it('should handle long strings without overflow', () => {
		const longString = 'a'.repeat(10000);
		const result = hashString(longString);
		expect(result).not.toBe('Infinity');
		expect(result).toBe(hashString(longString));
	});
});
