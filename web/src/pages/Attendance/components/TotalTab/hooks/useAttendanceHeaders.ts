import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	ATTENDANCE_TOTAL_GRID_COLUMNS,
	getAttendanceTotalColumns,
	ATTENDANCE_TOTAL_FILTER_KEYS,
} from '../constants';
import { summaryTimeCellRenderer } from '../renderers/summaryTimeCellRenderer';
import { timeCellRenderer } from '../renderers/timeCellRenderer';
import {
	AttendanceTotalFilters,
	AttendanceTotalGridRow,
	AttendanceTotalSetFilters,
} from '../types';

import {
	FreeTextAutoComplete,
	FreeTextModal,
	FromToProps,
	SelectionModal,
	cellWithTooltipRenderer,
	editCellRenderer,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromHoursToHoursTimeModal } from '@/components/Grid/modals/fromHoursToHoursModalRenderer';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { SelectionModalOption } from '@/components/Grid/renderers/header/types';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { fromToDateObject } from '@/constants/defaultValues';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFiltersEqualByKeys,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { isDateRangeApplyDisabled } from '@/helpers/isDateRangeApplyDisabled';
import { TRACKED_TIME_DEFAULT_VALUE } from '@/pages/constants';

interface AttendanceColumnsFiltersProps {
	idFilter: string;
	userFilter: string;
	departmentsFilter: string[];
	groupFilter: string;
	dateFilter: FromToProps<Date>;
	timeFilter: FromToProps<string>;
	setFilters: AttendanceTotalSetFilters;
	initialFilters: AttendanceTotalFilters;
	departamentsOptions?: SelectionModalOption[];
	userOptions?: string[];
	groupOptions?: string[];
}

export const useAttendanceHeaders = ({
	idFilter,
	userFilter,
	departmentsFilter,
	groupFilter,
	dateFilter,
	timeFilter,
	setFilters,
	initialFilters,
	userOptions = [],
	departamentsOptions = [],
	groupOptions = [],
}: AttendanceColumnsFiltersProps): Array<Column<AttendanceTotalGridRow>> => {
	const { role } = useAuth();

	const currentFilters: AttendanceTotalFilters = {
		userId: idFilter,
		user: userFilter,
		departments: departmentsFilter,
		group: groupFilter,
		date: dateFilter,
		time: timeFilter,
	};

	const filterKeys = ATTENDANCE_TOTAL_FILTER_KEYS;

	const isEditDisabled = areFiltersEqualByKeys(
		currentFilters,
		initialFilters,
		filterKeys
	);

	const handleClear = () => {
		setFilters.reset();
	};

	const columnWidth = useMemo(() => getAttendanceTotalColumns(role), [role]);

	const attendanceColumns: Array<Column<AttendanceTotalGridRow>> = useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: ATTENDANCE_TOTAL_GRID_COLUMNS.userId.key,
				name: ATTENDANCE_TOTAL_GRID_COLUMNS.userId.name,
				sortable: true,
				width: getColumnWidth(columnWidth.userId).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: idFilter,
						setFilter: setFilters.userId as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: idFilter !== '',
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_TOTAL_GRID_COLUMNS.user.key,
				name: ATTENDANCE_TOTAL_GRID_COLUMNS.user.name,
				sortable: true,
				width: getColumnWidth(columnWidth.user).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: userFilter,
						setFilter: setFilters.user as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextAutoComplete,
						autocompleteOptions: userOptions,
						isFilterApplied: userFilter !== '',
						areValuesEquals: arePrimitiveValuesEqual,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_TOTAL_GRID_COLUMNS.department.key,
				name: ATTENDANCE_TOTAL_GRID_COLUMNS.department.name,
				sortable: true,
				width: getColumnWidth(columnWidth.department).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<AttendanceTotalGridRow, string[]>({
						...props,
						filter: departmentsFilter || [],
						isFilterApplied: departmentsFilter.length > 0,
						setFilter: setFilters.departments as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: departamentsOptions,
						defaultValue: initialFilters.departments,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_TOTAL_GRID_COLUMNS.group.key,
				name: ATTENDANCE_TOTAL_GRID_COLUMNS.group.name,
				sortable: true,
				width: getColumnWidth(columnWidth.group).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: groupFilter,
						isFilterApplied: Boolean(groupFilter),
						setFilter: setFilters.group as Dispatch<SetStateAction<string>>,
						renderModal: FreeTextAutoComplete,
						areValuesEquals: arePrimitiveValuesEqual,
						autocompleteOptions: groupOptions,
						defaultValue: '',
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_TOTAL_GRID_COLUMNS.date.key,
				name: ATTENDANCE_TOTAL_GRID_COLUMNS.date.name,
				sortable: true,
				width: getColumnWidth(columnWidth.date).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<
						AttendanceTotalGridRow,
						FromToProps<Date>
					>({
						...props,
						filter: dateFilter || fromToDateObject,
						setFilter: setFilters.date as Dispatch<
							SetStateAction<FromToProps<Date>>
						>,
						defaultValue: fromToDateObject,
						renderModal: FromToDateModal,
						isFilterApplied: !!dateFilter?.from || !!dateFilter?.to,
						isApplyButtonDisabled: isDateRangeApplyDisabled,
						areValuesEquals: areFromToCompatibleObjectsEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			{
				...getColumnWithTooltipFactory({
					key: ATTENDANCE_TOTAL_GRID_COLUMNS.time.key,
					name: ATTENDANCE_TOTAL_GRID_COLUMNS.time.name,
					sortable: true,
					width: getColumnWidth(columnWidth.time).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<
							AttendanceTotalGridRow,
							FromToProps<string>
						>({
							...props,
							filter: timeFilter || TRACKED_TIME_DEFAULT_VALUE,
							setFilter: setFilters.time as Dispatch<
								SetStateAction<FromToProps<string>>
							>,
							defaultValue: TRACKED_TIME_DEFAULT_VALUE,
							renderModal: FromHoursToHoursTimeModal,
							isFilterApplied:
								timeFilter?.from !== null || timeFilter?.to !== null,
							areValuesEquals: areFromToCompatibleObjectsEqual,
						}),
					renderCell: (props) => timeCellRenderer(props),
					renderSummaryCell: summaryTimeCellRenderer,
				}),
			},
			getEditColumnFactory({
				key: '',
				name: '',
				disabled: isEditDisabled,
				renderCell: (props) => editCellRenderer(props),
				clearFilter: handleClear,
				sortable: false,
				frozen: true,
				headerCellClass: EDIT_COLUMN_HEADER_CLASS,
				cellClass: EDIT_COLUMN_CELL_CLASS,
			}),
		],
		[
			idFilter,
			userFilter,
			departmentsFilter,
			groupFilter,
			dateFilter,
			timeFilter,
			departamentsOptions,
			userOptions,
			groupOptions,
			initialFilters,
			isEditDisabled,
			columnWidth,
		]
	);

	return useMemo(
		() => attendanceColumns.filter(({ width }) => width !== 0),
		[attendanceColumns]
	);
};
