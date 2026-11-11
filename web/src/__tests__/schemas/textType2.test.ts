import { textType2Schema } from '@/forms/schemas';

const MIN_CHARACTERS = 2;
const MAX_CHARACTERS = 250;

const OVERFLOW_VALUE = 'f'.repeat(MAX_CHARACTERS + 1);
const SHORT_VALUE = 'f'.repeat(MIN_CHARACTERS - 1);
const ONLY_SPASES_STRING = ' '.repeat(20);

const NOT_VALID_SYMBOLS = '#@+&^/?!№$%*~';
const VALID_SYMBOLS = '.,:;_`()[]<>{}&/|\\';
const VALID_VALUE = 'valid value';
const INVALID_VALUE = '# invalid value';
const DIGITS = '1234567890';

describe('Schema textType2', () => {
	it('Should return true for valid value', () => {
		const validationValue = textType2Schema('test').validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE.trim());
	});

	it('Should return false for invalid value', () => {
		const validationValue = textType2Schema('test').isValidSync(INVALID_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false for not valid symbols', () => {
		const validationValue =
			textType2Schema('test').isValidSync(NOT_VALID_SYMBOLS);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too short', () => {
		const validationValue = textType2Schema('test').isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return false if value is too long', () => {
		const validationValue = textType2Schema('test').isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(false);
	});

	it('Should return true if value has only legal characters', () => {
		VALID_SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(textType2Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return false if value has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(textType2Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has only spaces', () => {
		expect(textType2Schema('test').isValidSync(ONLY_SPASES_STRING)).toEqual(
			false
		);
	});
});
