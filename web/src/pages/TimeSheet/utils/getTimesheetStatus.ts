import { TFunction } from 'i18next';

import { statusKeys } from '../constants';
import { TimesheetStatusType } from '../types';

export const getTimesheetStatus = (
	status: TimesheetStatusType,
	t: TFunction<'timesheet', 'status'>
) => t(statusKeys[status], { keyPrefix: 'status' });
