import { Dispatch, SetStateAction, useMemo } from 'react';
import { RenderCellProps, RenderHeaderCellProps } from 'react-data-grid';

import { CalendarTimelineItem } from '../../CalendarTimelineItemRenderer';
import {
	CALENDAR_DAY_HEADER_CLASSNAME,
	MINIMAL_DATE_COLUMN_WIDTH,
} from '../constants';
import {
	UseAbsenceCalendarColumnsParams,
	UseAbsenceColumnsReturn,
} from '../types';
import {
	getMinimalRequiredGridLength,
	getColumnWidthFromGridOffsetWidth,
} from '../utils';

import {
	cellWithTooltipRenderer,
	FreeTextAutoComplete,
	getColumnWithTooltipFactory,
	GridCellDefaultProps,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import dayjs from '@/lib/dayjs';
import {
	HOVER_NAME_CELL_CLASS_NAME,
	USER_INFO_CELL_CLASS,
	USER_NAME_COLUMN_HEADER_CLASS_NAME,
} from '@/pages/TimeSheet/components/TimeSheeetManager/constants';
import { TIMESHEET_DATE_FORMAT } from '@/pages/TimeSheet/components/TimesheetEngineer/constants';
import { renderMonthHeaderCell } from '@/pages/TimeSheet/components/TimesheetEngineer/GridRenderers/renderDateHeaderCell';
import { getHoverColumnIndexByOrder } from '@/pages/TimeSheet/utils/getHoverColumnIndexByOrder';
import { InquiryCalendarGroupRow } from '@/types/absences';
import { RenderHeaderCellPropsWithHoverIndex } from '@/types/columnHoverTypes';

function renderAbsenceItemCell(day: Date, columnWidth: number) {
	const dayDate = day.getDate();
	const dayMonth = day.getMonth();

	return ({ row }: RenderCellProps<InquiryCalendarGroupRow>) =>
		row.inquiries.map((item) => {
			const {
				level,
				id,
				inquiryType,
				inquiryName,
				status,
				dateFrom,
				dateTo,
				totalAcceptUsersCount,
				totalAcceptedCount,
				totalHours,
				isStartPart,
				isEndPart,
			} = item;

			const start = dayjs(dateFrom);

			if (start.date() !== dayDate || start.month() !== dayMonth) {
				return null;
			}

			return (
				<CalendarTimelineItem
					key={id}
					cellWidth={columnWidth}
					level={level}
					inquiry={{
						inquiryId: id,
						inquiryType,
						inquiryName,
						inquiryStatus: status,
					}}
					startDate={dateFrom}
					endDate={dateTo}
					alreadyApprovedCount={totalAcceptUsersCount}
					totalApproversCount={totalAcceptedCount}
					totalHours={totalHours}
					isStartPart={isStartPart}
					isEndPart={isEndPart}
				/>
			);
		});
}

export const useAbsenceCalendarColumns = ({
	period,
	gridOffsetWidth,
	absenceColumns,
	filters,
	setFilters,
	options,
	enableCellClick,
	onUserClick,
}: UseAbsenceCalendarColumnsParams): UseAbsenceColumnsReturn => {
	const fixedColumnsWidth = Object.values(absenceColumns).reduce(
		(acc, curr) => acc + curr.width,
		0
	);

	const dateColumnWidth =
		getMinimalRequiredGridLength(
			period.length,
			MINIMAL_DATE_COLUMN_WIDTH,
			fixedColumnsWidth
		) > gridOffsetWidth
			? MINIMAL_DATE_COLUMN_WIDTH
			: getColumnWidthFromGridOffsetWidth(
					gridOffsetWidth,
					period.length,
					fixedColumnsWidth
			  );

	const columns = useMemo(() => {
		const dateColumns = period.map((day, index) => {
			const dateKey = dayjs(day).format(TIMESHEET_DATE_FORMAT);

			return {
				name: dateKey,
				key: dateKey,
				renderHeaderCell: (
					props: RenderHeaderCellPropsWithHoverIndex<InquiryCalendarGroupRow>
				) => renderMonthHeaderCell(props, true),
				renderCell: renderAbsenceItemCell(day, dateColumnWidth),
				width: dateColumnWidth,
				hoverIndex: getHoverColumnIndexByOrder(index - 1),
				headerCellClass: dayjs(day).isSame(dayjs(), 'day')
					? CALENDAR_DAY_HEADER_CLASSNAME
					: '',
			};
		});

		return [
			getColumnWithTooltipFactory({
				...absenceColumns.userFullName,
				sortable: true,
				renderHeaderCell: (
					props: RenderHeaderCellProps<InquiryCalendarGroupRow>
				) =>
					headerCellWithFilterRenderer({
						...props,
						filter: filters.userFullName,
						isFilterApplied: !!filters.userFullName,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						areValuesEquals: arePrimitiveValuesEqual,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: options.users,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props: RenderCellProps<InquiryCalendarGroupRow>) =>
					cellWithTooltipRenderer(props, {
						onCellClick: enableCellClick(props.row) ? onUserClick : undefined,
						isClickable: !!props.row.isManagedByRoleRequest,
					}),
				cellClass: USER_INFO_CELL_CLASS,
				headerCellClass: `${USER_NAME_COLUMN_HEADER_CLASS_NAME} ${HOVER_NAME_CELL_CLASS_NAME}`,
			}),
			getColumnWithTooltipFactory({
				...absenceColumns.department,
				sortable: true,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<GridCellDefaultProps, string[]>({
						...props,
						filter: filters.userDepartment || [],
						isFilterApplied: filters.userDepartment.length > 0,
						setFilter: setFilters.departments as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: options.departments,
						defaultValue: [],
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: cellWithTooltipRenderer,
			}),
			...dateColumns,
		];
	}, [
		period,
		dateColumnWidth,
		absenceColumns,
		filters,
		setFilters,
		options,
		onUserClick,
		enableCellClick,
	]);

	return { columns, dateColumnWidth } as UseAbsenceColumnsReturn;
};
