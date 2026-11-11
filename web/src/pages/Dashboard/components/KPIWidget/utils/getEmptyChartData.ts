import dayjs from 'dayjs';

import { CHART_DATA_KEYS } from '../constants';
import { KPIWidgetData } from '../types';

import { formatMonthShort } from '@/helpers/formatRuDays';

export const getEmptyChartData = (locale: string): KPIWidgetData[] => {
	const year = dayjs().year();

	return Array.from({ length: 12 }, (_, i) => ({
		[CHART_DATA_KEYS.monthName]: formatMonthShort(new Date(year, i), locale),
		[CHART_DATA_KEYS.monthOfYear]: i + 1,
		totalActualFte: null,
		totalPlannedFte: null,
		totalKpi: null,
		users: null,
		percent: null,
		color: '',
	}));
};
