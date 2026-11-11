import { useMemo } from 'react';
import { Column } from 'react-data-grid';
import { useTranslation } from 'react-i18next';

import {
	TIMESHEET_DATE_FORMAT,
	TOTAL_COLUMN_KEY,
} from '../../TimesheetEngineer/constants';
import {
	renderMonthHeaderCell,
	renderWeekHeaderCell,
} from '../../TimesheetEngineer/GridRenderers/renderDateHeaderCell';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
	MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME,
	USER_INFO_CELL_CLASS,
	USER_NAME_COLUMN_HEADER_CLASS_NAME,
} from '../constants';
import { renderNameHeader } from '../GridRenderers/renderNameHeader';
import { renderTotalHeaderCell } from '../GridRenderers/renderTotalHeadercell';
import { userTimesheetRenderer } from '../GridRenderers/userTimesheetRenderer';
import { TimeSheetManagerRow } from '../types';

import dayjs from '@/lib/dayjs';
import {
	MONTH_MINIMAL_TRACK_CELL_WIDTH,
	TIMESHEET_TOTAL_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH,
	TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE,
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

const getMinimalColumnWidth = (isMonthPeriod: boolean) =>
	isMonthPeriod
		? MONTH_MINIMAL_TRACK_CELL_WIDTH
		: WEEK_MINIMAL_TRACK_CELL_WIDTH;

const getUserTimesheetColumn = (
	isMobileView: boolean
): ColumnWithHoverIndex<TimeSheetManagerRow> => ({
	key: 'userName',
	name: 'Name',
	renderHeaderCell: renderNameHeader,
	renderCell: userTimesheetRenderer,
	width: isMobileView
		? TIMESHEET_USER_NAME_COLUMN_WIDTH_MOBILE
		: TIMESHEET_USER_NAME_COLUMN_WIDTH,
	colSpan(args) {
		if (args.type === 'ROW') {
			return args.row.periodLength + 2;
		}

		return undefined;
	},
	cellClass: USER_INFO_CELL_CLASS,
	headerCellClass: `${USER_NAME_COLUMN_HEADER_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`,
	hoverIndex: USER_INFO_COLUMN_HOVER_INDEX,
});

const createTotalColumn = (
	isFrozen: boolean,
	periodLenght: number
): ColumnWithHoverIndex<TimeSheetManagerRow> => ({
	key: TOTAL_COLUMN_KEY,
	name: 'Total',
	renderCell: () => null,
	renderHeaderCell: renderTotalHeaderCell,
	width: TIMESHEET_TOTAL_COLUMN_WIDTH,
	headerCellClass: `${MANAGER_TOTAL_COLUMN_HEADER_CLASS_NAME} ${HOVER_TOTAL_CELL_CLASS_NAME}`,
	cellClass: HOVER_TOTAL_CELL_CLASS_NAME,
	frozen: isFrozen,
	hoverIndex: periodLenght + 1,
});

export const useTimeSheetManagerColumns = (
	period: Date[],
	isPeriodByMonth: boolean,
	gridOffsetWidth: number,
	areColumnsFrozen: boolean,
	isMobileView: boolean
): Array<Column<TimeSheetManagerRow>> => {
	const minimalColumnWidth = getMinimalColumnWidth(isPeriodByMonth);

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
	const { t } = useTranslation('timesheet');
	const columns = useMemo(() => {
		const dateColumns: Array<ColumnWithHoverIndex<TimeSheetManagerRow>> =
			period.map((day, index) => {
				const dateStringRepresentation = dayjs(day).format(
					TIMESHEET_DATE_FORMAT
				);

				return {
					name: dateStringRepresentation,
					key: dateStringRepresentation,
					renderHeaderCell: isPeriodByMonth
						? renderMonthHeaderCell
						: renderWeekHeaderCell,
					renderCell: () => null,
					width: columnWidth > 0 ? columnWidth : undefined,
					hoverIndex: getHoverColumnIndexByOrder(index - 1),
					headerCellClass: getTimesheetColumnClassname(index),
					isMobileView,
				};
			});

		return [
			{
				...getUserTimesheetColumn(isMobileView),
				name: t('name'),
			},
			...dateColumns,
			createTotalColumn(areColumnsFrozen, period.length),
		];
	}, [period, isPeriodByMonth, columnWidth, areColumnsFrozen, isMobileView]);

	return columns as Array<Column<TimeSheetManagerRow>>;
};
