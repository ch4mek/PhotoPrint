import { CalendarColumns } from './types';

export const MINIMAL_DATE_COLUMN_WIDTH = 33.8;
export const CALENDAR_GRID_ROW_HEIGHT = 40;
export const GRID_SCROLLBAR_WIDTH = 8;

export const getAbsenceCalendarColumns = (
	isDesktopWidth: boolean
): CalendarColumns => ({
	userFullName: {
		width: isDesktopWidth ? 260 : 180,
		fixed: true,
		key: 'userFullName',
		name: 'User',
	},
	department: {
		width: isDesktopWidth ? 119 : 65,
		fixed: true,
		key: 'department',
		name: isDesktopWidth ? 'Department' : 'Dep.',
	},
});

export const CALENDAR_DAY_HEADER_CLASSNAME = 'calendar-day-header-cell';
export const CALENDAR_ITEM_CLASSNAME = 'calendar-item';

export const INITIAL_ORDER_BY_VALUE = 'userFullName';
