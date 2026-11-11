import { getPeriod } from '@/helpers/getPeriod';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');
jest.mock('@/constants/date', () => ({
	CURRENT_YEAR: 2023,
	MAX_DATE_DAY: 31,
	MAX_DATE_MONTH: 11,
	MIN_DATE_DAY: 1,
	MIN_DATE_MONTH: 0,
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
}));

describe('Helper function getPeriod', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;

	beforeEach(() => {
		jest.clearAllMocks();
		mockDayjs.mockImplementation(
			(date) =>
				({
					date: () => (date instanceof Date ? date.getDate() : 31),
				} as any)
		);
	});

	it('should return full year period when months array is empty', () => {
		getPeriod(2023, []);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 0, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 11, 31));
	});

	it('should use CURRENT_YEAR when year not provided', () => {
		getPeriod(undefined, []);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 0, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 11, 31));
	});

	it('should return single month period when one month provided', () => {
		mockDayjs.mockImplementation(
			() =>
				({
					date: () => 31,
				} as any)
		);

		getPeriod(2023, ['March']);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 2, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 2, 31));
	});

	it('should handle February with correct last day', () => {
		mockDayjs.mockImplementation(
			() =>
				({
					date: () => 28,
				} as any)
		);

		getPeriod(2023, ['February']);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 1, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 1, 28));
	});

	it('should return array of dates for multiple months', () => {
		mockDayjs.mockImplementation(
			(date) =>
				({
					date: () => (date instanceof Date && date.getMonth() === 2 ? 31 : 30),
				} as any)
		);

		getPeriod(2023, ['March', 'April']);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 2, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 3, 30));
	});

	it('should use CURRENT_YEAR when year is null in multiple months case', () => {
		mockDayjs.mockImplementation(
			() =>
				({
					date: () => 30,
				} as any)
		);

		getPeriod(undefined, ['January', 'February']);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 0, 1));
		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, 1, 28));
	});

	it('should handle invalid month names', () => {
		getPeriod(2023, ['InvalidMonth']);

		expect(mockDayjs).toHaveBeenCalledWith(new Date(2023, -1, 1));
	});
});
