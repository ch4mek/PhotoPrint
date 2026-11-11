import {
	ManagementMainStatisticsCache,
	ManagementColumnStatisticsCache,
	ManagementStatisticsFlags,
	GetManagementStatisticsFlags,
	ManagementStatisticsResponseParams,
	ManagementStatisticsResponse,
} from '../types';

export const getManagementStatisticsFlags = ({
	year,
	month,
	filtersKey,
	mainStatisticsCache,
	columnStatisticsCache,
}: GetManagementStatisticsFlags): ManagementStatisticsFlags => ({
	shouldCalculateMainStatistics:
		!mainStatisticsCache ||
		mainStatisticsCache.year !== year ||
		mainStatisticsCache.month !== month,
	shouldCalculateColumnStatistics:
		!columnStatisticsCache ||
		columnStatisticsCache.year !== year ||
		columnStatisticsCache.month !== month ||
		columnStatisticsCache.filtersKey !== filtersKey,
});

export const mergeManagementStatisticsResponse = ({
	data,
	year,
	month,
	filtersKey,
	shouldCalculateMainStatistics,
	shouldCalculateColumnStatistics,
	mainStatisticsCache,
	columnStatisticsCache,
}: ManagementStatisticsResponseParams): ManagementStatisticsResponse => {
	const nextMainStatisticsCache: ManagementMainStatisticsCache =
		shouldCalculateMainStatistics
			? {
					year,
					month,
					expenses: data.expenses,
					profit: data.profit,
			  }
			: mainStatisticsCache ?? {
					year,
					month,
					expenses: data.expenses,
					profit: data.profit,
			  };

	const nextColumnStatisticsCache: ManagementColumnStatisticsCache =
		shouldCalculateColumnStatistics
			? {
					year,
					month,
					filtersKey,
					incomeBynAmount: data.incomeBynAmount,
					salaryAmount: data.salaryAmount,
					averageKpi: data.averageKpi,
					averageKpiColor: data.averageKpiColor,
			  }
			: columnStatisticsCache ?? {
					year,
					month,
					filtersKey,
					incomeBynAmount: data.incomeBynAmount,
					salaryAmount: data.salaryAmount,
					averageKpi: data.averageKpi,
					averageKpiColor: data.averageKpiColor,
			  };

	return {
		response: {
			...data,
			expenses: shouldCalculateMainStatistics
				? data.expenses
				: data.expenses ?? nextMainStatisticsCache.expenses,
			profit: shouldCalculateMainStatistics
				? data.profit
				: data.profit ?? nextMainStatisticsCache.profit,
			incomeBynAmount: shouldCalculateColumnStatistics
				? data.incomeBynAmount
				: data.incomeBynAmount ?? nextColumnStatisticsCache.incomeBynAmount,
			salaryAmount: shouldCalculateColumnStatistics
				? data.salaryAmount
				: data.salaryAmount ?? nextColumnStatisticsCache.salaryAmount,
			averageKpi: shouldCalculateColumnStatistics
				? data.averageKpi
				: data.averageKpi ?? nextColumnStatisticsCache.averageKpi,
			averageKpiColor: shouldCalculateColumnStatistics
				? data.averageKpiColor
				: data.averageKpiColor ?? nextColumnStatisticsCache.averageKpiColor,
		},
		mainStatisticsCache: nextMainStatisticsCache,
		columnStatisticsCache: nextColumnStatisticsCache,
	};
};
