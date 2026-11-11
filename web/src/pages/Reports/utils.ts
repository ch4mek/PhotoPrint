import { TIME_PERIOD, months } from '@/constants/date';
import { RP_DATE_FILTER_STORAGE_KEY } from '@/constants/storageKeys';
import { generateInitialData } from '@/helpers/generateInitialData';
import { getItemFromStorage } from '@/helpers/storage';

import { Month } from './types';

export const getInitialDate = (userId: number) => {
	const editDateFilter: ReturnType<typeof generateInitialData> | null =
		getItemFromStorage(RP_DATE_FILTER_STORAGE_KEY, userId);

	return editDateFilter || generateInitialData(TIME_PERIOD.Month);
};

export const getInitialTimePeriodFilter = (userId: number) => {
	const editDateFilter: ReturnType<typeof generateInitialData> | null =
		getItemFromStorage(RP_DATE_FILTER_STORAGE_KEY, userId);

	if (editDateFilter) {
		const reportMonths = [];

		if (editDateFilter.quarter) {
			const [startMonth, endMonth] = editDateFilter.quarter.split('-');
			const startIndex = months.indexOf(startMonth as Month);
			const endIndex = months.indexOf(endMonth as Month);

			for (let i = startIndex; i <= endIndex; i += 1) {
				reportMonths.push(months[i]);
			}
		} else if (editDateFilter.month) {
			reportMonths.push(editDateFilter.month);
		}

		return {
			year: editDateFilter.year,
			months: reportMonths,
		};
	}

	const initialData = generateInitialData(TIME_PERIOD.Month);

	return {
		year: initialData.year,
		months: [initialData.month] as string[],
	};
};
