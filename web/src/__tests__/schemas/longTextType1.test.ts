import {
	FIELD_IS_REQUIRED,
	getPleaseEnterAValidFieldErrorMessage,
} from '@/constants/errors';
import {
	DEFAULT_LONG_TEXT_TYPE_1_MAX_CHARACTERS,
	longTextType1Schema,
} from '@/forms/schemas';

const MIN_CHARACTERS = 2;
const MAX_CHARACTERS = DEFAULT_LONG_TEXT_TYPE_1_MAX_CHARACTERS;
const FIELD_NAME = 'test';
const ONLY_SPASES_STRING = ' '.repeat(20);

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const SHORT_VALUE = 'f'.repeat(MIN_CHARACTERS - 1);

const VALID_VALUE = 'valid value';
const SYMBOLS = '.,:;_!@#$%^&*()_"{}<>+-/&^/?!№$%*~';
const DIGITS = '1234567890';

describe('Schema longTextType1Schema', () => {
	it('Should return true for valid value', () => {
		const validationValue =
			longTextType1Schema('test').validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE);
	});

	it('Should return false if value is too short', () => {
		const validationValue =
			longTextType1Schema('test').isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too long', () => {
		const validationValue =
			longTextType1Schema('test').isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return true if value has characters', () => {
		SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(longTextType1Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return true if value has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(longTextType1Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return false if schema is mandatory and value is empty', () => {
		const value = ``;
		expect(longTextType1Schema('test', true).isValidSync(value)).toEqual(false);
	});

	it('Should return true if schema is optional and value is empty', () => {
		const value = ``;
		expect(longTextType1Schema('test', false).isValidSync(value)).toEqual(true);
	});

	it('Should respect custom minCountOfCharacters', () => {
		const value = 'ab';
		expect(longTextType1Schema('test', true, 3).isValidSync(value)).toEqual(
			false
		);
		expect(longTextType1Schema('test', true, 2).isValidSync(value)).toEqual(
			true
		);
	});

	it('Should skip symbol check if isIncludeSymbolCheck = false', () => {
		const value = '!!!@@@###';
		expect(
			longTextType1Schema('test', false, MIN_CHARACTERS, false).isValidSync(
				value
			)
		).toEqual(true);
	});

	it('Should fail symbol check if isIncludeSymbolCheck = true and value has only symbols', () => {
		const value = '!!!@@@###';
		expect(
			longTextType1Schema('test', true, MIN_CHARACTERS, true).isValidSync(value)
		).toEqual(false);
	});

	it('Should respect custom maxCountOfCharacters', () => {
		const customMax = 5000;
		const validCustomValue = 'f'.repeat(customMax);
		const overflowCustomValue = 'f'.repeat(customMax + 1);

		expect(
			longTextType1Schema('test', true, 2, true, false, customMax).isValidSync(
				validCustomValue
			)
		).toEqual(true);
		expect(
			longTextType1Schema('test', true, 2, true, false, customMax).isValidSync(
				overflowCustomValue
			)
		).toEqual(false);
		expect(
			longTextType1Schema('test', true).isValidSync(validCustomValue)
		).toEqual(false);
	});

	it('Should return required message for empty mandatory field', async () => {
		await expect(
			longTextType1Schema(FIELD_NAME, true).validate('')
		).rejects.toThrow(FIELD_IS_REQUIRED);
	});

	it('Should return validity message for mandatory field with only spaces', async () => {
		const validityMessage = getPleaseEnterAValidFieldErrorMessage(FIELD_NAME);

		await expect(
			longTextType1Schema(FIELD_NAME, true).validate(ONLY_SPASES_STRING)
		).rejects.toThrow(validityMessage);
	});
});
