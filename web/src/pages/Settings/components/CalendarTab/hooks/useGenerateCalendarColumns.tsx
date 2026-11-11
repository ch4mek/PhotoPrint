import { useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	CALENDAR_COUNTRY_COLUMN_HEADER_CLASS,
	CALENDAR_TOTAL_COLUMN_HEADER_CLASS,
	COUNTRY_NAME_COLUMN_INDEX,
	desktopCalendarGridWidths,
	TOTAL_COLUMN_INDEX,
} from '../constants';
import { countryNameRenderer } from '../renderers/countryNameRenderer';
import { hoursHeaderCellRenderer } from '../renderers/headerCellRenderer';
import { hoursCellRenderer } from '../renderers/hoursCellRenderer';
import { totalCellRenderer } from '../renderers/totalCellRenderer';
import {
	getCalendarCellClassname,
	getCountryNameCellClassname,
	getTotalCellClassname,
} from '../utils';

import { PERIOD } from '@/constants/date';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	HOVER_TOTAL_CELL_CLASS_NAME,
} from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { getTimesheetColumnClassname } from '@/pages/TimeSheet/utils/getTimesheetColumnClassname';
import { ColumnWithHoverIndex } from '@/types/columnHoverTypes';
import { WorkingHoursCalendarRow } from '@/types/settings';

const MONTH_COLUMN_INDEX_OFFSET = 2;

const createTotalColumn = (
	isFrozen: boolean,
	isLoading: boolean
): ColumnWithHoverIndex<WorkingHoursCalendarRow> => ({
	name: 'Total, h',
	key: 'total',
	renderCell: (props) => totalCellRenderer(props, isLoading),
	renderHeaderCell: hoursHeaderCellRenderer,
	frozen: isFrozen,
	width: desktopCalendarGridWidths.total,
	headerCellClass: `${CALENDAR_TOTAL_COLUMN_HEADER_CLASS} ${HOVER_TOTAL_CELL_CLASS_NAME}`,
	cellClass: ({ type }) => getTotalCellClassname(type),
	hoverIndex: TOTAL_COLUMN_INDEX,
});

const createCountryNameColumn = (
	isFrozen: boolean,
	isLoading: boolean
): ColumnWithHoverIndex<WorkingHoursCalendarRow> => ({
	name: '',
	key: 'country',
	width: desktopCalendarGridWidths.countryName,
	frozen: isFrozen,
	renderHeaderCell: hoursHeaderCellRenderer,
	renderCell: (props) => countryNameRenderer(props, isLoading),
	headerCellClass: `${CALENDAR_COUNTRY_COLUMN_HEADER_CLASS} ${HOVER_NAME_CELL_CLASS_NAME}`,
	cellClass: ({ type }) => getCountryNameCellClassname(type),
	hoverIndex: COUNTRY_NAME_COLUMN_INDEX,
});

export const useGenerateCalendarColumns = (
	isLoading: boolean,
	isCountryNameColumnFrozen: boolean,
	isTotalColumnFrozen: boolean
) =>
	useMemo(() => {
		const inputColumns: Array<ColumnWithHoverIndex<WorkingHoursCalendarRow>> =
			PERIOD.map((month, index) => ({
				name: `${month}, h`,
				key: month,
				width: desktopCalendarGridWidths.month,
				renderCell: (props) => hoursCellRenderer(props, index + 1, isLoading),
				renderHeaderCell: hoursHeaderCellRenderer,
				hoverIndex: index + MONTH_COLUMN_INDEX_OFFSET,
				headerCellClass: getTimesheetColumnClassname(index),
				cellClass: ({ type }) => getCalendarCellClassname(index, type),
			}));

		return [
			createCountryNameColumn(isCountryNameColumnFrozen, isLoading),
			...inputColumns,
			createTotalColumn(isTotalColumnFrozen, isLoading),
		] as Column<WorkingHoursCalendarRow>[];
	}, [isCountryNameColumnFrozen, isTotalColumnFrozen, isLoading]);
