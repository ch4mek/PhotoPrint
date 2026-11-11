import groupBy from 'lodash/groupBy';

import {
	ChartStackedProjectAdapted,
	ChartStackedProjectItemResponse,
	ChartStackedProjectResponse,
	ChartStackedResponse,
} from '@/components/ChartStacked/types';
import {
	getHoursFromNumber,
	getMinutesFromNumber,
} from '@/components/ChartStacked/utils';
import { MONTH_IN_YEAR, months } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import {
	Data,
	MonthData,
	MonthlyStat,
	Period,
	YearDataStack,
	YearStat,
} from '@/types/helpers';

import { minsToMinsRest } from './translateTimeUtils';

const FULL_DAY_FORMAT = 'dddd';
const FULL_MONTH_FORMAT = 'MMMM';

const getMonthlyStats = (
	monthData: MonthData[],
	year: number,
	monthPeriod: string
): MonthlyStat[] => {
	const currentMonth = months.findIndex((item) => item === monthPeriod);

	if (!monthData[0]) {
		return [];
	}
	const getFullCurrentDateByMonth = monthData?.[0]?.period;
	const monthStatsArray: MonthlyStat[] = new Array(
		dayjs(getFullCurrentDateByMonth).daysInMonth()
	)
		.fill(0)
		.map((_, index) => {
			const dayTracking = monthData?.find(
				(el) => Number(dayjs(el?.period).format('D')) === index + 1
			);

			return {
				name: {
					date: index + 1,
					weekDay: dayjs([year, currentMonth, index + 1]).format(
						FULL_DAY_FORMAT
					),
				},
				hours: dayTracking
					? Math.floor(dayTracking.totalTimeInMinutes / 60)
					: 0,
				minutes: dayTracking
					? minsToMinsRest(dayTracking.totalTimeInMinutes)
					: 0,
			};
		});

	return monthStatsArray;
};

const getYearStats = (
	yearDataStack: YearDataStack[],
	year: number
): YearStat[] => {
	const statsByMonth = yearDataStack?.map(({ totalTimeInMinutes, period }) => {
		const monthNumber = new Date(period).getMonth();

		return { totalTimeInMinutes, monthNumber };
	});
	const groupedYearStatsArray = groupBy(statsByMonth, 'monthNumber');
	const arrayOfMonthlyStats: number[] = [];
	const yearStatsArray: YearStat[] = new Array(MONTH_IN_YEAR)
		.fill(0)
		.map((_, index) => {
			if (groupedYearStatsArray[index]) {
				arrayOfMonthlyStats[index] = groupedYearStatsArray[index].reduce(
					(acc, monthItem) => Number(monthItem?.totalTimeInMinutes) + acc,
					0
				);
			}

			return {
				name: `${dayjs([year, index, 1]).format(FULL_MONTH_FORMAT)}`,
				hours: Math.floor(arrayOfMonthlyStats[index] / 60) || 0,
				minutes: minsToMinsRest(arrayOfMonthlyStats[index]),
			};
		});

	return yearStatsArray;
};

const getYearQuarter = (
	yearDataStack: YearDataStack[],
	year: number,
	quarter: string
): YearStat[] => {
	const quart = quarter
		.split('-')
		.map((pathOfQuarter) => months.findIndex((item) => item === pathOfQuarter));
	const statsByMonth = yearDataStack?.map(({ totalTimeInMinutes, period }) => {
		const monthNumber = new Date(period).getMonth();

		return { totalTimeInMinutes, monthNumber };
	});
	const groupedYearStatsArray = groupBy(statsByMonth, 'monthNumber');
	const arrayOfMonthlyStats: number[] = [];
	const yearStatsArray: YearStat[] = new Array(MONTH_IN_YEAR)
		.fill(0)
		.map((_, index) => {
			if (groupedYearStatsArray[index]) {
				arrayOfMonthlyStats[index] = groupedYearStatsArray[index].reduce(
					(acc, monthItem) => Number(monthItem?.totalTimeInMinutes) + acc,
					0
				);
			}

			return {
				name: `${dayjs([year, index, 1]).format(FULL_MONTH_FORMAT)}`,
				hours: Math.floor(arrayOfMonthlyStats[index] / 60) || 0,
				minutes: minsToMinsRest(arrayOfMonthlyStats[index]),
			};
		});

	return yearStatsArray.slice(quart[0], quart[1] + 1);
};

