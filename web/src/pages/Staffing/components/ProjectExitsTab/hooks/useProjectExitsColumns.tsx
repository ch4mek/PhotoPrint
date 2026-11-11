import { PickerButtonOption } from '@mms/mms-ui-library/components/PickerButton';
import {
	BasketIcon,
	EditIcon,
	RefreshIcon,
} from '@mms/mms-ui-library/icons/base';
import { Dispatch, SetStateAction, useMemo } from 'react';
import { Column } from 'react-data-grid';

import {
	PROJECT_ID_DEFAULT_VALUE,
	initialProjectExistsFilters,
	getProjectExitsColumnsWidths,
	projectExitGridColumns,
	projectExitsMenuOptionsValues,
} from '../constants';
import { UseProjectExitsProps } from '../types';

import {
	FreeTextModal,
	SelectionModal,
	cellWithTooltipRenderer,
	getLogicalTextColumnFactory,
	getColumnFactory,
	getColumnWithTooltipFactory,
	getEditColumnFactory,
	headerCellWithFilterRenderer,
	FreeTextAutoComplete,
	FromToProps,
	cellRenderer,
} from '@/components/Grid';
import {
	EDIT_COLUMN_CELL_CLASS,
	EDIT_COLUMN_HEADER_CLASS,
} from '@/components/Grid/constants';
import { FromToDateModal } from '@/components/Grid/modals/fromToDateModalRenderer';
import { menuOptionsCellRenderer } from '@/components/Grid/renderers/cell/menuOptionsCellRenderer';
import { getColumnWidth } from '@/components/Grid/utils/getColumnWidth';
import {
	activityWithDeletedOptions,
	statusWithDeletedOptions,
} from '@/constants/activityStatus';
import { fromToDateObject } from '@/constants/defaultValues';
import { isChiefRole, isManagerRole } from '@/constants/roles';
import { useAuth } from '@/context/AuthContext';
import {
	areArraysEqualByPrimitiveItems,
	areFromToCompatibleObjectsEqual,
	arePrimitiveValuesEqual,
} from '@/helpers/areValuesEqual';
import { checkIsHasClaims } from '@/helpers/checkIsHasClaims';
import { isDateRangeApplyDisabled } from '@/helpers/isDateRangeApplyDisabled';
import { IS_ACTIVE_DEFAULT_VALUE } from '@/pages/constants';
import { UserClaims } from '@/types/claims';
import { ProjectExitsGridRow } from '@/types/projectExits';

