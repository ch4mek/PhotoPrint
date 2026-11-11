import { DateFormat } from '@/constants/date';
import { formatDate } from '@/helpers/formatDate';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');

describe('Helper function formatDate', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;
	const mockFormat = jest.fn();

	beforeEach(() => {
		jest.clearAllMocks();
		mockDayjs.mockReturnValue({ format: mockFormat } as any);
	});

	it('should format Date object with DD_MM_YYYY format', () => {
		const date = new Date('2023-01-15');
		mockFormat.mockReturnValue('15.01.2023');

		const result = formatDate(date, DateFormat.DD_MM_YYYY);

		expect(mockDayjs).toHaveBeenCalledWith(date);
		expect(mockFormat).toHaveBeenCalledWith(DateFormat.DD_MM_YYYY);
		expect(result).toBe('15.01.2023');
	});

	it('should format string date with YYYY_MM_DD format', () => {
		const date = '2023-01-15';
		mockFormat.mockReturnValue('2023-01-15');

		const result = formatDate(date, DateFormat.YYYY_MM_DD);

		expect(mockDayjs).toHaveBeenCalledWith(date);
		expect(mockFormat).toHaveBeenCalledWith(DateFormat.YYYY_MM_DD);
		expect(result).toBe('2023-01-15');
	});

	it('should format with ISO format', () => {
		const date = new Date('2023-01-15T10:30:00Z');
		mockFormat.mockReturnValue('2023-01-15T10:30:00.000Z');

		const result = formatDate(date, DateFormat.ISO);

		expect(mockDayjs).toHaveBeenCalledWith(date);
		expect(mockFormat).toHaveBeenCalledWith(DateFormat.ISO);
		expect(result).toBe('2023-01-15T10:30:00.000Z');
	});
});
