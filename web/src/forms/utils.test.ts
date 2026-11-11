import {
	checkEmptyField,
	checkIncludesOtherThanSpecialSymbols,
	checkNumberLength,
	checkUrlToAvatar,
	checkYearInValidRange,
} from './utils';

describe('Helper function checkNumberLength test suite', () => {
	test('should return false if value length more than limit', () => {
		expect(checkNumberLength(8, '123456789')).toBeFalsy();
		expect(checkNumberLength(1, '123456789')).toBeFalsy();
	});

	test('should return true if value length more or equal to limit or no value', () => {
		expect(checkNumberLength(9, '123456789')).toBeTruthy();
		expect(checkNumberLength(10, '123456789')).toBeTruthy();
		expect(checkNumberLength(1)).toBeTruthy();
	});
});

describe('Helper function checkYearInValidRange test suite', () => {
	test('should return false if year is less than 1900', () => {
		expect(checkYearInValidRange('Mon 01 Jan 1800 00:00:00 GMT')).toBeFalsy();
	});

	test('should return false if year is more than 2100', () => {
		expect(checkYearInValidRange('Mon 01 Jan 2200 00:00:00 GMT')).toBeFalsy();
	});

	test('should return true if year is 1900', () => {
		expect(checkYearInValidRange('Mon 01 Jan 1900 00:00:00 GMT')).toBeTruthy();
	});

	test('should return true if year is 2100', () => {
		expect(checkYearInValidRange('Mon 01 Jan 2100 00:00:00 GMT')).toBeTruthy();
	});

	test('should return true if year is between 1900 and 2100', () => {
		expect(checkYearInValidRange()).toBeTruthy();
	});

	test('should return true if year is undefined or null', () => {
		expect(checkYearInValidRange(null)).toBeTruthy();
		expect(checkYearInValidRange(undefined)).toBeTruthy();
	});
});

describe('Helper function checkUrlToAvatar test suite ', () => {
	test('should return false if url does not start with http:// or https://', () => {
		expect(checkUrlToAvatar('ftp://example.com/image.jpg')).toBeFalsy();
	});

	test('should return false if url does not end with .jpg, .jpeg, .png', () => {
		expect(checkUrlToAvatar('http://example.com/image.gif')).toBeFalsy();
	});

	test('should return true if url starts with http:// or https:// and ends with .jpg, .jpeg, .png', () => {
		expect(checkUrlToAvatar('http://example.com/image.jpg')).toBeTruthy();
		expect(checkUrlToAvatar('https://example.com/image.jpeg')).toBeTruthy();
		expect(checkUrlToAvatar('https://example.com/image.png')).toBeTruthy();
	});

	test('should return true if url is undefined', () => {
		expect(checkUrlToAvatar(undefined)).toBeFalsy();
	});

	test('should return true if url is empty', () => {
		expect(checkUrlToAvatar('')).toBeFalsy();
	});
});

describe('Helper function checkEmptyField test suite ', () => {
	test('should return false if value is empty', () => {
		expect(checkEmptyField('')).toBeFalsy();
	});

	test('should return false if value is undefined', () => {
		expect(checkEmptyField(undefined)).toBeFalsy();
	});

	test('should return true if value is not empty', () => {
		expect(checkEmptyField('test')).toBeTruthy();
	});
});

describe('Helper function checkIncludesOtherThanSpecialSymbols test suite ', () => {
	test('should return false if value is empty', () => {
		expect(checkIncludesOtherThanSpecialSymbols(' ')).toBeFalsy();
	});

	test('should return false if value consists only of special symbols', () => {
		expect(
			checkIncludesOtherThanSpecialSymbols('!@#$%^&*()_+=-{}[];№":*?`~')
		).toBeFalsy();
	});

	test('should return false if value consists only of special symbols and spaces', () => {
		expect(
			checkIncludesOtherThanSpecialSymbols('!@#$%^ &*()_+=-{}[ ];№":*?`~')
		).toBeFalsy();
	});

	test('should return true if value consists only of letters and numbers', () => {
		expect(checkIncludesOtherThanSpecialSymbols('1 test')).toBeTruthy();
	});

	test('should return true if value consists only of letters and numbers ans special symbols', () => {
		expect(checkIncludesOtherThanSpecialSymbols('1 test!')).toBeTruthy();
	});
});
