import { formatDateString } from '@/helpers/dateToISOString';
import {
	DevelopmentPlanStatus,
	UserDevelopmentPlan,
} from '@/types/developmentPlan';

import { DevelopmentPlanSectionRow } from './types';

export const convertDevelopmentPlanResponseToRows = (
	rows: Array<UserDevelopmentPlan>
): Array<DevelopmentPlanSectionRow> =>
	rows.map(({ startDate, deadlineDate, endDate, status, ...rest }) => ({
		...rest,
		startDate: formatDateString(startDate, ''),
		deadlineDate: formatDateString(deadlineDate, ''),
		endDate: formatDateString(endDate, ''),
		status: DevelopmentPlanStatus[status],
	}));
