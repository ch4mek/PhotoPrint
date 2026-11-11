import { parsePositiveId } from '@/helpers/parsePositiveId';

describe('parsePositiveId', () => {
	it('should return 0 for undefined', () => {
		expect(parsePositiveId(undefined)).toBe(0);
	});

	it('should return 0 for empty string', () => {
		expect(parsePositiveId('')).toBe(0);
	});

	it('should return 0 for null (id == null check)', () => {
		expect(parsePositiveId(null)).toBe(0);
	});

	it('should parse valid positive integer strings', () => {
		expect(parsePositiveId('1')).toBe(1);
		expect(parsePositiveId('42')).toBe(42);
		expect(parsePositiveId('123')).toBe(123);
	});

	it('should return 0 for zero', () => {
		expect(parsePositiveId('0')).toBe(0);
	});

	it('should return 0 for negative numbers', () => {
		expect(parsePositiveId('-1')).toBe(0);
		expect(parsePositiveId('-42')).toBe(0);
	});

	it('should floor decimal numbers', () => {
		expect(parsePositiveId('3.7')).toBe(3);
		expect(parsePositiveId('10.9')).toBe(10);
	});

	it('should return 0 for non-numeric strings', () => {
		expect(parsePositiveId('abc')).toBe(0);
		expect(parsePositiveId('12abc')).toBe(0);
		expect(parsePositiveId('  ')).toBe(0);
	});

	it('should return 0 for NaN and Infinity', () => {
		expect(parsePositiveId('NaN')).toBe(0);
		expect(parsePositiveId('Infinity')).toBe(0);
	});
});
