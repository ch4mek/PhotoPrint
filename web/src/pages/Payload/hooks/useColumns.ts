import { useCallback, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	editCellRenderer,
	FreeTextAutoComplete,
	FreeTextModal,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	SelectionModal,
} from '@/components/Grid';
import { EDIT_COLUMN_SUMMARY_CLASS } from '@/components/Grid/constants';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import { PERIOD } from '@/constants/date';
import { techDepartments } from '@/constants/departments';
import {
	areArraysEqualByPrimitiveItems,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { SelectionModalWithDepartments } from '@/pages/Payload/components/SelectionModalWithDepartments';
import {
	LOGIC_OPTIONS,
	PAYLOAD_COLUMNS_WIDTH,
	PAYLOAD_GRID_COLUMNS,
	START_MONTH_COLUMN_INDEX,
} from '@/pages/Payload/contants';
import { defaultCellRenderer } from '@/pages/Payload/renderers/defaultCellRenderer';
import { employeeCellRenderer } from '@/pages/Payload/renderers/employeeCellRenderer';
import { MonthCellRenderer } from '@/pages/Payload/renderers/monthCellRenderer';
import { monthHeaderCellRenderer } from '@/pages/Payload/renderers/monthHeaderCellRenderer';
import { ProjectCellRenderer } from '@/pages/Payload/renderers/projectCellRenderer';
import { summaryMonthCellRenderer } from '@/pages/Payload/renderers/summaryMonthCellRenderer';
import { PayloadColumnsProps, PayloadGridRow } from '@/pages/Payload/types';
import { ColumnWithHoverIndex } from '@/types/columnHoverTypes';

const monthColumns: Array<
	ColumnWithHoverIndex<PayloadGridRow, PayloadGridRow>
> = PERIOD.map((month, index) => ({
	key: month,
	name: month,
	renderSummaryCell: (props) => summaryMonthCellRenderer(props),
	renderHeaderCell: (props) => monthHeaderCellRenderer(props),
	renderCell: (props) => MonthCellRenderer(props),
	hoverIndex: index + START_MONTH_COLUMN_INDEX,
}));

export function useGetGridColumns({
	filters: {
		commentFilter,
		departmentFilter,
		employeeFilter,
		idFilter,
		npFilter,
		planFilter,
		projectFilter,
		groupFilter,
	},
	setters: {
		setCommentFilter,
		setDepartmentFilter,
		setEmployeeFilter,
		setIdFilter,
		setNpFilter,
		setPlanFilter,
		setProjectFilter,
		setGroupFilter,
	},
	options,
}: PayloadColumnsProps) {
	const isResetFiltersDisabled =
		idFilter === '' &&
		commentFilter === '' &&
		areArraysEqualByPrimitiveItems(departmentFilter, techDepartments) &&
		employeeFilter === '' &&
		groupFilter === '' &&
		!npFilter.length &&
		projectFilter === '' &&
		planFilter === '';

	const handleClear = useCallback(() => {
		setCommentFilter('');
		setDepartmentFilter(techDepartments);
		setEmployeeFilter('');
		setIdFilter('');
		setNpFilter([]);
		setPlanFilter('');
		setProjectFilter('');
		setGroupFilter('');
	}, [
		setCommentFilter,
		setDepartmentFilter,
		setEmployeeFilter,
		setIdFilter,
		setNpFilter,
		setPlanFilter,
		setProjectFilter,
		setGroupFilter,
	]);

	const {
		departments,
		displayDepartmentColumn,
		displayGroupColumn,
		projects,
		users,
		groups,
	} = options;

	const idColumn: Column<PayloadGridRow> = useMemo(
		() => ({
			...PAYLOAD_GRID_COLUMNS.payloadId,
			renderCell: (props) =>
				defaultCellRenderer({
					props,
					options: {
						withCircle: true,
					},
				}),
			width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.id).width,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: idFilter,
					setFilter: setIdFilter,
					defaultValue: '',
					renderModal: FreeTextModal,
					isFilterApplied: idFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
				}),
		}),
		[idFilter]
	);
	const depColumn: Column<PayloadGridRow> = useMemo(
		() => ({
			...PAYLOAD_GRID_COLUMNS.payloadDepartment,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: departmentFilter,
					setFilter: setDepartmentFilter,

					defaultValue: techDepartments,
					renderModal: SelectionModalWithDepartments,
					options: departments,
					isFilterApplied: !!departmentFilter?.length,
					areValuesEquals: areArraysEqualByPrimitiveItems,
					disableFilterIconActiveStateAsDefault: true,
				}),
			width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.department).width,
			renderCell: (props) =>
				defaultCellRenderer({
					props,
					options: {
						withTooltip: true,
						isDepartmentCell: true,
					},
				}),
		}),
		[departmentFilter, departments]
	);

	const groupColumn: Column<PayloadGridRow> = useMemo(
		() => ({
			...PAYLOAD_GRID_COLUMNS.payloadGroup,
			renderHeaderCell: (props) =>
				headerCellWithFilterRenderer({
					...props,
					filter: groupFilter,
					setFilter: setGroupFilter,
					defaultValue: '',
					renderModal: FreeTextAutoComplete,
					isFilterApplied: groupFilter !== '',
					areValuesEquals: arePrimitiveValuesEqual,
					autocompleteOptions: groups,
					disableFilterIconActiveStateAsDefault: true,
				}),
			width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.group).width,
			renderCell: (props) =>
				defaultCellRenderer({
					props,
					options: {
						withTooltip: true,
						isDepartmentCell: true,
					},
				}),
		}),
		[groupFilter, groups]
	);

	const afterDepColumns = useMemo(
		() =>
			[
				{
					...PAYLOAD_GRID_COLUMNS.payloadEmployee,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: employeeFilter,
							setFilter: setEmployeeFilter,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							isFilterApplied: employeeFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							autocompleteOptions: users,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.employee).width,
					renderCell: employeeCellRenderer,
				},
				{
					...PAYLOAD_GRID_COLUMNS.payloadComment,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: commentFilter,
							setFilter: setCommentFilter,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: commentFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.comments).width,
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
				},
				{
					...PAYLOAD_GRID_COLUMNS.payloadNp,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							filter: npFilter,
							setFilter: setNpFilter,
							renderModal: SelectionModal,
							isFilterApplied: npFilter.length > 0,
							options: LOGIC_OPTIONS,
							defaultValue: [],
							disableFilterIconActiveStateAsDefault: true,
						}),
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								isLogicCell: true,
							},
						}),
				},
				{
					...PAYLOAD_GRID_COLUMNS.payloadProject,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: projectFilter,
							setFilter: setProjectFilter,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							isFilterApplied: projectFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							autocompleteOptions: projects,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.projects).width,
					renderCell: ProjectCellRenderer,
					renderSummaryCell: ProjectCellRenderer,
				},
				...monthColumns,
				{
					...PAYLOAD_GRID_COLUMNS.payloadPlan,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: planFilter,
							setFilter: setPlanFilter,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: planFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					width: getColumnWidth(PAYLOAD_COLUMNS_WIDTH.plan).width,
					renderCell: (props) =>
						defaultCellRenderer({
							props,
							options: {
								withTooltip: true,
								isResizable: true,
							},
						}),
				},
				getEditColumnFactory({
					key: '',
					name: '',
					sortable: false,
					frozen: true,
					summaryCellClass: EDIT_COLUMN_SUMMARY_CLASS,
					disabled: isResetFiltersDisabled,
					renderCell: (props) => editCellRenderer(props, { isDisabled: true }),
					clearFilter: handleClear,
				}),
			] as Array<Column<PayloadGridRow>>,
		[
			commentFilter,
			employeeFilter,
			planFilter,
			npFilter,
			projectFilter,
			users,
			projects,
		]
	);

	return [
		idColumn,
		...(displayDepartmentColumn ? [depColumn] : []),
		...(displayGroupColumn ? [groupColumn] : []),
		...afterDepColumns,
	];
}
