import type { ChartDataItem } from '../types';

export const extendChartData = (chartData: ChartDataItem[]) =>
	chartData?.map((dataObj) => ({
		...dataObj,
		hoursForBar: dataObj.hours + dataObj.minutes / 60,
	}));
