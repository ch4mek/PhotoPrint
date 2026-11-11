import { IncludedDepartments } from '@/pages/Dashboard/types';

import { Departments } from './departments';

export const OVERDUE_DAYS_COUNT = 7;
export const UPPER_DAYS_BORDER = 30;

export const DASHBOARD_WIDGET_HEIGHT = 375;
export const MIN_FULLNAME_COLUMN_WIDTH = 100;

export const includedDepartments: IncludedDepartments[] = [
	Departments.Js,
	Departments.Net,
	Departments.Java,
	Departments.QA,
	Departments.DevOps,
	Departments.Mobile,
	Departments.AIML,
	Departments.SAP,
	Departments.Design,
	Departments.BA,
	Departments.PMO,
] as const;
