import { YYYY_MM_DD_DAYJS_DATE_FORMAT } from '@/constants/date';
import dayjs from '@/lib/dayjs';

export const formatDevelopmentPlanDates = (
	startDate: Date | null,
	deadlineDate: Date | null
) => ({
	startDate: dayjs(startDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT),
	deadlineDate: deadlineDate
		? dayjs(deadlineDate).format(YYYY_MM_DD_DAYJS_DATE_FORMAT)
		: null,
});
