import { TFunction } from 'i18next';
import groupBy from 'lodash/groupBy';

import {
	ADD_TASK_CELL_CLASS_NAME,
	TASK_INFO_CELL_CLASS_NAME,
	TOTAL_COLUMN_CELL_CLASS_NAME,
	TOTAL_ROW_CELL_CLASS_NAME,
} from '../../constants';
import { TimesheetStatusType } from '../../types';
import { getTimesheetColumnClassname } from '../../utils/getTimesheetColumnClassname';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_ROW_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '../TimeSheeetManager/constants';

import {
	GRID_ROW_WITH_ADD_BUTTON_HEIGHT,
	GRID_ROW_WITH_ADD_BUTTON_HEIGHT_NARROW_VIEWPORT,
	GRID_TIME_CELL_ROW_HEIGHT,
} from '@/components/Grid/constants';
import { DateFormat } from '@/constants/date';
import dayjs from '@/lib/dayjs';
import { TimesheetData } from '@/queries/timesheet/types';
import { ProjectType } from '@/types/project';

import {
	ADD_ROW_ID,
	TIMESHEET_CLOSED_ROW_HEIGHT,
	TIMESHEET_DATE_FORMAT,
	TIMESHEET_EXPANDED_ROW_HEIGHT,
	TOTAL_ROW_ID,
	addRow,
	emptyRow,
} from './constants';
import { ProjectOption, TimeSheetRow, TimeTrackItem } from './types';

export function isDateBeforePreviousMonth(dateStringDDMMYYYY: string): boolean {
	const inputDate = dayjs(dateStringDDMMYYYY, TIMESHEET_DATE_FORMAT).toDate();
	const today = new Date();

	const lastDayOfMonthBeforePrevious = new Date(
		today.getFullYear(),
		today.getMonth() - 1,
		0
	);

	return inputDate <= lastDayOfMonthBeforePrevious;
}

export function isDateInPreviousMonth(dateStringDDMMYYYY: string): boolean {
	const date = dayjs(dateStringDDMMYYYY, TIMESHEET_DATE_FORMAT).toDate();

	const inputDate = dayjs(date);

	const previousMonth = dayjs().subtract(1, 'months').startOf('month');

	const startOfPreviousMonth = previousMonth.clone().startOf('month');
	const endOfPreviousMonth = previousMonth.clone().endOf('month');

	return (
		inputDate.isBetween(startOfPreviousMonth, endOfPreviousMonth) ||
		inputDate.isSame(startOfPreviousMonth)
	);
}

export const getYYYYMMDDFormattedDateFromString = (dateString: string) =>
	dayjs(`${dateString}T00:00:00`, 'DD.MM.YYYYTHH:mm:ss')
		.utc(true)
		.format('YYYY-MM-DD');

function isDateEqualTodayMonthAndYear(dateStringDDMMYYYY: string): boolean {
	const inputDate = dayjs(dateStringDDMMYYYY, TIMESHEET_DATE_FORMAT).toDate();
	const today = new Date();

	return (
		inputDate.getFullYear() === today.getFullYear() &&
		inputDate.getMonth() === today.getMonth()
	);
}

const checkIsTaskInfoEditable = (
	isEngineer: boolean,
	userHasAccessToProject: boolean,
	isProjectActive: boolean,
	isEditingForbiddenForPeriod: boolean,
	periodFirstTrackedDayKey: string
) => {
	if (!isProjectActive || isEditingForbiddenForPeriod) {
		return false;
	}

	if (
		!userHasAccessToProject &&
		!isDateEqualTodayMonthAndYear(
			dayjs(periodFirstTrackedDayKey).format(TIMESHEET_DATE_FORMAT)
		)
	) {
		return false;
	}

	return true;
};

export const createInitialNewTaskRow = (
	isEditingForbiddenForPeriod: boolean,
	isMobileView: boolean,
	isDoubleTimeCell: boolean,
	index: number
): TimeSheetRow => ({
	...structuredClone(emptyRow),
	isMobileView,
	isEditingForbiddenForPeriod,
	isDoubleTimeCell,
	orderIndex: index,
});

export const createInitialAddTaskRow = (
	isEditingForbiddenForPeriod: boolean
): TimeSheetRow => ({
	...structuredClone(addRow),
	isEditingForbiddenForPeriod,
});

