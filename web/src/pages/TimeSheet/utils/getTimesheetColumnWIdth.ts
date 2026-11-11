import {
	TIMESHEET_TOTAL_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE,
} from '../constants';

export const getMinimalRequiredGridLength = (
	periodLength: number,
	minimalColumnWidth: number,
	isMobileView: boolean
) =>
	(isMobileView
		? TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE
		: TIMESHEET_USER_NAME_COLUMN_WIDTH) +
	TIMESHEET_TOTAL_COLUMN_WIDTH +
	periodLength * minimalColumnWidth;

export const getColumnWidthFromGridOffsetWidth = (
	gridOffsetWidth: number,
	periodLength: number,
	isMobileView: boolean
) =>
	(gridOffsetWidth -
		(isMobileView
			? TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE
			: TIMESHEET_USER_NAME_COLUMN_WIDTH) -
		TIMESHEET_TOTAL_COLUMN_WIDTH) /
	periodLength;
