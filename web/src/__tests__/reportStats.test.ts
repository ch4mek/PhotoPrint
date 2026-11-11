import { ChartStackedResponse } from '@/components/ChartStacked/types';
import {
	getHoursFromNumber,
	getMinutesFromNumber,
} from '@/components/ChartStacked/utils';
import { getStackedDataByPeriod } from '@/helpers/reportStats';
import dayjs from '@/lib/dayjs';

jest.mock('@/lib/dayjs');
jest.mock('@/components/ChartStacked/utils');

describe('getStackedDataByPeriod', () => {
	const mockDayjs = dayjs as jest.MockedFunction<typeof dayjs>;
	const mockGetHoursFromNumber = getHoursFromNumber as jest.MockedFunction<
		typeof getHoursFromNumber
	>;
	const mockGetMinutesFromNumber = getMinutesFromNumber as jest.MockedFunction<
		typeof getMinutesFromNumber
	>;
	const mockFormat = jest.fn();

	const mockReportsData: ChartStackedResponse = {
		totalTimeInMinutes: 840,
		timePerPeriod: [
			{
				period: '2023-01-01',
				totalTimeInMinutes: 480,
				projects: [
					{
						projectId: '1',
						projectName: 'Project A',
						totalTime: 300,
						projectColor: '#FF0000',
						isBillableProject: true,
					},
					{
						projectId: '2',
						projectName: 'Project B',
						totalTime: 180,
						projectColor: '#00FF00',
						isBillableProject: false,
					},
				],
			},
			{
				period: '2023-01-02',
				totalTimeInMinutes: 360,
				projects: [
					{
						projectId: '1',
						projectName: 'Project A',
						totalTime: 240,
						projectColor: '#FF0000',
						isBillableProject: true,
					},
					{
						projectId: '3',
						projectName: 'Project C',
						totalTime: 120,
						projectColor: '#0000FF',
						isBillableProject: false,
					},
				],
			},
		],
		projects: [{ projectId: 1 }, { projectId: 2 }, { projectId: 3 }],
	};

	beforeEach(() => {
		jest.clearAllMocks();
		mockDayjs.mockReturnValue({ format: mockFormat } as any);
		mockGetHoursFromNumber.mockImplementation((time) => Math.floor(time / 60));
		mockGetMinutesFromNumber.mockImplementation((time) => time % 60);
	});

	it('should return quarterly stats when quarter is provided', () => {
		const period = { year: 2023, quarter: 'January-March' };
		mockFormat.mockReturnValue('January');

		const result = getStackedDataByPeriod(period, mockReportsData);

		expect(mockDayjs).toHaveBeenCalledWith([2023, 0, 1]);
		expect(mockFormat).toHaveBeenCalledWith('MMMM');
		expect(result).toHaveLength(2);
		expect(result[0].projects[0]).toEqual({
			projectId: '1',
			projectName: 'Project A',
			projectColor: '#FF0000',
			isBillableProject: true,
			totalTime: 300,
			hours: 5,
			minutes: 0,
			hoursForBar: 5,
		});
	});

	it('should return monthly stats when month is provided', () => {
		const period = { year: 2023, month: 'January' };
		mockFormat.mockReturnValue('Monday');

		const result = getStackedDataByPeriod(period, mockReportsData);

		expect(mockDayjs).toHaveBeenCalledWith([2023, 0, 1]);
		expect(mockFormat).toHaveBeenCalledWith('dddd');
		expect(result).toHaveLength(2);
		expect(result[0].name).toEqual({
			date: 1,
			weekDay: 'Monday',
		});
	});

	it('should return yearly stats when no month or quarter provided', () => {
		const period = { year: 2023 };
		mockFormat.mockReturnValue('January');

		const result = getStackedDataByPeriod(period, mockReportsData);

		expect(mockDayjs).toHaveBeenCalledWith([2023, 0, 1]);
		expect(mockFormat).toHaveBeenCalledWith('MMMM');
		expect(result).toHaveLength(2);
		expect(result[0].name).toBe('January');
	});

	it('should calculate hoursForBar correctly with fractional minutes', () => {
		const period = { year: 2023 };
		mockGetHoursFromNumber.mockReturnValue(2);
		mockGetMinutesFromNumber.mockReturnValue(30);
		mockFormat.mockReturnValue('January');

		const result = getStackedDataByPeriod(period, mockReportsData);

		expect(result[0].projects[0].hoursForBar).toBe(2.5);
	});

	it('should handle empty projects array', () => {
		const emptyData = {
			timePerPeriod: [
				{
					totalTimeInMinutes: 0,
					projects: [],
				},
			],
		};
		const period = { year: 2023 };
		mockFormat.mockReturnValue('January');

		const result = getStackedDataByPeriod(period, emptyData as any);

		expect(result).toHaveLength(1);
		expect(result[0].projects).toEqual([]);
	});

	it('should handle quarter with correct month indices', () => {
		const period = { year: 2023, quarter: 'April-June' };
		mockFormat.mockReturnValue('April');

		getStackedDataByPeriod(period, mockReportsData);

		expect(mockDayjs).toHaveBeenCalledWith([2023, 3, 1]);
	});
});
