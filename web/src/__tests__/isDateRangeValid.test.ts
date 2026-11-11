import { isDateRangeValid } from '@/helpers/isDateRangeValid';

const validDateRangesWithNoEquals = [
	[new Date('2024-04-30T23:59:59.000'), new Date('2024-05-01T00:00:00.000')],
	[new Date('2024-01-01'), new Date('2024-01-05')],
	[new Date('2023-04-30'), new Date('2024-04-30')],
];

const notValidDateRangesWithNoEquals = [
	[new Date('2024-05-01'), new Date('2024-04-30')],
	[new Date('2024-01-05'), new Date('2024-01-01')],
	[new Date('2024-04-30'), new Date('2023-04-30')],
	[new Date('2024-04-30'), new Date('2024-04-30')],
];

const validDateRangesWithEquals = [
	[new Date('2024-04-30'), new Date('2024-05-01')],
	[new Date('2024-01-01'), new Date('2024-01-05')],
	[new Date('2023-04-30'), new Date('2024-04-30')],
	[new Date('2024-04-30'), new Date('2024-04-30')],
	[new Date('2124-04-30T23:59:59.000'), new Date('2124-04-30T00:00:00.000')],
];

const notValidDateRangesWithEquals = [
	[new Date('2024-05-01'), new Date('2024-04-30')],
	[new Date('2024-01-05'), new Date('2024-01-01')],
	[new Date('2024-04-30'), new Date('2023-04-30')],
];

describe('isDateRangeValid helper test', () => {
	it('should return true if some arguments are undefined', () => {
		const fromDate = new Date('2021-01-01');

		expect(isDateRangeValid()).toBe(true);
		expect(isDateRangeValid(fromDate)).toBe(true);
	});

	it.each(validDateRangesWithNoEquals)(
		'should return true for valid ranges without equals',
		(startDate, endDate) => {
			expect(isDateRangeValid(startDate, endDate)).toBe(true);
		}
	);

	it.each(notValidDateRangesWithNoEquals)(
		'should return false for not valid ranges without equals',
		(startDate, endDate) => {
			expect(isDateRangeValid(startDate, endDate)).toBe(false);
		}
	);

	it.each(validDateRangesWithEquals)(
		'should return true for valid ranges with equals',
		(startDate, endDate) => {
			expect(isDateRangeValid(startDate, endDate, true)).toBe(true);
		}
	);

	it.each(notValidDateRangesWithEquals)(
		'should return false for not valid ranges with equals',
		(startDate, endDate) => {
			expect(isDateRangeValid(startDate, endDate, true)).toBe(false);
		}
	);
});
