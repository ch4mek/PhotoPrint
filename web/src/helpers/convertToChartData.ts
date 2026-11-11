import { PieChartResponseFull } from '@/types/pieChart';

import { getFullName } from './formatName';

export const convertToChartData = (arr: PieChartResponseFull[] | undefined) =>
	arr?.map(({ name, firstName, lastName, totalTimeInMinutes, color }) => ({
		name: name?.trim() ? name : getFullName(firstName, lastName),
		value: totalTimeInMinutes,
		color,
	})) || [];
