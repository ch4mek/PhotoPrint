import {
	convertStringDateToValidFormat,
	isValidOnChange,
	isValidTime,
} from '@/helpers/validTimeChecks';

describe('Helper function isValidOnChange', () => {
	it('Shoud return fixed time string', () => {
		expect(isValidOnChange('08')).toBe('08:00');
		expect(isValidOnChange('0')).toBe('0');
	});
});

describe('Helper function convertStringDateToValidFormat', () => {
	it('Should return empty string if input is undefined', () => {
		expect(convertStringDateToValidFormat(undefined)).toBe('');
	});

	it('Should return empty string if input is empty string', () => {
		expect(convertStringDateToValidFormat('')).toBe('');
	});

	it("Shouldn\\'t affect correct time string", () => {
		expect(convertStringDateToValidFormat('08:00')).toBe('08:00');
	});

	it('Should return fixed time string', () => {
		expect(convertStringDateToValidFormat('8')).toBe('08:00');
		expect(convertStringDateToValidFormat(':')).toBe('');
		expect(convertStringDateToValidFormat('2:')).toBe('02:00');
		expect(convertStringDateToValidFormat(':2')).toBe('00:20');
		expect(convertStringDateToValidFormat('08')).toBe('08:00');
		expect(convertStringDateToValidFormat('08:')).toBe('08:00');
		expect(convertStringDateToValidFormat('8:0')).toBe('08:00');
		expect(convertStringDateToValidFormat(':02')).toBe('00:02');
		expect(convertStringDateToValidFormat('102')).toBe('102:00');
		expect(convertStringDateToValidFormat('8:00')).toBe('08:00');
		expect(convertStringDateToValidFormat('08:0')).toBe('08:00');
		expect(convertStringDateToValidFormat('8888')).toBe('8888:00');
		expect(convertStringDateToValidFormat('00:00')).toBe('');
	});
});

describe('Validation function isValidTime', () => {
	it('Should return true when input is valid', () => {
		expect(isValidTime('08:00')).toBe(true);
		expect(isValidTime('8:00')).toBe(true);
		expect(isValidTime('8:12')).toBe(true);
		expect(isValidTime('8:2')).toBe(true);
		expect(isValidTime('08')).toBe(true);
		expect(isValidTime('08:')).toBe(true);
		expect(isValidTime('23:59')).toBe(true);
	});

	it('Should return false when input is invalid', () => {
		expect(isValidTime('')).toBe(false);
		expect(isValidTime('8:221')).toBe(false);
		expect(isValidTime('8888:221')).toBe(false);
		expect(isValidTime('8:6')).toBe(false);
		expect(isValidTime('24:05')).toBe(false);
		expect(isValidTime('22:61')).toBe(false);
		expect(isValidTime('022:025')).toBe(false);
		expect(isValidTime('24:00')).toBe(false);
	});
});
