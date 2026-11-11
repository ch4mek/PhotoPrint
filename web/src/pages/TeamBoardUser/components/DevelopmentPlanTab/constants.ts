import { fromToDateObject } from '@/constants/defaultValues';
import { DevelopmentPlanFilters } from '@/types/developmentPlan';

export const defaultDevelopmentPlanFilterValues: DevelopmentPlanFilters = {
	startDate: fromToDateObject,
	endDate: fromToDateObject,
	deadlineDate: fromToDateObject,
	summary: '',
	status: [],
	notes: '',
};
