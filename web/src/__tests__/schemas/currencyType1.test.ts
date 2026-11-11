import { currencyType1Schema } from '@/forms/schemas';

// Valid currencies: USD($) EURO(€) PLN(zł) BYN(р)
// USD and EURO symbols are placed before the value ($100, €100'),
// PLN and BYN  symbols are placed after the value (100zł, 100р)

const NOT_VALID_SYMBOLS = '#@+&^"<>{}()";`._-/?!№$%*~';
const ONLY_SPASES_STRING = ' '.repeat(20);

const VALID_VALUES = [
	100,
	'$100',
	'€100',
	'100zł',
	'100р',
	100.99,
	'$100.1',
	'€100.99',
	'100.99zł',
	'100.99р',
];

const INVALID_VALUES = [
	-100,
	'100$',
	'10$0',
	'100€',
	'zł100',
	'р100',
	'100$.1',
	'100.99€',
	'100zł.99',
	'100р.99',
];

describe('currencyType1Schema', () => {
	it('Should return true for valid values', () => {
		VALID_VALUES.forEach((value) => {
			expect(currencyType1Schema('test').isValidSync(value)).toEqual(true);
		});
	});

	it('Should return false for invalid values', () => {
		INVALID_VALUES.forEach((value) => {
			expect(currencyType1Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has one of forbidden characters', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `${VALID_VALUES[0]}${char}`;
			expect(currencyType1Schema('test').isValidSync(value)).toEqual(false);
		});
	});

	it('Should return false if value has only spaces', () => {
		NOT_VALID_SYMBOLS.split('').forEach((char) => {
			const value = `${VALID_VALUES[0]}${char}`;
			expect(
				currencyType1Schema(ONLY_SPASES_STRING).isValidSync(value)
			).toEqual(false);
		});
	});
});
