import { isRefObject } from '@/helpers/isRefObject';

describe('Helper function isRefObject', () => {
	it('should return true for a valid MutableRefObject', () => {
		const mockRef = { current: document.createElement('input') };
		expect(isRefObject<HTMLInputElement>(mockRef)).toBe(true);
	});

	it('should return false for a callback ref', () => {
		const callbackRef = () => {};
		expect(isRefObject(callbackRef)).toBe(false);
	});

	it('should return false for null', () => {
		expect(isRefObject(null)).toBe(false);
	});

	it('should return false for plain object without "current"', () => {
		const plainObj = { notCurrent: 123 };
		expect(isRefObject(plainObj as any)).toBe(false);
	});

	it('should return false for primitive values', () => {
		expect(isRefObject('string' as any)).toBe(false);
		expect(isRefObject(123 as any)).toBe(false);
		expect(isRefObject(true as any)).toBe(false);
	});
});
