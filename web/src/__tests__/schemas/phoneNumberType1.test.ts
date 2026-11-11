import { phoneNumberType1Schema } from '@/forms/schemas';

const MAX_CHARACTERS = 15;

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const ONLY_SPASES_STRING = ' '.repeat(20);

const NOT_VALID_SYMBOLS = '#@&^"<>{}()";`._/?!№$%*~/?!№$%*~';
const VALID_SYMBOL = '+';
const VALID_VALUE = `${VALID_SYMBOL}375291234567`;
const INVALID_VALUE = `375291234567${VALID_SYMBOL}`;

describe('Schema phoneNumberType1', () => {
	it('Should return true for valid value', () => {
		const validationValue = phoneNumberType1Schema().validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE);
	});

	it('Should return false for invalid value', () => {
		const validationValue = phoneNumberType1Schema().isValidSync(INVALID_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too long', () => {
		const validationValue =
			phoneNumberType1Schema().isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value has one of forbidden characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(phoneNumberType1Schema().isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has not valid symbols', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `${VALID_VALUE}${char}`;
			expect(phoneNumberType1Schema().isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has only spaces', () => {
		expect(phoneNumberType1Schema().isValidSync(ONLY_SPASES_STRING)).toEqual(
			false
		);
	});

	it('Should return false if value more than 1 valid symbol', () => {
		const value = `${VALID_VALUE}${VALID_SYMBOL}`;
		expect(phoneNumberType1Schema().isValidSync(value)).toEqual(false);
	});
});
