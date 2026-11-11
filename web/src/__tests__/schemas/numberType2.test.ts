import { numberType2Schema } from '@/forms/schemas';

const NOT_VALID_SYMBOLS = 'sSтТ,:;_!@#$%^&*()_"{}<>-/&^/?!№$%*~';

const VALID_VALUES = [1, 13, 1234567890];

const INVALID_VALUES = [
	-1234567890,
	'1234567890+',
	123.12,
	123.123,
	'123,12',
	'+123.1.2',
];

describe('Schema numberType2', () => {
	it('Should return true for valid values', () => {
		VALID_VALUES.forEach((value) => {
			expect(numberType2Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return true if value has only legal characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `100${char}99`;
			expect(numberType2Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false for invalid values', () => {
		INVALID_VALUES.forEach((value) => {
			expect(numberType2Schema('test').isValidSync(value)).toEqual(false);
		});
	});
});