export const getDataByPeriod = (
	{ year, month, quarter }: Period,
	data: Data
) => {
	if (quarter) {
		return getYearQuarter(data?.timePerPeriod, year, quarter);
	}

	if (month) {
		return getMonthlyStats(data?.timePerPeriod, year, month);
	}

	return getYearStats(data?.timePerPeriod, year);
};

const getStackedMonthlyStats = (
	data: ChartStackedProjectResponse[],
	year: number,
	monthPeriod: string
): ChartStackedProjectAdapted[] => {
	const currentMonth = months.findIndex((item) => item === monthPeriod);

	const result: ChartStackedProjectAdapted[] = [];

	data.forEach((monthDataItem, index) => {
		result.push({
			totalTimeInMinutes: monthDataItem.totalTimeInMinutes,
			name: {
				date: index + 1,
				weekDay: dayjs([year, currentMonth, index + 1]).format(FULL_DAY_FORMAT),
			},
			projects: monthDataItem.projects.map(
				(project: ChartStackedProjectItemResponse) => ({
					...project,
					hours: getHoursFromNumber(project.totalTime),
					minutes: getMinutesFromNumber(project.totalTime),
					hoursForBar:
						getHoursFromNumber(project.totalTime) +
						getMinutesFromNumber(project.totalTime) / 60,
				})
			),
		});
	});

	return result;
};

const getStackedYearStats = (
	data: ChartStackedProjectResponse[],
	year: number
): ChartStackedProjectAdapted[] => {
	const result: ChartStackedProjectAdapted[] = [];

	data.forEach((yearDataItem, index) => {
		result.push({
			totalTimeInMinutes: yearDataItem.totalTimeInMinutes,
			name: `${dayjs([year, index, 1]).format(FULL_MONTH_FORMAT)}`,
			projects: yearDataItem.projects.map(
				(project: ChartStackedProjectItemResponse) => ({
					...project,
					hours: getHoursFromNumber(project.totalTime),
					minutes: getMinutesFromNumber(project.totalTime),
					hoursForBar:
						getHoursFromNumber(project.totalTime) +
						getMinutesFromNumber(project.totalTime) / 60,
				})
			),
		});
	});

	return result;
};

const getStackedQuarterStats = (
	data: ChartStackedProjectResponse[],
	year: number,
	quarter: string
): ChartStackedProjectAdapted[] => {
	const quart = quarter
		.split('-')
		.map((pathOfQuarter) => months.findIndex((item) => item === pathOfQuarter));

	const result: ChartStackedProjectAdapted[] = [];

	data.forEach((quarterDataItem, index) => {
		result.push({
			totalTimeInMinutes: quarterDataItem.totalTimeInMinutes,
			name: `${dayjs([year, quart[0] + index, 1]).format(FULL_MONTH_FORMAT)}`,
			projects: quarterDataItem.projects.map(
				(project: ChartStackedProjectItemResponse) => ({
					...project,
					hours: getHoursFromNumber(project.totalTime),
					minutes: getMinutesFromNumber(project.totalTime),
					hoursForBar:
						getHoursFromNumber(project.totalTime) +
						getMinutesFromNumber(project.totalTime) / 60,
				})
			),
		});
	});

	return result;
};

export const getStackedDataByPeriod = (
	{ year, month, quarter }: Period,
	reportsData: ChartStackedResponse
) => {
	if (quarter) {
		return getStackedQuarterStats(reportsData?.timePerPeriod, year, quarter);
	}

	if (month) {
		return getStackedMonthlyStats(reportsData?.timePerPeriod, year, month);
	}

	return getStackedYearStats(reportsData?.timePerPeriod, year);
};
