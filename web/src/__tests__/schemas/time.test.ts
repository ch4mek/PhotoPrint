import { timeSchema } from '@/forms/schemas';

const MAX_NUMBERS_COUNT = 4;
const SEPARATIONS_COUNTER = 1;

const ALLOW_SYMBOL = ':';
const NOT_VALID_SYMBOLS = '/#@+&^"<>{}()";`._-?!№$%*~';

const DIGITS = '1234567890';

const VALID_VALUE = '13:57';
const INVALID_VALUES = ['13.57', '13.57', '33:57', '13:77'];

describe('Schema timeSchema', () => {
	it('Should return true for valid time', () => {
		const validationValue = timeSchema('test').validateSync(VALID_VALUE);
		expect(validationValue).toEqual(VALID_VALUE);
	});

	it('Should return false if value has one of forbidden characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `${VALID_VALUE.replace(ALLOW_SYMBOL, char)}`;
			expect(timeSchema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should include only valid chars', () => {
		const VALID_CHARS = DIGITS.concat(ALLOW_SYMBOL);

		VALID_VALUE.split('').forEach((char) => {
			const isValueHasNoIllegalChars = VALID_CHARS.includes(char);
			expect(isValueHasNoIllegalChars).toEqual(true);
		});

		const VALUE_WITH_WRONG_SEPARATION = '13/25';
		const isValueHasNoIllegalChars = VALID_CHARS.includes(
			VALUE_WITH_WRONG_SEPARATION
		);
		expect(isValueHasNoIllegalChars).toEqual(false);
	});

	it('Should return false for invalid time', () => {
		INVALID_VALUES.forEach((date) => {
			const validateValue = timeSchema('test').isValidSync(date);
			expect(validateValue).toEqual(false);
		});
	});

	it(`Should have length not over than ${
		MAX_NUMBERS_COUNT + SEPARATIONS_COUNTER
	} chars`, () => {
		expect(VALID_VALUE.split('').length).toBeLessThanOrEqual(
			MAX_NUMBERS_COUNT + SEPARATIONS_COUNTER
		);
	});

	it(`Should include ${SEPARATIONS_COUNTER} separations`, () => {
		const COUNTER = VALID_VALUE.split('').filter(
			(char) => char === ALLOW_SYMBOL
		);
		expect(COUNTER.length).toEqual(SEPARATIONS_COUNTER);
	});

	it('Should include a valid hour value', () => {
		const VALUE_WITH_INVALID_HOUR = '25:00';

		const validationValue = timeSchema('test').isValidSync(
			VALUE_WITH_INVALID_HOUR
		);
		expect(validationValue).toEqual(false);
	});

	it('Should include a valid minutes value', () => {
		const VALUE_WITH_INVALID_HOUR = '24:70';

		const validationValue = timeSchema('test').isValidSync(
			VALUE_WITH_INVALID_HOUR
		);
		expect(validationValue).toEqual(false);
	});
});
