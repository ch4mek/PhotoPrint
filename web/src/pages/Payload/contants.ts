import { lightPalette } from '@mms/mms-ui-library/theme';

import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { PERIOD } from '@/constants/date';
import { getMonthColumnWidths } from '@/helpers/getMonthColumnWidths';

export const START_MONTH_COLUMN_INDEX = 8;
export const HEIGHT_CELL = 44;
export const SUMMARY_ROW_HEIGHT = 75;
export const GRID_SUBROW_HEIGHT = 26;
export const HEIGHT_TIME_CELL = 24;
export const SUMMARY_ROW_BOX_SHADOW_HEIGHT = 2;

export const COLUMNS_COUNT = 22;
export const MAX_PROJECTS_FITS = 2;
export const FIRST_PROJECTS_ROW_HEIGHT = 20;
export const PROJECTS_ROW_HEIGHT = 18;
export const PAGE_SIZE = 25;
export const INITIAL_SPECIALIZATION_TITLE = 'All Specializations';
export const PAYLOAD_BORDER_WIDTH = 2;
export const PAYLOAD_FREEZE_COLUMNS_AMOUNT = 5;
export const PAYLOAD_FREEZE_BREAKPOINT = 1107;
export const PAYLOAD_MOBILE_BREAKPOINT = 879;

export const PROJECT_SUBROW_CLASS_NAME = 'project-subrow-item';
export const WORKLOAD_SUBROW_CLASS_NAME = 'workload-subrow-item';
export const PROJECT_SUBROW_HOVERED_CLASS_NAME = 'project-subrow-item-hovered';
export const WORKLOAD_SUBROW_HOVERED_CLASS_NAME =
	'workload-subrow-item-hovered';
export const WORKLOAD_SUBROW_LAST_CELL_CLASS_NAME =
	'workload-subrow-item-hovered-last-cell';

export const MINIMAL_HARD_SKILLS_COUNT_FOR_SPECIALIZATION = 1;

export const FILTERS_FIELD_NAME: Record<string, string> = {
	id: 'UserId',
	employee: 'EmployeeFullName',
};

export const TOTAL_ROW_ID = -9999;

export const NBSP = '\u00A0';

export const DEPARTMENT_CELL_CHILD_WIDTH = '95%';
export const DEFAULT_CIRCLE_COLOR_PICKER_COLOR =
	lightPalette['graphics-light-gray'];

export const NO_ACTIVE_ASSIGNMENTS_TITLE = 'No active assignments';

export enum MonthCellColors {
	Red = '#EDB6B0',
	Green = '#BAE5BE',
	Blue = '#BBD0F8',
	Orange = '#FFE0A7',
}

export const idSettings = {
	id: {
		width: 67,
		fixed: true,
	},
};

export const departmentSettings = {
	department: {
		width: 65,
		fixed: true,
	},
};

export const groupSettings = {
	group: {
		width: 75,
		fixed: true,
	},
};

export const anotherSettings = {
	employee: {
		width: 200,
		fixed: true,
	},
	comments: {
		width: 150,
	},
	needProject: {
		width: 57,
		fixed: true,
	},
	projects: {
		width: 180,
	},
	...getMonthColumnWidths(PERIOD, 88),
	plan: {
		width: 200,
		maxWidth: 300,
	},
};

export const PAYLOAD_COLUMNS_WIDTH = {
	...idSettings,
	...departmentSettings,
	...groupSettings,
	...anotherSettings,
};

export const LOGIC_OPTIONS: Array<SelectionModalOption> = [
	{
		id: 'Yes',
		description: 'Yes',
		disabled: false,
	},
	{
		id: 'No',
		description: 'No',
		disabled: false,
	},
];
export const PAYLOAD_GRID_COLUMNS = {
	payloadId: {
		key: 'id',
		name: 'Id',
		sortable: true,
	},
	payloadDepartment: {
		key: 'department',
		name: 'Dep.',
	},
	payloadGroup: {
		key: 'group',
		name: 'Group',
	},
	payloadEmployee: {
		key: 'employee',
		name: 'Employee',
		sortable: true,
	},
	payloadComment: {
		key: 'comments',
		name: 'Comment',
	},
	payloadProject: {
		key: 'projects',
		name: 'Projects',
	},
	payloadNp: {
		key: 'needProject',
		name: 'NP',
		sortable: false,
	},
	...PERIOD.map((month) => ({
		key: month.toLowerCase(),
		name: month,
		sortable: false,
	})),
	payloadPlan: {
		key: 'plan',
		name: 'Plan',
	},
};

export enum BoundaryValuesForColorCalculation {
	Max = 100,
	Medium = 50,
}
