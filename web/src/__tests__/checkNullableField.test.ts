import { checkNullableField } from '@/forms/EquipmentForm/utils/checkNullableField';

describe('Helper function checkNullableField for Equipment form', () => {
	it('should return true', () => {
		expect(checkNullableField('guaranteeDate')).toBe(true);
		expect(checkNullableField('purchaseDate')).toBe(true);
		expect(checkNullableField('location')).toBe(true);
		expect(checkNullableField('storage')).toBe(true);
	});

	it('should return false', () => {
		expect(checkNullableField('hello')).toBe(false);
		expect(checkNullableField('inventoryNumber')).toBe(false);
	});
});
