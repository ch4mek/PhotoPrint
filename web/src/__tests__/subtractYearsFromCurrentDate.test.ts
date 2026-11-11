import { subtractPeriodFromCurrentDate } from '@/helpers/subtractYearsFromCurrentDate';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');

describe('Helper function subtractYearsFromCurrentDate', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;
	const mockSubtract = jest.fn();
	const mockFormat = jest.fn();

	beforeEach(() => {
		jest.clearAllMocks();
		mockDayjs.mockReturnValue({
			subtract: mockSubtract,
			format: mockFormat,
		} as any);
		mockSubtract.mockReturnValue({ format: mockFormat } as any);
	});

	it('should subtract years from current date with default format', () => {
		mockFormat.mockReturnValue('2021-01-15');

		const result = subtractPeriodFromCurrentDate('years', 2);

		expect(mockDayjs).toHaveBeenCalledWith();
		expect(mockSubtract).toHaveBeenCalledWith(2, 'years');
		expect(mockFormat).toHaveBeenCalledWith('YYYY-MM-DD');
		expect(result).toBe('2021-01-15');
	});

	it('should subtract months from current date', () => {
		mockFormat.mockReturnValue('2022-10-15');

		const result = subtractPeriodFromCurrentDate('months', 3);

		expect(mockSubtract).toHaveBeenCalledWith(3, 'months');
		expect(mockFormat).toHaveBeenCalledWith('YYYY-MM-DD');
		expect(result).toBe('2022-10-15');
	});

	it('should subtract days from current date', () => {
		mockFormat.mockReturnValue('2023-01-10');

		const result = subtractPeriodFromCurrentDate('days', 5);

		expect(mockSubtract).toHaveBeenCalledWith(5, 'days');
		expect(mockFormat).toHaveBeenCalledWith('YYYY-MM-DD');
		expect(result).toBe('2023-01-10');
	});

	it('should use custom format when provided', () => {
		const customFormat = 'DD/MM/YYYY';
		mockFormat.mockReturnValue('15/01/2023');

		const result = subtractPeriodFromCurrentDate('years', 1, customFormat);

		expect(mockSubtract).toHaveBeenCalledWith(1, 'years');
		expect(mockFormat).toHaveBeenCalledWith(customFormat);
		expect(result).toBe('15/01/2023');
	});

	it('should handle zero amount', () => {
		mockFormat.mockReturnValue('2023-01-15');

		const result = subtractPeriodFromCurrentDate('days', 0);

		expect(mockSubtract).toHaveBeenCalledWith(0, 'days');
		expect(result).toBe('2023-01-15');
	});
});
