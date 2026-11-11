import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '../../TimeSheeetManager/constants';
import { renderTotalHeaderCell } from '../../TimeSheeetManager/GridRenderers/renderTotalHeadercell';
import {
	TASK_INFO_COLUMN_KEY,
	TIMESHEET_DATE_FORMAT,
	TOTAL_COLUMN_KEY,
} from '../constants';
import {
	renderMonthHeaderCell,
	renderWeekHeaderCell,
} from '../GridRenderers/renderDateHeaderCell';
import { renderTaskInfoCell } from '../GridRenderers/renderTaskInfoCell';
import { renderTaskInfoHeaderCell } from '../GridRenderers/renderTaskInfoHeaderCell';
import { renderTotalCell } from '../GridRenderers/renderTotalCell';
import { renderTrackCell } from '../GridRenderers/renderTrackCell';
import { TimeSheetRow } from '../types';
import {
	getTaskInfoColumnClassname,
	getTotalColumnClassname,
	getEngineerCellClassname,
} from '../utils';

import dayjs from '@/lib/dayjs';
import {
	getTotalColumnHoverIndex,
	MONTH_MINIMAL_TRACK_CELL_WIDTH,
	TASK_INFO_HEADER_CLASS_NAME,
	TIMESHEET_TOTAL_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH,
	TOTAL_COLUMN_HEADER_CLASS_NAME,
	USER_INFO_COLUMN_HOVER_INDEX,
	WEEK_MINIMAL_TRACK_CELL_WIDTH,
} from '@/pages/TimeSheet/constants';
import { getHoverColumnIndexByOrder } from '@/pages/TimeSheet/utils/getHoverColumnIndexByOrder';
import { getTimesheetColumnClassname } from '@/pages/TimeSheet/utils/getTimesheetColumnClassname';
import {
	getColumnWidthFromGridOffsetWidth,
	getMinimalRequiredGridLength,
} from '@/pages/TimeSheet/utils/getTimesheetColumnWIdth';
import { ColumnWithHoverIndex } from '@/types/columnHoverTypes';

const createTaskInfoColumn = (
	frozen: boolean,
	hideHeaderRow?: boolean
): Column<TimeSheetRow> =>
	({
		key: TASK_INFO_COLUMN_KEY,
		name: 'Task Info',
		renderCell: renderTaskInfoCell,
		renderHeaderCell: !hideHeaderRow ? renderTaskInfoHeaderCell : undefined,
		headerCellClass: `${TASK_INFO_HEADER_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`,
		cellClass: (row: TimeSheetRow) => getTaskInfoColumnClassname(row.rowId),
		width: TIMESHEET_USER_NAME_COLUMN_WIDTH,
		frozen,
		hoverIndex: USER_INFO_COLUMN_HOVER_INDEX,
	} as unknown as Column<TimeSheetRow>);

const createTotalColumn = (
	frozen: boolean,
	periodLength: number
): ColumnWithHoverIndex<TimeSheetRow> => ({
	key: TOTAL_COLUMN_KEY,
	name: 'Total',
	renderCell: renderTotalCell,
	headerCellClass: `${TOTAL_COLUMN_HEADER_CLASS_NAME} ${HOVER_TOTAL_CELL_CLASS_NAME}`,
	cellClass: (row: TimeSheetRow) => getTotalColumnClassname(row.rowId),
	width: TIMESHEET_TOTAL_COLUMN_WIDTH,
	frozen,
	hoverIndex: getTotalColumnHoverIndex(periodLength),
	renderHeaderCell: renderTotalHeaderCell,
});

export const useGenerateTimeSheetGridColumns = (
	period: Date[],
	gridOffsetWidth: number,
	isPeriodByMonth: boolean,
	freezeTaskInfoColumn: boolean,
	freezeTotalColumn: boolean,
	isMobileView: boolean,
	hideHeaderRow?: boolean
): Array<Column<TimeSheetRow>> => {
	const minimalColumnWidth = isPeriodByMonth
		? MONTH_MINIMAL_TRACK_CELL_WIDTH
		: WEEK_MINIMAL_TRACK_CELL_WIDTH;

	const columnWidth =
		getMinimalRequiredGridLength(
			period.length,
			minimalColumnWidth,
			isMobileView
		) > gridOffsetWidth
			? minimalColumnWidth
			: getColumnWidthFromGridOffsetWidth(
					gridOffsetWidth,
					period.length,
					isMobileView
			  );

	return useMemo(() => {
		const dateColumns: Array<ColumnWithHoverIndex<TimeSheetRow>> = period.map(
			(day, index) => {
				const dateStringRepresentation = dayjs(day).format(
					TIMESHEET_DATE_FORMAT
				);

				const headerCellRenderer = isPeriodByMonth
					? renderMonthHeaderCell
					: renderWeekHeaderCell;

				return {
					name: dateStringRepresentation,
					key: dateStringRepresentation,
					width: columnWidth,
					minWidth: isPeriodByMonth
						? MONTH_MINIMAL_TRACK_CELL_WIDTH
						: undefined,
					renderHeaderCell: !hideHeaderRow ? headerCellRenderer : undefined,
					headerCellClass: getTimesheetColumnClassname(index),
					renderCell: renderTrackCell,
					cellClass: (row) => getEngineerCellClassname(index, row.rowId),
					hoverIndex: getHoverColumnIndexByOrder(index - 1),
					isMobileView,
				};
			}
		);

		return [
			createTaskInfoColumn(freezeTaskInfoColumn, hideHeaderRow),
			...dateColumns,
			createTotalColumn(freezeTotalColumn, period.length),
		] as Array<Column<TimeSheetRow>>;
	}, [
		period,
		isPeriodByMonth,
		freezeTaskInfoColumn,
		freezeTotalColumn,
		isMobileView,
		hideHeaderRow,
	]);
};
