// eslint-disable-next-line import/no-extraneous-dependencies
import { BasePlacement, VariationPlacement } from '@popperjs/core';

import { TimeSheetRow } from './types';

export const TOTAL_ROW_ID = 'total';
export const EMPTY_ROW_ID = 'empty';
export const ADD_ROW_ID = 'add';

export const TASK_INFO_COLUMN_KEY = 'taskInfoColumn';
export const TOTAL_COLUMN_KEY = 'total';

export const NORMAL_WORK_DAY_IN_MINUTES = 480;
export const TIMESHEET_CLOSED_ROW_HEIGHT = 44;
export const TIMESHEET_EXPANDED_ROW_HEIGHT = 144;
export const TIMESHEET_MANAGER_ROW_HEIGHT_MOBILE = 62;
export const TOOLTIP_DELAY = 300;

export const DEFAULT_TOTAL_TIME = '00:00';
export const TIMESHEET_DATE_FORMAT = 'DD.MM.YYYY';

export const TOTAL_ROW_INDEX = 0;

export const TIMESHEET_CACHE_KEY = 'timesheet';

export const emptyRow: Omit<TimeSheetRow, 'isEditingForbiddenForPeriod'> = {
	rowId: EMPTY_ROW_ID,
	projectId: 0,
	projectName: '',
	taskName: '',
	previousTaskName: '',
	timeMap: new Map(),
	errorMessage: undefined,
	isValidTaskName: true,
	isTaskInfoEditable: true,
	areTimeCellsEditable: true,
	isProjectBillable: false,
	isProjectPrivate: false,
	isDoubleTimeCell: false,
	userHasAccessToProject: true,
	isProjectActive: true,
	areTaskInfoFieldsTouched: false,
	isMobileView: false,
	disabledDates: [],
	billabilityDays: [],
};

export const addRow: Omit<TimeSheetRow, 'isEditingForbiddenForPeriod'> = {
	rowId: ADD_ROW_ID,
	projectId: 0,
	projectName: ADD_ROW_ID,
	taskName: '',
	previousTaskName: '',
	timeMap: new Map(),
	errorMessage: undefined,
	isValidTaskName: true,
	isTaskInfoEditable: false,
	areTimeCellsEditable: false,
	isProjectBillable: false,
	isProjectPrivate: false,
	isDoubleTimeCell: false,
	userHasAccessToProject: false,
	isProjectActive: false,
	isMobileView: false,
	disabledDates: [],
	billabilityDays: [],
};

export const addButtonTooltipFallbackPlacements: Array<
	BasePlacement | VariationPlacement
> = ['top-start', 'right'];
