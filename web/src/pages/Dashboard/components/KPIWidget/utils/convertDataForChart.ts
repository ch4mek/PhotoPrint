import { KPIWidgetData } from '../types';

import { formatMonthShort } from '@/helpers/formatRuDays';
import dayjs from '@/lib/dayjs';
import { TotalMonth } from '@/pages/Payload/types';

export const convertDataForChart = (
	data: TotalMonth[],
	locale: string
): KPIWidgetData[] => {
	const currentMonth = dayjs().month() + 1;
	const year = dayjs().year();

	return data.map(
		({
			monthOfYear,
			totalActualFte,
			totalPlannedFte,
			totalKpi,
			usersCount,
			color,
		}) => ({
			monthName: formatMonthShort(new Date(year, monthOfYear - 1), locale),
			monthOfYear,
			totalActualFte: monthOfYear <= currentMonth ? totalActualFte : null,
			totalPlannedFte: monthOfYear <= currentMonth ? totalPlannedFte : null,
			totalKpi: monthOfYear <= currentMonth ? totalKpi : null,
			users: monthOfYear <= currentMonth ? usersCount : null,
			percent:
				monthOfYear <= currentMonth && totalKpi !== null
					? Math.round(totalKpi)
					: null,
			color,
		})
	);
};
