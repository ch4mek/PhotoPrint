import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	ATTENDANCE_DETAILED_GRID_COLUMNS,
	DIRECTION_OPTIONS,
	getAttendanceDetailedColumns,
} from '../constants';
import {
	AttendanceDetailedFilters,
	AttendanceDetailedGridRow,
	AttendanceDetailedSetFilters,
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
import { AttendenceDirection } from '@/types/attendances';

interface AttendanceColumnsFiltersProps {
	idFilter: string;
	userFilter: string;
	departmentsFilter: string[];
	groupFilter: string;
	directionFilter: AttendenceDirection[];
	doorFilter: string;
	locationFilter: string;
	dateFilter: FromToProps<Date>;
	timeFilter: FromToProps<string>;
	setFilters: AttendanceDetailedSetFilters;
	initialFilters: AttendanceDetailedFilters;
	departamentsOptions?: SelectionModalOption[];
	userOptions?: string[];
	groupOptions?: string[];
}

export const useAttendanceHeaders = ({
	idFilter,
	userFilter,
	departmentsFilter,
	groupFilter,
	directionFilter,
	doorFilter,
	locationFilter,
	dateFilter,
	timeFilter,
	setFilters,
	initialFilters,
	userOptions = [],
	departamentsOptions = [],
	groupOptions = [],
}: AttendanceColumnsFiltersProps): Array<Column<AttendanceDetailedGridRow>> => {
	const { role } = useAuth();

	const currentFilters: AttendanceDetailedFilters = {
		id: idFilter,
		user: userFilter,
		departments: departmentsFilter,
		group: groupFilter,
		direction: directionFilter,
		door: doorFilter,
		location: locationFilter,
		date: dateFilter,
		time: timeFilter,
	};

	const detailedFilterKeys = Object.keys(initialFilters) as Array<
		keyof AttendanceDetailedFilters
	>;

	const isEditDisabled = areFiltersEqualByKeys(
		currentFilters,
		initialFilters,
		detailedFilterKeys
	);

	const handleClear = () => {
		setFilters.reset();
	};

	const columnWidth = useMemo(() => getAttendanceDetailedColumns(role), [role]);

	const attendanceColumns: Array<Column<AttendanceDetailedGridRow>> = useMemo(
		() => [
			getColumnWithTooltipFactory({
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.id.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.id.name,
				sortable: true,
				width: getColumnWidth(columnWidth.id).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: idFilter,
						setFilter: setFilters.id as Dispatch<SetStateAction<string>>,
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
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.user.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.user.name,
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
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.department.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.department.name,
				sortable: true,
				width: getColumnWidth(columnWidth.department).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<AttendanceDetailedGridRow, string[]>({
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
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.group.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.group.name,
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
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.direction.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.direction.name,
				sortable: true,
				width: getColumnWidth(columnWidth.direction).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<AttendanceDetailedGridRow, string[]>({
						...props,
						filter: directionFilter,
						isFilterApplied: directionFilter.length > 0,
						setFilter: setFilters.direction as Dispatch<
							SetStateAction<string[]>
						>,
						renderModal: SelectionModal,
						areValuesEquals: areArraysEqualByPrimitiveItems,
						options: DIRECTION_OPTIONS,
						defaultValue: initialFilters.direction,
						disableFilterIconActiveStateAsDefault: true,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.door.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.door.name,
				sortable: true,
				width: getColumnWidth(columnWidth.door).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: doorFilter,
						setFilter: setFilters.door as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: doorFilter !== '',
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.location.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.location.name,
				sortable: true,
				width: getColumnWidth(columnWidth.location).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer({
						...props,
						filter: locationFilter,
						setFilter: setFilters.location as Dispatch<SetStateAction<string>>,
						defaultValue: '',
						renderModal: FreeTextModal,
						isFilterApplied: locationFilter !== '',
						areValuesEquals: arePrimitiveValuesEqual,
					}),
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
			getColumnWithTooltipFactory({
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.date.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.date.name,
				sortable: true,
				width: getColumnWidth(columnWidth.date).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<
						AttendanceDetailedGridRow,
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
			getColumnWithTooltipFactory({
				key: ATTENDANCE_DETAILED_GRID_COLUMNS.time.key,
				name: ATTENDANCE_DETAILED_GRID_COLUMNS.time.name,
				sortable: true,
				width: getColumnWidth(columnWidth.time).width,
				renderHeaderCell: (props) =>
					headerCellWithFilterRenderer<
						AttendanceDetailedGridRow,
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
				renderCell: (props) =>
					cellWithTooltipRenderer(props, {
						isCopyEnabled: true,
					}),
			}),
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
			directionFilter,
			doorFilter,
			locationFilter,
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
