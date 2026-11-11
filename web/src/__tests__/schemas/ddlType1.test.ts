import { ddlType1Schema } from '@/forms/schemas/types/ddlType1';

const DROPDOWN_ITEM = {
	id: 1,
	name: 'DesktopComputer',
	description: 'Desktop computer',
	location: 'null',
};

describe('Schema ddlType1Schema', () => {
	it('Should return true for valid value', () => {
		expect(ddlType1Schema().validateSync(DROPDOWN_ITEM));
	});

	it('Should return false for empty dropdown value with no description field', () => {
		const INVALID_ITEM = {};
		const validationValue = ddlType1Schema().isValidSync(INVALID_ITEM);
		expect(validationValue).toEqual(false);
	});
});