export const convertResponseToTimesheetRows = (
	data: Array<TimesheetData>,
	userProjects: ProjectOption[],
	period: Date[],
	isEngineer: boolean,
	displayTotalRow: boolean,
	displayDoubleTotalRow: boolean,
	isDisabledTimesheet: boolean,
	isEditingForbiddenForPeriod: boolean,
	disableMessage: string | undefined,
	isMobileView: boolean
): Array<TimeSheetRow> => {
	const group = groupBy(data, (dataItem) => {
		if (typeof dataItem !== 'number') {
			const { taskName, projectId } = dataItem;

			return `${projectId}/${taskName}`;
		}

		return dataItem;
	});

	const disableHoverMessage =
		(isDisabledTimesheet && disableMessage) || undefined;

	const result: Array<TimeSheetRow> = Object.entries(group).map(
		([key, value], index) => {
			const {
				projectName,
				projectId,
				taskName,
				reportForDate: firstTaskReportForDate,
				btm: firstBillableTime,
				isProjectActive,
			} = value[0];

			const timeMap = new Map(
				value.map((trackItem) => {
					const {
						id,
						ftm: factTrackedInMinutes,
						reportForDate,
						btm: billableTrackedInMinutes,
					} = trackItem;

					const dateStringRepresentation = dayjs(reportForDate).format(
						TIMESHEET_DATE_FORMAT
					);

					const timeTrackItem: TimeTrackItem = {
						trackId: id,
						factTrackedInMinutes,
						billableTrackedInMinutes,
						reportForDate,
						isLoading: {
							fact: false,
							billable: false,
						},
					};

					return [dateStringRepresentation, timeTrackItem];
				})
			);

			const project = userProjects.find(({ id }) => id === projectId);

			const userHasAccessToProject = Boolean(project);

			const areTimeCellsEditable =
				isProjectActive && (isEngineer ? userHasAccessToProject : true);

			const entireRowHoverMessage = disableHoverMessage;

			const isDoubleTimeCell = isEngineer
				? project?.isCurrentUserAbleToSeeBillableTrackedTime ||
				  firstBillableTime !== null
				: true;

			return {
				rowId: key,
				projectId,
				projectName,
				taskName,
				previousTaskName: taskName,
				timeMap,
				errorMessage: undefined,
				isValidTaskName: true,
				isTaskInfoEditable: checkIsTaskInfoEditable(
					isEngineer,
					userHasAccessToProject,
					isProjectActive,
					isEditingForbiddenForPeriod,
					firstTaskReportForDate
				),
				areTimeCellsEditable,
				entireRowHoverMessage,
				isProjectPrivate: project?.type === ProjectType.Private,
				isProjectBillable: project?.isBillable ?? false,
				isDoubleTimeCell,
				userHasAccessToProject,
				isProjectActive,
				isEditingForbiddenForPeriod,
				isMobileView,
				index: index + 1,
				disabledDates: project?.disabledDates || [],
				billabilityDays: project?.billabilityDays || [],
			};
		}
	);

	const totalTimeMap = new Map(
		period.map((date) => {
			const dateStringRepresentation = dayjs(date).format(
				TIMESHEET_DATE_FORMAT
			);

			const totalTimeInMinutes = result.reduce(
				(acc, row) => {
					const allTrackedTimesInMinutes = row.timeMap.get(
						dateStringRepresentation
					);

					if (!allTrackedTimesInMinutes) {
						return acc;
					}

					const { factTrackedInMinutes, billableTrackedInMinutes } =
						allTrackedTimesInMinutes;
					const newTotalTime = [...acc];

					if (factTrackedInMinutes) {
						newTotalTime[0] += factTrackedInMinutes;
					}

					if (billableTrackedInMinutes) {
						newTotalTime[1] += billableTrackedInMinutes;
					}

					return newTotalTime;
				},
				[0, 0]
			);

			const timeTrackItem: TimeTrackItem = {
				trackId: 0,
				factTrackedInMinutes: totalTimeInMinutes[0],
				billableTrackedInMinutes: totalTimeInMinutes[1],
				reportForDate: dateStringRepresentation,
				isLoading: {
					fact: false,
					billable: false,
				},
			};

			return [dateStringRepresentation, timeTrackItem];
		})
	);

	const totalRow: TimeSheetRow = {
		rowId: TOTAL_ROW_ID,
		projectId: 0,
		projectName: TOTAL_ROW_ID,
		taskName: TOTAL_ROW_ID,
		previousTaskName: TOTAL_ROW_ID,
		timeMap: totalTimeMap,
		displayTotalRow,
		errorMessage: undefined,
		isValidTaskName: true,
		isTaskInfoEditable: false,
		areTimeCellsEditable: false,
		isProjectPrivate: false,
		isProjectBillable: false,
		isDoubleTimeCell: displayDoubleTotalRow,
		userHasAccessToProject: false,
		isProjectActive: false,
		isEditingForbiddenForPeriod,
		isMobileView,
		disabledDates: [],
		billabilityDays: [],
	};

	return [
		totalRow,
		...result,
		{
			...createInitialNewTaskRow(
				isEditingForbiddenForPeriod,
				isMobileView,
				!isEngineer,
				result.length + 2
			),
			isTaskInfoEditable: !isEditingForbiddenForPeriod,
			entireRowHoverMessage: disableHoverMessage,
			isMobileView,
		},
		{ ...createInitialAddTaskRow(isEditingForbiddenForPeriod) },
	];
};

