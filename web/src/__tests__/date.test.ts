import { TimePeriod } from '../types';

import {
	changeSelectDate,
	generateMonths,
	generateQuarter,
	generateShorthandOfQuarter,
	generateYearsCompanyExistence,
	generateYearsWithFutureYearOptions,
	isCurrentYearMonth,
	isDateMoreThanCurrent,
	isFutureMonth,
	parseDayjsDateToUTCString,
} from '@/helpers/date';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs', () => jest.fn());

jest.mock('@/constants/date', () => ({
	months: [
		'January',
		'February',
		'March',
		'April',
		'May',
		'June',
		'July',
		'August',
		'September',
		'October',
		'November',
		'December',
	],
	date: { getUTCFullYear: () => 2023 },
	COMPANY_FOUNDATION_DATE: 2020,
	quartersShorthands: { 'Jan-Mar': 'January-March', 'Apr-Jun': 'April-June' },
	DATE_FORMAT: 'DD/MM/YYYY',
	currentDate: { month: () => 5, year: () => 2023 },
}));

describe('Helper functions date', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;

	beforeEach(() => {
		jest.clearAllMocks();
	});

	describe('generateYearsCompanyExistence', () => {
		it('generates years from current to foundation year', () => {
			const result = generateYearsCompanyExistence();

			expect(result).toEqual([2023, 2022, 2021, 2020]);
		});
	});

	describe('generateShorthandOfQuarter', () => {
		it('generates shorthand for quarter', () => {
			const result = generateShorthandOfQuarter('January-March');

			expect(result).toBe('Jan-Mar, ');
		});

		it('returns empty string for undefined quarter', () => {
			const result = generateShorthandOfQuarter();

			expect(result).toBe('');
		});
	});

	describe('changeSelectDate', () => {
		it('parses year from TimePeriod.Year', () => {
			const result = changeSelectDate(TimePeriod.Year, '2023');

			expect(result).toEqual({ year: 2023 });
		});

		it('parses month and year from TimePeriod.Month', () => {
			const result = changeSelectDate(TimePeriod.Month, 'January, 2023');

			expect(result).toEqual({ year: '2023', month: 'January' });
		});

		it('parses quarter and year from TimePeriod.Quarter', () => {
			const result = changeSelectDate(TimePeriod.Quarter, 'Jan-Mar, 2023');

			expect(result).toEqual({ year: '2023', quarter: 'January-March' });
		});

		it('returns undefined for unknown title', () => {
			const result = changeSelectDate('Unknown', 'value');

			expect(result).toBeUndefined();
		});
	});

	describe('generateQuarter', () => {
		it('returns Q1 months', () => {
			const result = generateQuarter('January-March');

			expect(result).toEqual(['January', 'February', 'March']);
		});

		it('returns Q2 months', () => {
			const result = generateQuarter('April-June');

			expect(result).toEqual(['April', 'May', 'June']);
		});

		it('returns Q3 months', () => {
			const result = generateQuarter('July-September');

			expect(result).toEqual(['July', 'August', 'September']);
		});

		it('returns Q4 months', () => {
			const result = generateQuarter('October-December');

			expect(result).toEqual(['October', 'November', 'December']);
		});

		it('returns undefined for invalid quarter', () => {
			const result = generateQuarter('Invalid-Quarter');

			expect(result).toBeUndefined();
		});
	});

	describe('generateMonths', () => {
		it('converts month names to numbers', () => {
			const result = generateMonths(['January', 'March', 'December']);

			expect(result).toEqual([1, 3, 12]);
		});

		it('handles empty array', () => {
			const result = generateMonths([]);

			expect(result).toEqual([]);
		});
	});

	describe('parseDayjsDateToUTCString', () => {
		it('converts Dayjs to UTC string', () => {
			const mockDate = {} as any;
			const mockUtc = jest.fn().mockReturnValue({
				local: () => ({
					format: () => '2023-01-15T10:30:00+00:00',
				}),
			});
			mockDayjs.utc = mockUtc;

			const result = parseDayjsDateToUTCString(mockDate);

			expect(mockUtc).toHaveBeenCalledWith(mockDate);
			expect(result).toBe('2023-01-15T10:30:00Z');
		});

		it('returns null for null input', () => {
			const result = parseDayjsDateToUTCString(null);

			expect(result).toBeNull();
		});

		it('returns null for undefined input', () => {
			const result = parseDayjsDateToUTCString(undefined);

			expect(result).toBeNull();
		});
	});

	describe('isDateMoreThanCurrent', () => {
		it('returns true when target date is after current date', () => {
			const mockStartOf = jest.fn().mockReturnValue({
				isSameOrBefore: () => true,
			});
			mockDayjs.mockImplementation(() => ({ startOf: mockStartOf } as any));

			const result = isDateMoreThanCurrent(new Date('2023-12-31'));
			expect(result).toBe(true);
		});

		it('returns false when target date is before current date', () => {
			const mockStartOf = jest.fn().mockReturnValue({
				isSameOrBefore: () => false,
			});
			mockDayjs.mockImplementation(() => ({ startOf: mockStartOf } as any));

			const result = isDateMoreThanCurrent(new Date('2020-01-01'));
			expect(result).toBe(false);
		});
	});

	describe('isCurrentYearMonth', () => {
		it('returns true for current month and year', () => {
			const result = isCurrentYearMonth(6, 2023);

			expect(result).toBe(true);
		});

		it('returns false for different month', () => {
			const result = isCurrentYearMonth(12, 2023);

			expect(result).toBe(false);
		});

		it('returns false for different year', () => {
			const result = isCurrentYearMonth(6, 2022);

			expect(result).toBe(false);
		});
	});

	describe('isFutureMonth', () => {
		it('returns true for future month', () => {
			const mockIsAfter = jest.fn().mockReturnValue(true);
			mockDayjs.mockReturnValue({ isAfter: mockIsAfter } as any);

			const result = isFutureMonth(12, 2023);

			expect(mockDayjs).toHaveBeenCalledWith({ year: 2023, month: 11 });
			expect(result).toBe(true);
		});

		it('returns false for past month', () => {
			const mockIsAfter = jest.fn().mockReturnValue(false);
			mockDayjs.mockReturnValue({ isAfter: mockIsAfter } as any);

			const result = isFutureMonth(1, 2023);

			expect(result).toBe(false);
		});
	});

	describe('generateYearsWithFutureYearOptions', () => {
		it('includes next year and company existence years', () => {
			jest.spyOn(Date.prototype, 'getFullYear').mockReturnValue(2023);
			const result = generateYearsWithFutureYearOptions();

			expect(result).toEqual([2024, 2023, 2022, 2021, 2020]);
		});
	});
});
