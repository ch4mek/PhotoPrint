import { getEnumKeyByValue } from '@/helpers/getEnumKeyByValue';

describe('getEnumKeyByValue', () => {
	enum TestEnum {
		Key1 = 'Value1',
		Key2 = 'Value2',
		Key3 = 3,
	}

	it('should return the correct key for a string value', () => {
		const result = getEnumKeyByValue(TestEnum, 'Value1');
		expect(result).toBe('Key1');
	});

	it('should return the correct key for a numeric value', () => {
		const result = getEnumKeyByValue(TestEnum, 3);
		expect(result).toBe('Key3');
	});

	it('should return undefined for a value that does not exist in the enum', () => {
		const result = getEnumKeyByValue(TestEnum, 'NonExistentValue');
		expect(result).toBeUndefined();
	});

	it('should return undefined for a numeric value that does not exist in the enum', () => {
		const result = getEnumKeyByValue(TestEnum, 999);
		expect(result).toBeUndefined();
	});

	it('should handle enums with mixed string and number values', () => {
		const result1 = getEnumKeyByValue(TestEnum, 'Value2');
		expect(result1).toBe('Key2');

		const result2 = getEnumKeyByValue(TestEnum, 3);
		expect(result2).toBe('Key3');
	});
});