export const getTotalTimeByDay = (rows: Array<TimeSheetRow>, dayKey: string) =>
	rows.reduce(
		(acc, mapRow) => {
			if (mapRow.rowId === TOTAL_ROW_ID || mapRow.rowId === ADD_ROW_ID) {
				return acc;
			}

			const allTrackedTimesInMinutes = mapRow.timeMap.get(dayKey);

			if (!allTrackedTimesInMinutes) {
				return acc;
			}

			const { factTrackedInMinutes, billableTrackedInMinutes } =
				allTrackedTimesInMinutes;
			const totalTime = [...acc];

			if (factTrackedInMinutes) {
				totalTime[0] += factTrackedInMinutes;
			}

			if (billableTrackedInMinutes) {
				totalTime[1] += billableTrackedInMinutes;
			}

			return totalTime;
		},
		[0, 0]
	);

export const getRowsWithUpdatedRowTime = (
	rows: Array<TimeSheetRow>,
	rowIndex: number,
	dayKey: string,
	newFactTime: number | null,
	newBillableTime: number | null,
	isLoading?: {
		fact?: boolean;
		billable?: boolean;
	}
) => {
	const row = rows[rowIndex];
	const timeMap = row.timeMap;
	const trackItem = timeMap.get(dayKey);

	const factTrackedInMinutes =
		newFactTime ?? trackItem?.factTrackedInMinutes ?? 0;
	const billableTrackedInMinutes =
		newBillableTime ?? trackItem?.billableTrackedInMinutes ?? 0;

	const newTrackItem: TimeTrackItem = trackItem
		? {
				...trackItem,
				factTrackedInMinutes,
				billableTrackedInMinutes,
				isLoading: {
					...trackItem.isLoading,
					...isLoading,
				},
		  }
		: {
				trackId: 0,
				reportForDate: getYYYYMMDDFormattedDateFromString(dayKey),
				factTrackedInMinutes,
				billableTrackedInMinutes,
				isLoading: {
					fact: false,
					billable: false,
					...isLoading,
				},
		  };

	const newTimeMap = new Map(timeMap);

	if (newTrackItem) {
		newTimeMap.set(dayKey, newTrackItem);
	} else {
		newTimeMap.delete(dayKey);
	}

	const rowsCopy = [...rows];

	rowsCopy[rowIndex] = {
		...row,
		timeMap: newTimeMap,
	};

	return rowsCopy;
};

export const getRowsWithUpdatedTrackItem = (
	rows: Array<TimeSheetRow>,
	rowIndex: number,
	dayKey: string,
	newTrackItem: undefined | TimeTrackItem
) => {
	const row = rows[rowIndex];
	const timeMap = row.timeMap;

	const newTimeMap = new Map(timeMap);

	if (newTrackItem) {
		newTimeMap.set(dayKey, newTrackItem);
	} else {
		newTimeMap.delete(dayKey);
	}

	const rowsCopy = [...rows];
	rowsCopy[rowIndex].timeMap = newTimeMap;

	return rowsCopy;
};

