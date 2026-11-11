import { DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { DevelopmentPlanRow } from '@/types/developmentPlan';

export const sortDevelopmentPlanRowsDateByDesc = (
	rows: DevelopmentPlanRow[]
): DevelopmentPlanRow[] =>
	rows.sort((a, b) => {
		const dateA = dayjs(a.startDate, DATE_FORMAT);
		const dateB = dayjs(b.startDate, DATE_FORMAT);

		return dateB.diff(dateA);
	});
