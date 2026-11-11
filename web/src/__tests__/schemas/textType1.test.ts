import {
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import { textType1Schema } from '@/forms/schemas';

const MIN_CHARACTERS = 2;
const MAX_CHARACTERS = 250;
const FIELD_NAME = 'test';

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const SHORT_VALUE = 'f'.repeat(MIN_CHARACTERS - 1);
const ONLY_SPASES_STRING = ' '.repeat(20);

const NOT_VALID_SYMBOLS = '#@+&^"<>{}()";`._/?!№$%*~/?!№$%*~';
const VALID_VALUE = 'valid value';
const INVALID_VALUE = '# invalid value';
const DIGITS = '1234567890';

describe('Schema textType1', () => {
	const schema = textType1Schema(FIELD_NAME);

	it('Should return true for valid value', () => {
		const validationValue = schema.validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE.trim());
	});

	it('Should return false for invalid value', () => {
		const validationValue = schema.isValidSync(INVALID_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too short', () => {
		const validationValue = schema.isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too long', () => {
		const validationValue = schema.isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value has one of forbidden characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(schema.isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(schema.isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has only spaces', () => {
		expect(schema.isValidSync(ONLY_SPASES_STRING)).toEqual(false);
	});

	it('Should return required message for empty mandatory field', async () => {
		await expect(schema.validate('')).rejects.toThrow(FIELD_IS_REQUIRED);
	});

	it('Should return validity message for mandatory field with only spaces', async () => {
		const validityMessage = getPleaseEnterAValidFieldErrorMessage(FIELD_NAME);

		await expect(schema.validate(ONLY_SPASES_STRING)).rejects.toThrow(
			validityMessage
		);
	});
});
