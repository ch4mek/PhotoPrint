import { Timezone } from '@/components/Timeline/types';
import { FULL_HOURS_IN_DAY, MAX_SECONDS_COUNT } from '@/constants/time';
import {
	parseDateToTimeObject,
	parseDayjsToUtcString,
	parseStringToDayjs,
	parseTimeToTodayDate,
} from '@/helpers/bookingParsers';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');

const mockedDayjs = dayjs as jest.MockedFunction<typeof dayjs>;

describe('Helper functions bookingParsers', () => {
	beforeEach(() => {
		jest.clearAllMocks();
	});

	describe('parseTimeToTodayDate', () => {
		it('should parse time string to today date', () => {
			const mockNow = {
				hour: jest.fn().mockReturnThis(),
				minute: jest.fn().mockReturnThis(),
				second: jest.fn().mockReturnThis(),
				millisecond: jest.fn().mockReturnValue('result'),
			};
			mockedDayjs.mockReturnValue(mockNow as any);

			const result = parseTimeToTodayDate('14:30');

			expect(mockNow.hour).toHaveBeenCalledWith(14);
			expect(mockNow.minute).toHaveBeenCalledWith(30);
			expect(mockNow.second).toHaveBeenCalledWith(0);
			expect(mockNow.millisecond).toHaveBeenCalledWith(0);
			expect(result).toBe('result');
		});

		it('should handle single digit values', () => {
			const mockNow = {
				hour: jest.fn().mockReturnThis(),
				minute: jest.fn().mockReturnThis(),
				second: jest.fn().mockReturnThis(),
				millisecond: jest.fn().mockReturnValue('result'),
			};
			mockedDayjs.mockReturnValue(mockNow as any);

			parseTimeToTodayDate('9:05');

			expect(mockNow.hour).toHaveBeenCalledWith(9);
			expect(mockNow.minute).toHaveBeenCalledWith(5);
		});
	});

	describe('parseStringToDayjs', () => {
		it('should parse time string and return dayjs object', () => {
			const mockParsedTime = 'parsedTimeResult';
			const mockDayjsInstance = 'dayjsInstance';

			const mockTimeDate = {
				hour: jest.fn().mockReturnThis(),
				minute: jest.fn().mockReturnThis(),
				second: jest.fn().mockReturnThis(),
				millisecond: jest.fn().mockReturnValue(mockParsedTime),
			};
			mockedDayjs.mockReturnValueOnce(mockTimeDate as any);

			mockedDayjs.mockReturnValueOnce(mockDayjsInstance as any);

			const result = parseStringToDayjs('15:45');

			expect(mockedDayjs).toHaveBeenCalledWith(mockParsedTime, 'HH:mm');
			expect(result).toBe(mockDayjsInstance);
		});

		it('should work with different time formats', () => {
			const mockParsedTime = 'parsedTime';
			const mockDayjsInstance = 'dayjsResult';

			const mockTimeDate = {
				hour: jest.fn().mockReturnThis(),
				minute: jest.fn().mockReturnThis(),
				second: jest.fn().mockReturnThis(),
				millisecond: jest.fn().mockReturnValue(mockParsedTime),
			};
			mockedDayjs.mockReturnValueOnce(mockTimeDate as any);
			mockedDayjs.mockReturnValueOnce(mockDayjsInstance as any);

			const result = parseStringToDayjs('08:00');

			expect(mockedDayjs).toHaveBeenCalledWith(mockParsedTime, 'HH:mm');
			expect(result).toBe(mockDayjsInstance);
		});
	});

	describe('parseDateToTimeObject', () => {
		it('should parse date string to time object', () => {
			const mockToObject = jest.fn().mockReturnValue({
				years: 2023,
				months: 5,
				date: 15,
				hours: 14,
				minutes: 30,
				seconds: 45,
				milliseconds: 123,
			});

			const mockDayjsInstance = {
				toObject: mockToObject,
			};

			mockedDayjs.mockReturnValue(mockDayjsInstance as any);

			const result = parseDateToTimeObject('2023-06-15T14:30:45.123Z');

			expect(mockedDayjs).toHaveBeenCalledWith('2023-06-15T14:30:45.123Z');
			expect(mockToObject).toHaveBeenCalled();
			expect(result).toEqual({
				years: 2023,
				months: 5,
				date: 15,
				hours: 14,
				minutes: 30,
				seconds: 45,
				milliseconds: 123,
			});
		});

		it('should handle edge case when seconds equal MAX_SECONDS_COUNT', () => {
			const mockToObject = jest.fn().mockReturnValue({
				years: 2023,
				months: 5,
				date: 15,
				hours: 10,
				minutes: 25,
				seconds: MAX_SECONDS_COUNT,
				milliseconds: 999,
			});

			const mockDayjsInstance = {
				toObject: mockToObject,
			};

			mockedDayjs.mockReturnValue(mockDayjsInstance as any);

			const result = parseDateToTimeObject('2023-06-15T10:25:59.999Z');

			expect(result).toEqual({
				years: 2023,
				months: 5,
				date: 15,
				hours: FULL_HOURS_IN_DAY,
				minutes: 0,
				seconds: 0,
				milliseconds: 999,
			});
		});

		it('should not modify object when seconds is not MAX_SECONDS_COUNT', () => {
			const mockToObject = jest.fn().mockReturnValue({
				years: 2023,
				months: 5,
				date: 15,
				hours: 10,
				minutes: 25,
				seconds: 30,
				milliseconds: 500,
			});

			const mockDayjsInstance = {
				toObject: mockToObject,
			};

			mockedDayjs.mockReturnValue(mockDayjsInstance as any);

			const result = parseDateToTimeObject('2023-06-15T10:25:30.500Z');

			expect(result).toEqual({
				years: 2023,
				months: 5,
				date: 15,
				hours: 10,
				minutes: 25,
				seconds: 30,
				milliseconds: 500,
			});
		});
	});

	describe('parseDayjsToUtcString', () => {
		it('should convert dayjs object to UTC string with timezone', () => {
			const mockTz = jest.fn().mockReturnThis();
			const mockToISOString = jest
				.fn()
				.mockReturnValue('2023-06-15T12:30:00.000Z');
			const mockClone = jest.fn().mockReturnValue({
				tz: mockTz,
				toISOString: mockToISOString,
			});

			const mockDayjsInstance = {
				clone: mockClone,
			};

			const timezone = Timezone.MINSK;
			const result = parseDayjsToUtcString(mockDayjsInstance as any, timezone);

			expect(mockClone).toHaveBeenCalled();
			expect(mockTz).toHaveBeenCalledWith(timezone, true);
			expect(mockToISOString).toHaveBeenCalled();
			expect(result).toBe('2023-06-15T12:30:00.000Z');
		});

		it('should not mutate original dayjs instance', () => {
			const mockTz = jest.fn().mockReturnThis();
			const mockToISOString = jest.fn().mockReturnValue('iso-string');
			const mockClone = jest.fn().mockReturnValue({
				tz: mockTz,
				toISOString: mockToISOString,
			});

			const mockDayjsInstance = {
				clone: mockClone,
			};

			const timezone = Timezone.MINSK;
			parseDayjsToUtcString(mockDayjsInstance as any, timezone);

			expect(mockClone).toHaveBeenCalled();
		});

		it('should handle UTC timezone', () => {
			const mockTz = jest.fn().mockReturnThis();
			const mockToISOString = jest
				.fn()
				.mockReturnValue('2023-06-15T14:30:00.000Z');
			const mockClone = jest.fn().mockReturnValue({
				tz: mockTz,
				toISOString: mockToISOString,
			});

			const mockDayjsInstance = {
				clone: mockClone,
			};

			const timezone = Timezone.MINSK;
			const result = parseDayjsToUtcString(mockDayjsInstance as any, timezone);

			expect(mockTz).toHaveBeenCalledWith('Europe/Minsk', true);
			expect(result).toBe('2023-06-15T14:30:00.000Z');
		});
	});
});
