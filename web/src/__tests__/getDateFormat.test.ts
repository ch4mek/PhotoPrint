import { DATE_FORMAT } from '@/constants/date';
import { getDateFormat } from '@/helpers/getDateFormat';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');

describe('Helper function getDateFormat', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;
	const mockFormat = jest.fn();

	beforeEach(() => {
		jest.clearAllMocks();
		mockDayjs.mockReturnValue({ format: mockFormat } as any);
	});

	it('should format Dayjs object with given format', () => {
		const date = dayjs('2023-01-15');
		mockFormat.mockReturnValue('15.01.2023');

		const result = getDateFormat(date, DATE_FORMAT);

		expect(mockDayjs).toHaveBeenCalledWith(date);
		expect(mockFormat).toHaveBeenCalledWith(DATE_FORMAT);
		expect(result).toBe('15.01.2023');
	});

	it('should format string date with given format', () => {
		const date = '2023-01-15';
		const format = 'YYYY-MM-DD';
		mockFormat.mockReturnValue('2023-01-15');

		const result = getDateFormat(date, format);

		expect(mockDayjs).toHaveBeenCalledWith(date);
		expect(mockFormat).toHaveBeenCalledWith(format);
		expect(result).toBe('2023-01-15');
	});

	it('should return null when date is null', () => {
		const result = getDateFormat(null, 'DD/MM/YYYY');

		expect(mockDayjs).not.toHaveBeenCalled();
		expect(result).toBeNull();
	});

	it('should format with different format strings', () => {
		const date = '2023-12-25';
		mockFormat.mockReturnValue('25.12.2023');

		const result = getDateFormat(date, 'DD.MM.YYYY');

		expect(mockFormat).toHaveBeenCalledWith('DD.MM.YYYY');
		expect(result).toBe('25.12.2023');
	});
});
