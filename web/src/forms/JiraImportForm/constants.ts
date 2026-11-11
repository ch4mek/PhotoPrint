import { TimesheetQueryKeys } from '@/constants/queriesKeys';

import { InitialDataValues } from './types';

export const POPUP_TITLE = 'Jira import';

export const initialData: InitialDataValues = {
	project: null,
	dateFrom: null,
	dateTo: null,
};

export const fieldsNames = {
	project: 'project',
	dateTo: 'dateTo',
	dateFrom: 'dateFrom',
};

export const timesheetQueryKeysToInvalidate = [
	TimesheetQueryKeys.userTimesheet,
	TimesheetQueryKeys.timesheetStatus,
	TimesheetQueryKeys.timesheetCalender,
] as const;