export const getDataGridRowsWithUpdatedAddRow = (
	updatedRow: TimeSheetRow,
	allDataGridRows: TimeSheetRow[]
) => {
	const rowsCopy = [...allDataGridRows];
	const addRowId = rowsCopy[rowsCopy.length - 1].rowId;
	rowsCopy[rowsCopy.length - 1] = {
		...updatedRow,
		rowId: addRowId,
	};

	return rowsCopy;
};

export const getDynamicRowHeight = (
	row: TimeSheetRow,
	isMobileView: boolean,
	displayTotalRow: boolean,
	isNarrowViewport: boolean,
	isPeriodByMonth: boolean
) => {
	const DEFAULT_ROW_HEIGHT = isMobileView
		? TIMESHEET_EXPANDED_ROW_HEIGHT
		: GRID_TIME_CELL_ROW_HEIGHT;

	if (row.rowId === 'total') {
		return displayTotalRow ? TIMESHEET_CLOSED_ROW_HEIGHT : 0;
	}

	if (row.rowId === ADD_ROW_ID) {
		return isNarrowViewport && !isMobileView && isPeriodByMonth
			? GRID_ROW_WITH_ADD_BUTTON_HEIGHT_NARROW_VIEWPORT
			: GRID_ROW_WITH_ADD_BUTTON_HEIGHT;
	}

	return DEFAULT_ROW_HEIGHT;
};

export const getTimeCellHoverMessage = (
	isEngineer: boolean,
	dayKey: string,
	userHasAccessToProject: boolean,
	isProjectActive: boolean,
	isBlockedDay: boolean,
	tCommon: TFunction<'common', 'tooltips'>,
	entireRowHoverMessage?: string
) => {
	if (isEngineer) {
		if (!userHasAccessToProject && isProjectActive) {
			return tCommon('noAccess');
		}

		if (isBlockedDay) {
			return tCommon('forbiddenToEdit');
		}

		if (!isProjectActive) {
			return tCommon('inactiveProject');
		}

		return undefined;
	}

	if (entireRowHoverMessage) {
		return entireRowHoverMessage;
	}

	if (!isProjectActive) {
		return tCommon('inactiveProject');
	}

	if (!userHasAccessToProject && !isDateEqualTodayMonthAndYear(dayKey)) {
		return tCommon('noAccess');
	}

	return undefined;
};

export const isStatusEditable = (status?: TimesheetStatusType | null) =>
	status === null || status === TimesheetStatusType.Rejected;

export const isDayBlocked = (blockedDays: string[], day: string) => {
	const dayDayjs = dayjs(day, DateFormat.DD_MM_YYYY);
	const dayDayjsFormatted = dayDayjs.format(DateFormat.YYYY_MM_DD);

	return blockedDays.includes(dayDayjsFormatted);
};

export const isDayDisabledByProjectAssignDates = (
	dayKey: string,
	disabledDates: string[]
) => {
	const convertedDay = getYYYYMMDDFormattedDateFromString(dayKey);

	return disabledDates.some((date) => date === convertedDay);
};

export const getEngineerCellClassname = (index: number, rowId: string) =>
	[
		getTimesheetColumnClassname(index),
		...(rowId === TOTAL_ROW_ID ? [TOTAL_ROW_CELL_CLASS_NAME] : []),
		...(rowId !== TOTAL_ROW_ID && rowId !== ADD_ROW_ID
			? [HOVER_ROW_CELL_CLASS_NAME]
			: []),
	].join(' ');

export const getTaskInfoColumnClassname = (rowId: string) => {
	if (rowId === TOTAL_ROW_ID) {
		return `${TOTAL_ROW_CELL_CLASS_NAME} ${TASK_INFO_CELL_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`;
	}

	if (rowId === ADD_ROW_ID) {
		return `${ADD_TASK_CELL_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`;
	}

	return `${TASK_INFO_CELL_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME} ${HOVER_ROW_CELL_CLASS_NAME}`;
};

export const getTotalColumnClassname = (rowId: string) =>
	[
		HOVER_TOTAL_CELL_CLASS_NAME,
		TOTAL_COLUMN_CELL_CLASS_NAME,
		...(rowId === TOTAL_ROW_ID ? [TOTAL_ROW_CELL_CLASS_NAME] : []),
		...(rowId !== TOTAL_ROW_ID && rowId !== ADD_ROW_ID
			? [HOVER_ROW_CELL_CLASS_NAME]
			: []),
	].join(' ');
