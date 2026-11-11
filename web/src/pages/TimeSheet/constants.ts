import {
	TimesheetModeFilterOption,
	TimesheetStatusLabelKey,
	TimesheetStatusType,
} from './types';

export const WEEK_MINIMAL_TRACK_CELL_WIDTH = 90;
export const MONTH_MINIMAL_TRACK_CELL_WIDTH = 80;

export const TIMESHEET_USER_NAME_COLUMN_WIDTH = 424;
export const TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE = 216;
export const TIMESHEET_TOTAL_COLUMN_WIDTH = 80;
export const TIMESHEET_FREEZE_COLUMN_BREAKPOINT = 786;
const TIMESHEET_TOTAL_PADDING = 20;
const visibleWeekColumnsCount = 3;
export const DAYS_COLUMNS_START_INDEX = 3;

export const ADD_TASK_CELL_CLASS_NAME = 'add-task-cell';
export const TASK_INFO_CELL_CLASS_NAME = 'engineer-task-info-cell';
export const TASK_INFO_HEADER_CLASS_NAME = 'engineer-task-info-header-cell';
export const TOTAL_COLUMN_HEADER_CLASS_NAME = 'engineer-total-header-cell';
export const TOTAL_COLUMN_CELL_CLASS_NAME = 'engineer-total-cell';

export const TOTAL_ROW_CELL_CLASS_NAME = 'engineer-total-row-cell';

export const TIMESHEET_FREEZE_COLUMNS_BREAKPOINT = Math.round(
	TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE +
		TIMESHEET_TOTAL_COLUMN_WIDTH +
		visibleWeekColumnsCount * WEEK_MINIMAL_TRACK_CELL_WIDTH -
		TIMESHEET_TOTAL_PADDING
);

export enum TimeSheetBreakpoints {
	DESKTOP = 1471,
	DESKTOP_MD = 1257,
	TABLET_LG = 820,
	TABLET = 640,
	MOBILE_LG = 500,
	MOBILE = 400,
}

export const USER_INFO_COLUMN_HOVER_INDEX = 1;
export const getTotalColumnHoverIndex = (periodLength: number) =>
	periodLength + 2;
export const CLOSE_DELAY_ROLE_FILTER_MS = 100;

export const timesheetModesIds = {
	departmentModeId: 'Department mode',
	projectsModeId: 'Projects mode',
	personalModeId: 'Personal mode',
	groupModeId: 'Group mode',
} as const;

const { projectsModeId, personalModeId, departmentModeId, groupModeId } =
	timesheetModesIds;

export const defaultInitialTimesheetMode: TimesheetModeFilterOption = {
	id: departmentModeId,
	fullName: departmentModeId,
};

export const defaultTimesheetModes: Array<TimesheetModeFilterOption> = [
	defaultInitialTimesheetMode,
	{ id: personalModeId, fullName: personalModeId },
];

export const timesheetModesForHeadProjectManager: Array<TimesheetModeFilterOption> =
	[
		defaultInitialTimesheetMode,
		{ id: projectsModeId, fullName: projectsModeId },
		{ id: personalModeId, fullName: personalModeId },
	];

export const timesheetModesForProjectManager: Array<TimesheetModeFilterOption> =
	[
		{ id: projectsModeId, fullName: projectsModeId },
		{ id: personalModeId, fullName: personalModeId },
	];

export const initialPMOEmployeeTimesheetMode =
	timesheetModesForProjectManager[0];

export const timesheetZIndexes = {
	fixedHeader: 7,
	header: 6,
	fixedCellWithPriority: 5,
	fixedCell: 4,
	cell: 3,
	taskCell: 3,
	activeCell: 2,
	hoverCell: 1,
};

export const timesheetModesForGroupManager: Array<TimesheetModeFilterOption> = [
	{ id: groupModeId, fullName: groupModeId },
	{ id: personalModeId, fullName: personalModeId },
];

export const initialGroupManagerTimesheetMode =
	timesheetModesForGroupManager[0];

export const statusKeys = {
	[TimesheetStatusType.UnderReview]: 'underReview',
	[TimesheetStatusType.Approved]: 'approved',
	[TimesheetStatusType.Rejected]: 'rejected',
} as const satisfies Record<TimesheetStatusType, TimesheetStatusLabelKey>;
