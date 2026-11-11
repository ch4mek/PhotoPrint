import {
	isNeedZero,
	dateToString,
	formatDateString,
} from '@/helpers/dateToISOString';

describe('Helper function isNeedZero', () => {
	it('Should return 01 if input is 1 as a number', () => {
		expect(isNeedZero(1)).toBe('01');
	});

	it('Should return 01 if input is 1 as a string', () => {
		expect(isNeedZero('1')).toBe('01');
	});

	it('Should return 10 if input is 10 as a number', () => {
		expect(isNeedZero(10)).toBe(10);
	});

	it('Should return 10 if input is 10 as a string', () => {
		expect(isNeedZero('10')).toBe('10');
	});
});

describe('Helper function dateToString', () => {
	it('Shoult return iso date string in format YYYY-MM-DDT00:00:00.000Z', () => {
		const date = new Date('2023-08-08T11:49:05.617Z');
		expect(dateToString(date, '')).toBe('2023-08-08T00:00:00.000Z');
	});
});

describe('formatDateString', () => {
	it('should return "-" when dateISO is null', () => {
		const result = formatDateString(null);
		expect(result).toBe('-');
	});

	it('should format the date using dayjs', () => {
		const dateISO = '2022-01-01T00:00:00.000Z';
		const expectedResult = '01.01.2022';
		const result = formatDateString(dateISO);
		expect(result).toBe(expectedResult);
	});
});
