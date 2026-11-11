import {
	checkIfNoLettersOrNumbers,
	checkToAllSpacesFromSecondSymbol,
} from '@/forms/utils';

describe('Util function checkToAllSpacesFromSecondSymbol', () => {
	it('Should return false if argument is undefined', () => {
		expect(checkToAllSpacesFromSecondSymbol(undefined)).toBe(false);
	});

	it('Should return false if argument is string with length less then 2 symbols', () => {
		expect(checkToAllSpacesFromSecondSymbol('')).toBe(false);
		expect(checkToAllSpacesFromSecondSymbol('a')).toBe(false);
	});

	it('Should return false if argument is string of 2+ symbols and starting from 2nd symbol there is any that is not space', () => {
		expect(checkToAllSpacesFromSecondSymbol('ab')).toBe(false);
		expect(checkToAllSpacesFromSecondSymbol('a b')).toBe(false);
		expect(checkToAllSpacesFromSecondSymbol('a    g')).toBe(false);
		expect(checkToAllSpacesFromSecondSymbol('a  b  c     d')).toBe(false);
	});

	it('Should return true if argument is string of 2+ symbols and starting from 2nd symbol there are only spaces', () => {
		expect(checkToAllSpacesFromSecondSymbol('a ')).toBe(true);
		expect(checkToAllSpacesFromSecondSymbol('a  ')).toBe(true);
		expect(checkToAllSpacesFromSecondSymbol('a         ')).toBe(true);
	});
});

describe('Util function checkIfNoLettersOrNumbers', () => {
	it('Should return true if argument is undefined or null', () => {
		expect(checkIfNoLettersOrNumbers(undefined)).toBe(true);
		expect(checkIfNoLettersOrNumbers(null)).toBe(true);
	});

	it('Should return false if argument has at lest one letter', () => {
		expect(checkIfNoLettersOrNumbers('a')).toBe(false);
		expect(checkIfNoLettersOrNumbers('(s')).toBe(false);
		expect(checkIfNoLettersOrNumbers('Ё')).toBe(false);
		expect(checkIfNoLettersOrNumbers('Ёжик')).toBe(false);
		expect(checkIfNoLettersOrNumbers('User')).toBe(false);
		expect(checkIfNoLettersOrNumbers('((&;Y}{]')).toBe(false);
		expect(checkIfNoLettersOrNumbers('a      ')).toBe(false);
		expect(checkIfNoLettersOrNumbers('a ()')).toBe(false);
		expect(checkIfNoLettersOrNumbers('{{{{З')).toBe(false);
	});

	it('Should return true if argument has at lest one letter', () => {
		expect(checkIfNoLettersOrNumbers('')).toBe(true);
		expect(checkIfNoLettersOrNumbers('(')).toBe(true);
		expect(checkIfNoLettersOrNumbers('())')).toBe(true);
		expect(checkIfNoLettersOrNumbers('{}:;<>[[[')).toBe(true);
		expect(checkIfNoLettersOrNumbers('[[')).toBe(true);
		expect(checkIfNoLettersOrNumbers('.,:;')).toBe(true);
		expect(checkIfNoLettersOrNumbers('{}:   ;<>   [[[')).toBe(true);
		expect(checkIfNoLettersOrNumbers('[   [')).toBe(true);
		expect(checkIfNoLettersOrNumbers('.  ,:   ;   ')).toBe(true);
	});
});
