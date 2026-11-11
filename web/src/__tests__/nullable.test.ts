import {
	isNotEmptyish,
	toNullableDayjs,
	toNullableNumber,
	toNullableString,
} from '@/helpers/nullable';
import dayjs from '@/lib/dayjs';

describe('helpers/nullable', () => {
	describe('isNotEmptyish', () => {
		it('returns false for null/undefined/empty string', () => {
			expect(isNotEmptyish(null)).toBe(false);
			expect(isNotEmptyish(undefined)).toBe(false);
			expect(isNotEmptyish('')).toBe(false);
		});

		it('returns true for other falsy values and non-empty strings', () => {
			expect(isNotEmptyish(0)).toBe(true);
			expect(isNotEmptyish(false)).toBe(true);
			expect(isNotEmptyish('0')).toBe(true);
			expect(isNotEmptyish('test')).toBe(true);
		});
	});

	describe('toNullableDayjs', () => {
		it('returns null for emptyish values', () => {
			expect(toNullableDayjs(null)).toBeNull();
			expect(toNullableDayjs(undefined)).toBeNull();
			expect(toNullableDayjs('')).toBeNull();
		});

		it('returns dayjs instance for a date-like value', () => {
			const value = '2024-01-15';
			const result = toNullableDayjs(value);

			expect(result).not.toBeNull();
			expect(result?.isValid()).toBe(true);
			expect(result?.isSame(dayjs(value), 'day')).toBe(true);
		});
	});

	describe('toNullableNumber', () => {
		it('returns null for emptyish values', () => {
			expect(toNullableNumber(null)).toBeNull();
			expect(toNullableNumber(undefined)).toBeNull();
			expect(toNullableNumber('')).toBeNull();
		});

		it('converts strings and numbers to number', () => {
			expect(toNullableNumber('0')).toBe(0);
			expect(toNullableNumber(0)).toBe(0);
			expect(toNullableNumber('42')).toBe(42);
			expect(toNullableNumber(42)).toBe(42);
		});
	});

	describe('toNullableString', () => {
		it('returns null for emptyish values', () => {
			expect(toNullableString(null)).toBeNull();
			expect(toNullableString(undefined)).toBeNull();
			expect(toNullableString('')).toBeNull();
		});

		it('trims whitespace-only strings to null', () => {
			expect(toNullableString('   ')).toBeNull();
		});

		it('returns trimmed non-empty string', () => {
			expect(toNullableString('hello')).toBe('hello');
			expect(toNullableString('  hello  ')).toBe('hello');
		});
	});
});
