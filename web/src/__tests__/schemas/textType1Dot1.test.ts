import { textType1Dot1Schema } from '@/forms/schemas';

const MIN_CHARACTERS = 5;
const MAX_CHARACTERS = 501;

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const SHORT_VALUE = 'f'.repeat(MIN_CHARACTERS - 1);
const ONLY_SPASES_STRING = ' '.repeat(20);

const NOT_VALID_SYMBOLS = '#@+&^"<>{}()";`._/?!№$%*~/?!№$%*~';
const VALID_VALUE = 'valid value';
const INVALID_VALUE = '# invalid value';
const DIGITS = '1234567890';

describe('Schema textType1Dot1Schema', () => {
	it('Should return true for valid value', () => {
		const validationValue =
			textType1Dot1Schema('test').validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE.trim());
	});

	it('Should return false for invalid value', () => {
		const validationValue =
			textType1Dot1Schema('test').isValidSync(INVALID_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too short', () => {
		const validationValue =
			textType1Dot1Schema('test').isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too long', () => {
		const validationValue =
			textType1Dot1Schema('test').isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value has one of forbidden characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(textType1Dot1Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(textType1Dot1Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has only spaces', () => {
		expect(textType1Dot1Schema('test').isValidSync(ONLY_SPASES_STRING)).toEqual(
			false
		);
	});

	it('Should return false if schema is mondatory and value is empty', () => {
		const value = ``;
		expect(textType1Dot1Schema('test').isValidSync(value)).toEqual(false);
	});

	it('Should return true if optional and value is empty', () => {
		const value = ``;
		expect(textType1Dot1Schema('test', true).isValidSync(value)).toEqual(false);
	});
});
