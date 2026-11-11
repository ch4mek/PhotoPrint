import { TimeSheetRow } from './types';

type TimesheetRowsKey = string | number;

const timesheetRowsMap = new Map<TimesheetRowsKey, Array<TimeSheetRow>>();

export const setTimesheetRows = (
	key: TimesheetRowsKey,
	timesheetRows: Array<TimeSheetRow>
) => timesheetRowsMap.set(key, timesheetRows);

export const removeTimesheetRows = (key: TimesheetRowsKey) =>
	timesheetRowsMap.delete(key);

export const getTimesheetRows = (key: TimesheetRowsKey) =>
	timesheetRowsMap.get(key);
