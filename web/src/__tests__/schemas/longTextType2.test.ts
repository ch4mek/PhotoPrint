import { longTextType2Schema } from '@/forms/schemas';

const SHORT_VALUE_LENGTH = 2;
const LONG_VALUE_LENGTH = 1000;

const OVERFLOW_VALUE = 'f'.repeat(SHORT_VALUE_LENGTH + 1);
const SHORT_VALUE = 'f'.repeat(LONG_VALUE_LENGTH - 1);

const VALID_VALUE = 'valid value';
const SYMBOLS = '.,:;_!@#$%^&*()_"{}<>+-/&^/?!№$%*~';
const DIGITS = '1234567890';

describe('Schema longTextType2Schema', () => {
	it('Should return true for valid value', () => {
		const validationValue =
			longTextType2Schema('test').validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE);
	});

	it('Should return true if value is too short', () => {
		const validationValue =
			longTextType2Schema('test').isValidSync(SHORT_VALUE);
		expect(validationValue).toEqual(true);
	});

	it('Should return true if value is too long', () => {
		const validationValue =
			longTextType2Schema('test').isValidSync(OVERFLOW_VALUE);
		expect(validationValue).toEqual(true);
	});

	it('Should return true if value has characters', () => {
		SYMBOLS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(longTextType2Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return true if value has digits', () => {
		DIGITS.split('').forEach((char) => {
			const value = `text${char}`;
			expect(longTextType2Schema('test').isValidSync(value)).toEqual(true);
		});
	});
});