export const useProjectExitsColumns = ({
	filters,
	setFilters,
	isResetFiltersDisabled,
	options,
	canManageProjectExits,
	onCellClick,
	handleDelete,
	handleRestore,
	handleEdit,
}: UseProjectExitsProps) => {
	const { role, claims } = useAuth();

	const showDepartment =
		checkIsHasClaims([UserClaims.ManageAllProjectExits], claims) ||
		isChiefRole(role);

	const showGroup =
		checkIsHasClaims([UserClaims.ManageAllProjectExits], claims) ||
		isManagerRole(role);

	const projectExitsColumnsWidths = useMemo(
		() => getProjectExitsColumnsWidths(showDepartment, showGroup),
		[showDepartment, showGroup]
	);

	const projectExitsColumns: Array<Column<ProjectExitsGridRow>> =
		useMemo(() => {
			const {
				idFilter,
				userFilter,
				departmentFilter,
				groupFilter,
				clientFilter,
				projectFilter,
				closingDateFilter,
				closingReasonFilter,
				statusFilter,
			} = filters;

			const departmentColumn: Column<ProjectExitsGridRow> =
				getColumnWithTooltipFactory({
					...projectExitGridColumns.department,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string[]>({
							...props,
							filter: departmentFilter || [],
							isFilterApplied: departmentFilter.length > 0,
							setFilter: setFilters.departmentFilter as Dispatch<
								SetStateAction<string[]>
							>,
							renderModal: SelectionModal,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							options: options.departments,
							defaultValue: initialProjectExistsFilters.departmentFilter,
							disableFilterIconActiveStateAsDefault: true,
						}),
					renderCell: (props) =>
						cellWithTooltipRenderer(props, {
							isCopyEnabled: true,
						}),
					width: getColumnWidth(projectExitsColumnsWidths.department || 0)
						.width,
				});

			const groupColumn: Column<ProjectExitsGridRow> =
				getColumnWithTooltipFactory({
					...projectExitGridColumns.group,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: groupFilter,
							setFilter: setFilters.groupFilter as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							isFilterApplied: groupFilter !== '',
							areValuesEquals: arePrimitiveValuesEqual,
							autocompleteOptions: options.groups,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(projectExitsColumnsWidths.group || 0).width,
				});

			return [
				getColumnWithTooltipFactory({
					...projectExitGridColumns.id,
					width: getColumnWidth(projectExitsColumnsWidths.id).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer({
							...props,
							filter: idFilter,
							setFilter: setFilters.idFilter as Dispatch<
								SetStateAction<string>
							>,
							defaultValue: '',
							renderModal: FreeTextModal,
							isFilterApplied: idFilter !== PROJECT_ID_DEFAULT_VALUE,
							areValuesEquals: arePrimitiveValuesEqual,
						}),
					renderCell: (props) =>
						cellWithTooltipRenderer(props, {
							isCopyEnabled: true,
						}),
				}),
				getColumnWithTooltipFactory({
					...projectExitGridColumns.user,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string>({
							...props,
							filter: userFilter,
							isFilterApplied: !!userFilter,
							setFilter: setFilters.userFilter as Dispatch<
								SetStateAction<string>
							>,
							areValuesEquals: arePrimitiveValuesEqual,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: options.users,
							disableFilterIconActiveStateAsDefault: true,
						}),
					width: getColumnWidth(projectExitsColumnsWidths.user).width,
					renderCell: (props) =>
						cellWithTooltipRenderer(props, {
							onCellClick,
							isClickable: !!props.row.canViewPersonalInfo,
						}),
				}),
				...(showDepartment ? [departmentColumn] : []),
				...(showGroup ? [groupColumn] : []),
				getColumnFactory({
					...projectExitGridColumns.client,
					width: getColumnWidth(projectExitsColumnsWidths.client).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string>({
							...props,
							filter: clientFilter,
							setFilter: setFilters.clientFilter as Dispatch<
								SetStateAction<string>
							>,
							isFilterApplied: !!clientFilter,
							areValuesEquals: arePrimitiveValuesEqual,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: options.clients,
							disableFilterIconActiveStateAsDefault: true,
						}),
				}),
				getColumnWithTooltipFactory({
					...projectExitGridColumns.project,
					width: getColumnWidth(projectExitsColumnsWidths.project).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string>({
							...props,
							filter: projectFilter,
							setFilter: setFilters.projectFilter as Dispatch<
								SetStateAction<string>
							>,
							isFilterApplied: !!projectFilter,
							areValuesEquals: arePrimitiveValuesEqual,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: options.projects,
							disableFilterIconActiveStateAsDefault: true,
						}),
				}),
				getColumnWithTooltipFactory({
					...projectExitGridColumns.closingDate,
					width: getColumnWidth(projectExitsColumnsWidths.closingDate).width,
					sortable: true,
					resizable: false,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<
							ProjectExitsGridRow,
							FromToProps<Date>
						>({
							...props,
							filter: closingDateFilter,
							setFilter: setFilters.closingDateFilter as Dispatch<
								SetStateAction<FromToProps<Date>>
							>,
							defaultValue: fromToDateObject,
							renderModal: FromToDateModal,
							isFilterApplied:
								!!closingDateFilter?.from || !!closingDateFilter?.to,
							isApplyButtonDisabled: isDateRangeApplyDisabled,
							areValuesEquals: areFromToCompatibleObjectsEqual,
						}),
					renderCell: cellRenderer,
				}),
				getColumnFactory({
					...projectExitGridColumns.closingReason,
					width: getColumnWidth(projectExitsColumnsWidths.closingReason).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string>({
							...props,
							filter: closingReasonFilter,
							setFilter: setFilters.closingReasonFilter as Dispatch<
								SetStateAction<string>
							>,
							isFilterApplied: !!closingReasonFilter,
							areValuesEquals: arePrimitiveValuesEqual,
							defaultValue: '',
							renderModal: FreeTextAutoComplete,
							autocompleteOptions: options.reasons,
							disableFilterIconActiveStateAsDefault: true,
						}),
				}),
				getLogicalTextColumnFactory({
					...projectExitGridColumns.status,
					logicalTextOptions: activityWithDeletedOptions,
					width: getColumnWidth(projectExitsColumnsWidths.status).width,
					renderHeaderCell: (props) =>
						headerCellWithFilterRenderer<ProjectExitsGridRow, string[]>({
							...props,
							areValuesEquals: areArraysEqualByPrimitiveItems,
							filter: statusFilter,
							setFilter: setFilters.statusFilter as Dispatch<
								SetStateAction<string[]>
							>,
							renderModal: SelectionModal,
							isFilterApplied: statusFilter.length > 0,
							options: statusWithDeletedOptions,
							defaultValue: IS_ACTIVE_DEFAULT_VALUE,
							disableFilterIconActiveStateAsDefault: true,
						}),
				}),
				getEditColumnFactory({
					key: '',
					name: '',
					disabled: isResetFiltersDisabled,
					renderCell: (props) =>
						menuOptionsCellRenderer(props, {
							options: (row) => {
								const menuOptions: PickerButtonOption<ProjectExitsGridRow>[] =
									[];

								if (!canManageProjectExits) {
									return menuOptions;
								}

								if (row.isActive) {
									menuOptions.push(
										{
											value: projectExitsMenuOptionsValues.edit,
											addon: <EditIcon />,
											onClick: () => handleEdit(row),
										},
										{
											value: projectExitsMenuOptionsValues.delete,
											addon: <BasketIcon />,
											onClick: () => handleDelete(row),
										}
									);
								} else {
									menuOptions.push({
										value: projectExitsMenuOptionsValues.restore,
										addon: <RefreshIcon />,
										onClick: () => handleRestore(row),
									});
								}

								return menuOptions;
							},
						}),
					clearFilter: setFilters.reset,
					frozen: true,
					sortable: false,
					headerCellClass: EDIT_COLUMN_HEADER_CLASS,
					cellClass: EDIT_COLUMN_CELL_CLASS,
				}),
			];
		}, [
			filters,
			setFilters,
			options,
			projectExitsColumnsWidths,
			showDepartment,
			showGroup,
			isResetFiltersDisabled,
			canManageProjectExits,
			onCellClick,
			handleDelete,
			handleRestore,
			handleEdit,
		]);

	return { columns: projectExitsColumns, projectExitsColumnsWidths };
};
