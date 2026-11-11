import { extractKeyById, extractKeyFromPrimitive } from '@/helpers/extractKey';

describe('Helper function extractKeyFromPrimitive', () => {
	it('should return the same value for strings', () => {
		expect(extractKeyFromPrimitive('test')).toBe('test');
	});

	it('should return the same value for numbers', () => {
		expect(extractKeyFromPrimitive(42)).toBe(42);
	});

	it('should return the same value for booleans', () => {
		expect(extractKeyFromPrimitive(true)).toBe(true);
		expect(extractKeyFromPrimitive(false)).toBe(false);
	});

	it('should return the same value for null', () => {
		expect(extractKeyFromPrimitive(null)).toBe(null);
	});

	it('should return the same value for undefined', () => {
		expect(extractKeyFromPrimitive(undefined)).toBe(undefined);
	});
});

describe('Helper function extractKeyById', () => {
	it('should extract id from object', () => {
		const obj = { id: 123, name: 'Test' };
		expect(extractKeyById(obj)).toBe(123);
	});

	it('should extract string id from object', () => {
		const obj = { id: 'test-id', name: 'Test' };
		expect(extractKeyById(obj)).toBe('test-id');
	});

	it('should handle objects with only id field', () => {
		const obj = { id: 'only-id' };
		expect(extractKeyById(obj)).toBe('only-id');
	});

	it('should work with complex objects', () => {
		const obj = {
			id: 999,
			name: 'Complex',
			nested: { value: 'test' },
			array: [1, 2, 3],
		};
		expect(extractKeyById(obj)).toBe(999);
	});
});
